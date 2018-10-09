using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;

using AspNetCore.Identity.Neo4j.Internal;
using Neo4jExtras;
using Neo4jExtras.Extensions;

namespace AspNetCore.Identity.Neo4j
{
    /// <summary>
    /// Creates a new instance of a persistence store for the specified user type.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    public class Neo4jUserStore<TUser> :
        Neo4jUserStore<TUser, Neo4jIdentityRole, Neo4jIdentityUserClaim, Neo4jIdentityUserLogin, Neo4jIdentityUserToken, Neo4jIdentityRoleClaim>
        where TUser : Neo4jIdentityUser
    {
        /// <summary>
        /// Constructs a new instance of <see cref="Neo4jUserStore{TUser,TRole}"/>.
        /// </summary>
        /// <param name="session">The <see cref="ISession"/>.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public Neo4jUserStore(
            ISession session,
            ITenantStore tenantStore, 
            IdentityErrorDescriber describer = null) : base(session, tenantStore, describer) { }
    }

    /// <summary>
    /// Creates a new instance of a persistence store for the specified user type.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    public class Neo4jUserStore<TUser, TRole> :
        Neo4jUserStore<TUser, TRole, Neo4jIdentityUserClaim, Neo4jIdentityUserLogin, Neo4jIdentityUserToken, Neo4jIdentityRoleClaim>
        where TUser : Neo4jIdentityUser
        where TRole : Neo4jIdentityRole
    {
        /// <summary>
        /// Constructs a new instance of <see cref="Neo4jUserStore{TUser,TRole}"/>.
        /// </summary>
        /// <param name="session">The <see cref="ISession"/>.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public Neo4jUserStore(
            ISession session, 
            ITenantStore tenantStore,
            IdentityErrorDescriber describer = null) : base(session, tenantStore,describer) { }
    }

    /// <summary>
    /// Represents a new instance of a persistence store for the specified user and role types.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    /// <typeparam name="TRoleClaim">The type representing a role claim.</typeparam>
    public class Neo4jUserStore<TUser, TRole, TUserClaim, TUserLogin, TUserToken, TRoleClaim> :
        Neo4jUserStoreBase<TUser, TRole, TUserClaim, TUserLogin, TUserToken, TRoleClaim>
        where TUser : Neo4jIdentityUser
        where TRole : Neo4jIdentityRole
        where TUserClaim : Neo4jIdentityUserClaim, new()
        where TUserLogin : Neo4jIdentityUserLogin, new()
        where TUserToken : Neo4jIdentityUserToken, new()
        where TRoleClaim : Neo4jIdentityRoleClaim, new()
    {
        private static readonly string User, Role, Claim, Login, Token;
        private static readonly string 
            InRole = Neo4jConstants.Relationships.InRole,
            HasClaim = Neo4jConstants.Relationships.HasClaim,
            HasLogin = Neo4jConstants.Relationships.HasLogin,
            HasToken = Neo4jConstants.Relationships.HasToken;
        static Neo4jUserStore()
        {
            User = typeof(TUser).GetNeo4jLabelName();
            Role = typeof(TRole).GetNeo4jLabelName();
            Claim = typeof(TUserClaim).GetNeo4jLabelName();
            Login = typeof(TUserLogin).GetNeo4jLabelName();
            Token = typeof(TUserToken).GetNeo4jLabelName();
        }

        /// <summary>
        /// Creates a new instance of the store.
        /// </summary>
        /// <param name="session">The context used to access the store.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
        public Neo4jUserStore(
            ISession session, 
            ITenantStore tenantStore,
            IdentityErrorDescriber describer = null) : base(tenantStore,describer ?? new IdentityErrorDescriber())
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }

        /// <summary>
        /// Gets the database context for this store.
        /// </summary>
        public ISession Session { get; }

        /// <summary>
        /// Creates the specified <paramref name="user"/> in the user store.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.</returns>
        public override async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.TenantId = this._tenantStore.TenantId;
            var cypher = $"CREATE (u:{User} $p0)";
            await Session.RunAsync(cypher, Params.Create(user.ConvertToMap()));
            return IdentityResult.Success;
        }

        /// <summary>
        /// Updates the specified <paramref name="user"/> in the user store.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
        public override async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.TenantId = this._tenantStore.TenantId;
            var cypher = $@"
                MATCH (u:{User})
                WHERE u.TenantId = $p0 AND u.Id = $p1
                SET u = $p2";
            await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, user.Id, user.ConvertToMap()));

            return IdentityResult.Success;
        }

        /// <summary>
        /// Deletes the specified <paramref name="user"/> from the user store.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
        public override async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.TenantId = this._tenantStore.TenantId;
            var cypher = $@"
                MATCH (u:{User})
                OPTIONAL MATCH (u)-[:{HasLogin}]->(l:{Login})
                WHERE u.TenantId = $p0 AND u.Id = $p1
                DETACH DELETE u, l";

            await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, user.Id));
            return IdentityResult.Success;
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId"/> if it exists.
        /// </returns>
        public override async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var cypher = $@"
                MATCH (u:{User})
                WHERE u.TenantId = $p0 AND u.Id = $p1
                RETURN u {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, userId));
            var user = await result.SingleOrDefaultAsync(r => r.MapTo<TUser>("u"));
            return user;
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified normalized user name.
        /// </summary>
        /// <param name="normalizedUserName">The normalized user name to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedUserName"/> if it exists.
        /// </returns>
        public override async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var cypher = $@"
                MATCH (u:{User})
                WHERE u.TenantId = $p0 AND u.NormalizedUserName = $p1
                RETURN u {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, normalizedUserName));
            var user = await result.SingleOrDefaultAsync(r => r.MapTo<TUser>("u"));
            return user;
        }

        /// <summary>
        /// Adds the given <paramref name="normalizedRoleName"/> to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to add the role to.</param>
        /// <param name="normalizedRoleName">The role to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override async Task AddToRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));
            }
            user.TenantId = this._tenantStore.TenantId;
            var cypher = $@"
                MATCH (u:{User}), (r:{Role})
                WHERE u.TenantId = $p0 AND u.Id = $p1 AND r.NormalizedName = $p2 AND NOT exists((u)-[:{InRole}]->(r))
                CREATE (u)-[ur:{InRole}]->(r)
                RETURN ur";

            var result = await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, user.Id, normalizedRoleName));
            if (!await result.FetchAsync())
            {
                throw new InvalidOperationException($"Role {normalizedRoleName} does not exist.");
            }
        }

        /// <summary>
        /// Removes the given <paramref name="normalizedRoleName"/> from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the role from.</param>
        /// <param name="normalizedRoleName">The role to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override async Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));
            }
            user.TenantId = this._tenantStore.TenantId;
            var cypher = $@"
                MATCH (u:{User})-[ur:{InRole}]->(r:{Role})
                WHERE u.TenantId = $p0 AND u.Id = $p1 AND r.NormalizedName = $p2
                DELETE ur";
            await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, user.Id, normalizedRoleName));
        }

        /// <summary>
        /// Retrieves the roles the specified <paramref name="user"/> is a member of.
        /// </summary>
        /// <param name="user">The user whose roles should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the roles the user is a member of.</returns>
        public override async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.TenantId = this._tenantStore.TenantId;
            var cypher = $@"
                MATCH (u:{User})-[:{InRole}]->(r:{Role})
                WHERE u.TenantId = $p0 AND u.Id = $p1
                RETURN r.Name as RoleName";

            var result = await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, user.Id));
            var roles = await result.ToListAsync(r => r["RoleName"].As<string>());
            return roles;
        }

        /// <summary>
        /// Returns a flag indicating if the specified user is a member of the give <paramref name="normalizedRoleName"/>.
        /// </summary>
        /// <param name="user">The user whose role membership should be checked.</param>
        /// <param name="normalizedRoleName">The role to check membership of</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing a flag indicating if the specified user is a member of the given group. If the 
        /// user is a member of the group the returned value with be true, otherwise it will be false.</returns>
        public override async Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));
            }
            user.TenantId = this._tenantStore.TenantId;
            var cypher = $@"
                MATCH (u:{User})-[ur:{InRole}]->(r:{Role})
                WHERE u.TenantId = $p0 AND u.Id = $p1 AND r.NormalizedName = $p2
                RETURN ur";

            var result = await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, user.Id, normalizedRoleName));

            var isInRole = await result.ToListAsync();

            return isInRole != null;
        }

        /// <summary>
        /// Get the claims associated with the specified <paramref name="user"/> as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose claims should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the claims granted to a user.</returns>
        public override async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.TenantId = this._tenantStore.TenantId;
            var cypher = $@"
                MATCH (u:{User})-[:{HasClaim}]->(c:{Claim})
                WHERE u.TenantId = $p0 AND u.Id = $p1
                RETURN c {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, user.Id));
            var claims = await result.ToListAsync(r => r.MapTo<TUserClaim>("c").ToClaim());
            return claims;
        }

        /// <summary>
        /// Adds the <paramref name="claims"/> given to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to add the claim to.</param>
        /// <param name="claims">The claim to add to the user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }
            user.TenantId = this._tenantStore.TenantId;
            var cypher = $@"
                MATCH (u:{User} {{TenantId: $p0,Id: $p1}})
                UNWIND $p1 AS claim
                MERGE (c:{Claim} {"claim".AsMapFor<TUserClaim>()})
                MERGE (u)-[:{HasClaim}]->(c)";

            await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, user.Id, claims.Select(c => CreateUserClaim(user, c)).ToList()));
        }

        /// <summary>
        /// Replaces the <paramref name="claim"/> on the specified <paramref name="user"/>, with the <paramref name="newClaim"/>.
        /// </summary>
        /// <param name="user">The user to replace the claim on.</param>
        /// <param name="claim">The claim replace.</param>
        /// <param name="newClaim">The new claim replacing the <paramref name="claim"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            if (newClaim == null)
            {
                throw new ArgumentNullException(nameof(newClaim));
            }
            user.TenantId = this._tenantStore.TenantId;
            var cypher = $@"
                MATCH (u:{User})-[:{HasClaim}]->(c:{Claim})
                WHERE u.TenantId = $p0 AND u.Id = $p1 AND c.ClaimType = $p2.ClaimType AND c.ClaimValue = $p2.ClaimValue
                SET c.ClaimType = $p3.ClaimType, c.ClaimType = $p3.ClaimType";

            await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, user.Id, CreateUserClaim(user, claim), CreateUserClaim(user, newClaim)));
        }

        /// <summary>
        /// Removes the <paramref name="claims"/> given from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the claims from.</param>
        /// <param name="claims">The claim to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }
            user.TenantId = this._tenantStore.TenantId;
            var cypher = $@"
                MATCH (u:{User} {{TenantId: $p0,Id: $p1}})
                UNWIND $p1 AS claim
                MATCH (u)-[uc:{HasClaim}]->(c:{Claim} claim)
                DELETE uc";

            await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, user.Id, claims.Select(c => CreateUserClaim(user, c)).ToList()));
        }

        /// <summary>
        /// Adds the <paramref name="login"/> given to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to add the login to.</param>
        /// <param name="login">The login to add to the user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }
            user.TenantId = this._tenantStore.TenantId;
            var cypher = $@"
                MATCH (u:{User} {{TenantId: $p0,Id: $p1}})
                MERGE (l:Login {"$p2".AsMapFor<TUserLogin>()})
                MERGE (u)-[:{HasLogin}]->(l)";

            await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, user.Id, CreateUserLogin(user, login)));
        }

        /// <summary>
        /// Removes the <paramref name="loginProvider"/> given from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the login from.</param>
        /// <param name="loginProvider">The login to remove from the user.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.TenantId = this._tenantStore.TenantId;
            var cypher = $@"
                MATCH (u:{User})-[:{HasLogin}]->(l:{Login})
                WHERE u.TenantId = $p0 AND u.Id = $p1 AND l.LoginProvider = $p2 AND l.ProviderKey = $p3
                DETACH DELETE l";

            await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, user.Id, loginProvider, providerKey));
        }

        /// <summary>
        /// Retrieves the associated logins for the specified <param ref="user"/>.
        /// </summary>
        /// <param name="user">The user whose associated logins to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing a list of <see cref="UserLoginInfo"/> for the specified <paramref name="user"/>, if any.
        /// </returns>
        public override async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.TenantId = this._tenantStore.TenantId;
            var cypher = $@"
                MATCH (u:{User})-[:{HasLogin}]->(l:{Login})
                WHERE u.TenantId = $p0 AND u.Id = $p1
                RETURN l {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, user.Id));
            var logins = await result.ToListAsync(r =>
            {
                var l = r.MapTo<TUserLogin>("l");
                return new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.ProviderDisplayName);
            });
            return logins;
        }

        /// <summary>
        /// Retrieves the user associated with the specified login provider and login provider key.
        /// </summary>
        /// <param name="loginProvider">The login provider who provided the <paramref name="providerKey"/>.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing the user, if any which matched the specified login provider and key.
        /// </returns>
        public override async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var cypher = $@"
                MATCH (u:{User})-[:{HasLogin}]->(l:{Login})
                WHERE u.TenantId = $p0 AND l.LoginProvider = $p1 AND l.ProviderKey = $p2
                RETURN u {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, loginProvider, providerKey));
            var user = await result.SingleOrDefaultAsync(r => r.MapTo<TUser>("u"));
            return user;
        }

        /// <summary>
        /// Gets the user, if any, associated with the specified, normalized email address.
        /// </summary>
        /// <param name="normalizedEmail">The normalized email address to return the user for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the user if any associated with the specified normalized email address.
        /// </returns>
        public override async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var cypher = $@"
                MATCH (u:{User})
                WHERE u.TenantId = $p0 AND u.NormalizedEmail = $p1
                RETURN u {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, normalizedEmail));
            var user = await result.SingleOrDefaultAsync(r => r.MapTo<TUser>("u"));
            return user;
        }

        /// <summary>
        /// Retrieves all users with the specified claim.
        /// </summary>
        /// <param name="claim">The claim whose users should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> contains a list of users, if any, that contain the specified claim. 
        /// </returns>
        public override async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var cypher = $@"
                MATCH (u:{User})-[:{HasClaim}]->(c:{Claim})
                WHERE u.TenantId = $p0 AND c.ClaimType = $p1 AND c.ClaimValue = $p2
                RETURN u {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, claim.Type, claim.Value));
            var users = await result.ToListAsync(r => r.MapTo<TUser>("u"));
            return users;
        }

        /// <summary>
        /// Retrieves all users in the specified role.
        /// </summary>
        /// <param name="normalizedRoleName">The role whose users should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> contains a list of users, if any, that are in the specified role. 
        /// </returns>
        public override async Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));
            }

            var cypher = $@"
                MATCH (u:{User})-[:{InRole}]->(r:{Role})
                WHERE u.TenantId = $p0 AND r.NormalizedName = $p1
                RETURN u {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, normalizedRoleName));
            var users = await result.ToListAsync(r => r.MapTo<TUser>("u"));
            return users;
        }

        /// <summary>
        /// Sets the token value for a particular user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProvider">The authentication provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="value">The value of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override async Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.TenantId = this._tenantStore.TenantId;
            var cypher = $@"
                MATCH (u:{User} {{TenantId: $p0,Id: $p1}})
                MERGE (t:{Token} {"$p1".AsMapFor<TUserToken>()})
                MERGE (u)-[:{HasToken}]->(t)";

            await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, user.Id, CreateUserToken(user, loginProvider, name, value)));
        }

        /// <summary>
        /// Deletes a token for a user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProvider">The authentication provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override async Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.TenantId = this._tenantStore.TenantId;
            var cypher = $@"
                MATCH (u:{User})-[:{HasToken}]->(t:{Token})
                WHERE u.TenantId = $p0 AND u.Id = $p1 AND t.LoginProvider = $p2 AND t.Name = $p3
                DETACH DELETE t";

            await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, user.Id, loginProvider, name));
        }

        /// <summary>
        /// Returns the token value.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProvider">The authentication provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override async Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var cypher = $@"
                MATCH (u:{User})-[:{HasToken}]->(t:{Token})
                WHERE u.TenantId = $p0 AND u.Id = $p1 AND t.LoginProvider = $p2 AND t.Name = $p3
                RETURN t.Value AS Value";

            var result = await Session.RunAsync(cypher, Params.Create(this._tenantStore.TenantId, user.Id, loginProvider, name));
            var token = await result.SingleOrDefaultAsync(r => r["Value"].As<string>());
            return token;
        }
    }
}
