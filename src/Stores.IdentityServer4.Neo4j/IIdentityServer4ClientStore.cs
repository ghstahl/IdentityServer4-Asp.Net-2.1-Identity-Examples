using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4Extras;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras;
using Neo4jExtras.Extensions;
using Stores.IdentityServer4.Neo4j.Entities;

namespace Stores.IdentityServer4.Neo4j
{
    public interface IIdentityServer4GrantTypeStore<TGrantType> :
        IDisposable
        where TGrantType : ClientGrantType
    {
        Task<IdentityResult> CreateAsync(TGrantType grantType,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> UpdateAsync(TGrantType originalGrantType, TGrantType grantType,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteAsync(TGrantType grantType,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<Neo4jIdentityServer4ClientGrantType> FindGrantTypeAsync(string grantType,
            CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IIdentityServer4ClientStore<TClient, TGrantType> :
        IIdentityServer4GrantTypeStore<TGrantType>,
        IDisposable
        where TClient : ClientRoot
        where TGrantType : ClientGrantType
    {
        Task<IdentityResult> CreateAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> UpdateAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<Neo4jIdentityServer4Client> FindClientByClientIdAsync(string clientId,
            CancellationToken cancellationToken = default(CancellationToken));
        
        Task<IdentityResult> AddSecretToClientAsync(TClient client,
            Neo4jIdentityServer4ClientSecret secret,
            CancellationToken cancellationToken = default(CancellationToken));
      

        Task<IdentityResult> AddAllowedGrantTypeToClientAsync(TClient client,
            TGrantType grantType,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TGrantType>> GetAllowedGrantTypesAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IIdentityServer4ClientUserStore<TUser, TClient, TGrantType> :
        IIdentityServer4ClientStore<TClient, TGrantType>,
        IDisposable
        where TUser : class
        where TClient : ClientRoot
        where TGrantType : ClientGrantType

    {
        Task CreateConstraintsAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> AddToClientAsync(TUser user, TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TClient>> GetClientsAsync(TUser user,
            CancellationToken cancellationToken = default(CancellationToken));
    }

    public class Neo4jIdentityServer4ClientUserStore<TUser> :
        IIdentityServer4ClientUserStore<TUser, Neo4jIdentityServer4Client, Neo4jIdentityServer4ClientGrantType>
        where TUser : Neo4jIdentityUser
    {
        private bool _disposed;

        /// <summary>
        /// Dispose the stores
        /// </summary>
        public void Dispose() => _disposed = true;

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public static string User { get; set; }

        /// <summary>
        /// Gets the database session for this store.
        /// </summary>
        public ISession Session { get; }

        private static readonly string IdentityServer4Client;
        private static readonly string IdentityServer4ClientSecret;
        private static readonly string IdentityServer4ClientGrantType;

        static Neo4jIdentityServer4ClientUserStore()
        {
            User = typeof(TUser).GetNeo4jLabelName();
            IdentityServer4Client = typeof(Neo4jIdentityServer4Client).GetNeo4jLabelName();
            IdentityServer4ClientSecret = typeof(Neo4jIdentityServer4ClientSecret).GetNeo4jLabelName();
            IdentityServer4ClientGrantType = typeof(Neo4jIdentityServer4ClientGrantType).GetNeo4jLabelName();
        }

        public async Task CreateConstraintsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var cypher = $@"CREATE CONSTRAINT ON (r:{IdentityServer4Client}) ASSERT r.ClientId IS UNIQUE";
            await Session.RunAsync(cypher);
            cypher = $@"CREATE CONSTRAINT ON (r:{IdentityServer4ClientGrantType}) ASSERT r.GrantType IS UNIQUE";
            await Session.RunAsync(cypher);
        }

        public Neo4jIdentityServer4ClientUserStore(ISession session)
        {
            Session = session;
        }

        public async Task<IdentityResult> CreateAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            try
            {
                var cypher = $@"CREATE (r:{IdentityServer4Client} $p0)";
                await Session.RunAsync(cypher, Params.Create(client.ConvertToMap()));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError() {Code = ex.Code, Description = ex.Message}
                });
            }
        }
        
        public async Task<IdentityResult> UpdateAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            try
            {
                var cypher = $@"
                MATCH (r:{IdentityServer4Client})
                WHERE r.ClientId = $p0
                SET r = $p1";

                await Session.RunAsync(cypher, Params.Create(client.ClientId, client.ConvertToMap()));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError() {Code = ex.Code, Description = ex.Message}
                });
            }
        }

        public async Task<IdentityResult> DeleteAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            try
            {
                var cypher = $@"
                MATCH (r:{IdentityServer4Client})
                WHERE r.ClientId = $p0
                DETACH DELETE r";

                await Session.RunAsync(cypher, Params.Create(client.ClientId));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError() {Code = ex.Code, Description = ex.Message}
                });
            }
        }
        public async Task<Neo4jIdentityServer4Client> FindClientByClientIdAsync(string clientId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            clientId.ThrowIfNull(nameof(clientId));

            var cypher = $@"
                MATCH (r:{IdentityServer4Client})
                WHERE r.ClientId = $p0
                RETURN r {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(clientId));
            var factor = await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4Client>("r"));
            return factor;
        }

        public async Task<IdentityResult> AddSecretToClientAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientSecret secret,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            secret.ThrowIfNull(nameof(secret));
            try
            {
                var cypher = $@"
                MATCH (u:{IdentityServer4Client} {{ClientId: $p0}})
                MERGE (l:{IdentityServer4ClientSecret} {"$p1".AsMapFor<Neo4jIdentityServer4ClientSecret>()})
                MERGE (u)-[:{Neo4jConstants.Relationships.HasSecret}]->(l)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, secret));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError() {Code = ex.Code, Description = ex.Message}
                });
            }
        }

        public async Task<IdentityResult> AddAllowedGrantTypeToClientAsync(
            Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientGrantType grantType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            grantType.ThrowIfNull(nameof(grantType));
            try
            {
                var cypher = $@"
                MATCH (u:{IdentityServer4Client} {{ClientId: $p0}})
                MERGE (l:{IdentityServer4ClientGrantType} {"$p1".AsMapFor<Neo4jIdentityServer4ClientGrantType>()})
                MERGE (u)-[:{Neo4jConstants.Relationships.HasGrantType}]->(l)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, grantType));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError() {Code = ex.Code, Description = ex.Message}
                });
            }
        }

        public async Task<IList<Neo4jIdentityServer4ClientGrantType>> GetAllowedGrantTypesAsync(
            Neo4jIdentityServer4Client client, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                MATCH (c:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasGrantType}]->(g:{
                    IdentityServer4ClientGrantType
                })
                WHERE c.ClientId = $p0
                RETURN g{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var grantTypes = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientGrantType>("g"));
            return grantTypes;

        }



        



        public async Task<IdentityResult> AddToClientAsync(TUser user, Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            client.ThrowIfNull(nameof(client));
            try
            {
                var cypher = $@"
                MATCH (u:{User} {{Id: $p0}})
                MERGE (l:{IdentityServer4Client} {"$p1".AsMapFor<Neo4jIdentityServer4Client>()})
                MERGE (u)-[:{Neo4jConstants.Relationships.HasClient}]->(l)";

                var result = await Session.RunAsync(cypher, Params.Create(user.Id, client));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError() {Code = ex.Code, Description = ex.Message}
                });
            }
        }

        public async Task<IList<Neo4jIdentityServer4Client>> GetClientsAsync(TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));


            var cypher = $@"
                MATCH (u:{User})-[:{Neo4jConstants.Relationships.HasClient}]->(r:{IdentityServer4Client})
                WHERE u.Id = $p0
                RETURN r{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(user.Id));

            var factors = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4Client>("r"));
            return factors;
        }


        public async Task<IdentityResult> CreateAsync(Neo4jIdentityServer4ClientGrantType grantType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            grantType.ThrowIfNull(nameof(grantType));
            try
            {
                var cypher = $@"CREATE (r:{IdentityServer4ClientGrantType} $p0)";
                await Session.RunAsync(cypher, Params.Create(grantType.ConvertToMap()));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError() {Code = ex.Code, Description = ex.Message}
                });
            }
        }

        public async Task<IdentityResult> UpdateAsync(
            Neo4jIdentityServer4ClientGrantType originalGrantType, 
            Neo4jIdentityServer4ClientGrantType grantType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            grantType.ThrowIfNull(nameof(grantType));
            try
            {
                var cypher = $@"
                MATCH (r:{IdentityServer4ClientGrantType})
                WHERE r.GrantType = $p0
                SET r = $p1";

                await Session.RunAsync(cypher, Params.Create(originalGrantType.GrantType, grantType.ConvertToMap()));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError() {Code = ex.Code, Description = ex.Message}
                });
            }
        }

        public async Task<IdentityResult> DeleteAsync(Neo4jIdentityServer4ClientGrantType grantType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            grantType.ThrowIfNull(nameof(grantType));
            try
            {
                var cypher = $@"
                MATCH (r:{IdentityServer4ClientGrantType})
                WHERE r.GrantType = $p0
                DETACH DELETE r";

                await Session.RunAsync(cypher, Params.Create(grantType.GrantType));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError() {Code = ex.Code, Description = ex.Message}
                });
            }
        }

        public async Task<Neo4jIdentityServer4ClientGrantType> FindGrantTypeAsync(string grantType, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            grantType.ThrowIfNull(nameof(grantType));

            var cypher = $@"
                MATCH (r:{IdentityServer4ClientGrantType})
                WHERE r.GrantType = $p0
                RETURN r {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(grantType));
            var grantTypeRecord = await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ClientGrantType>("r"));
            return grantTypeRecord;
        }
    }
}