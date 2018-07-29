using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4.Models;
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

        Task<TGrantType> FindGrantTypeAsync(string grantType,
            CancellationToken cancellationToken = default(CancellationToken));
    }



    public interface IIdentityServer4ClientStore<
        TClient,
        TSecret,
        TGrantType,
        TClaim,
        TCorsOrigin,
        TScope,
        TIdPRestriction,
        TProperty,
        TPostLogoutRedirectUri,
        TRedirectUri> :
        IIdentityServer4GrantTypeStore<TGrantType>,
        IDisposable
        where TClient : ClientRoot
        where TSecret : Secret
        where TGrantType : ClientGrantType
        where TClaim : ClientClaim
        where TCorsOrigin : ClientCorsOrigin
        where TScope : ClientScope
        where TIdPRestriction : ClientIdPRestriction
        where TProperty : ClientProperty
        where TPostLogoutRedirectUri : ClientPostLogoutRedirectUri
        where TRedirectUri : ClientRedirectUri

    {
        Task<IdentityResult> CreateClientAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> UpdateAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<Neo4jIdentityServer4Client> FindClientByClientIdAsync(string clientId,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> AddSecretToClientAsync(TClient client,
            TSecret secret,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> UpdateSecretAsync(TClient client, TSecret secret,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteSecretAsync(TClient client, TSecret secret,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<TSecret> FindSecretAsync(TClient client, TSecret secret,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TSecret>> GetSecretsAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));



        Task<IdentityResult> AddAllowedGrantTypeToClientAsync(TClient client,
            TGrantType grantType,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TGrantType>> GetAllowedGrantTypesAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IIdentityServer4ClientUserStore<
        TUser, 
        TClient,
        TSecret,
        TGrantType,
        TClaim,
        TCorsOrigin,
        TScope,
        TIdPRestriction,
        TProperty,
        TPostLogoutRedirectUri,
        TRedirectUri> :
        IIdentityServer4ClientStore<
            TClient,
            TSecret,
            TGrantType,
            TClaim,
            TCorsOrigin,
            TScope,
            TIdPRestriction,
            TProperty,
            TPostLogoutRedirectUri,
            TRedirectUri>,
        IDisposable
        where TUser : class
        where TClient : ClientRoot
        where TSecret : Secret
        where TGrantType : ClientGrantType
        where TClaim : ClientClaim
        where TCorsOrigin : ClientCorsOrigin
        where TScope : ClientScope
        where TIdPRestriction : ClientIdPRestriction
        where TProperty : ClientProperty
        where TPostLogoutRedirectUri : ClientPostLogoutRedirectUri
        where TRedirectUri : ClientRedirectUri

    {
        Task CreateConstraintsAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> AddToClientAsync(TUser user, TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TClient>> GetClientsAsync(TUser user,
            CancellationToken cancellationToken = default(CancellationToken));
    }

    public class Neo4jIdentityServer4ClientUserStore<TUser> :
        IIdentityServer4ClientUserStore<
            TUser,
            Neo4jIdentityServer4Client,
            Neo4jIdentityServer4ClientSecret,
            Neo4jIdentityServer4ClientGrantType,
            Neo4jIdentityServer4ClientClaim,
            Neo4jIdentityServer4ClientCorsOrigin,
            Neo4jIdentityServer4ClientScope,
            Neo4jIdentityServer4ClientIdPRestriction,
            Neo4jIdentityServer4ClientProperty,
            Neo4jIdentityServer4ClientPostLogoutRedirectUri,
            Neo4jIdentityServer4ClientRedirectUri>
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
        private static readonly string IdentityServer4ClientClaim;
        private static readonly string IdentityServer4ClientCorsOrigin;
        private static readonly string IdentityServer4ClientScope;
        private static readonly string IdentityServer4ClientIdPRestriction;
        private static readonly string IdentityServer4ClientProperty;
        private static readonly string IdentityServer4ClientPostLogoutRedirectUri;
        private static readonly string IdentityServer4ClientRedirectUri;


        static Neo4jIdentityServer4ClientUserStore()
        {
            User = typeof(TUser).GetNeo4jLabelName();
            IdentityServer4Client = typeof(Neo4jIdentityServer4Client).GetNeo4jLabelName();
            IdentityServer4ClientSecret = typeof(Neo4jIdentityServer4ClientSecret).GetNeo4jLabelName();
            IdentityServer4ClientGrantType = typeof(Neo4jIdentityServer4ClientGrantType).GetNeo4jLabelName();
            IdentityServer4ClientClaim = typeof(Neo4jIdentityServer4ClientClaim).GetNeo4jLabelName();
            IdentityServer4ClientCorsOrigin = typeof(Neo4jIdentityServer4ClientCorsOrigin).GetNeo4jLabelName();
            IdentityServer4ClientScope = typeof(Neo4jIdentityServer4ClientScope).GetNeo4jLabelName();
            IdentityServer4ClientIdPRestriction = typeof(Neo4jIdentityServer4ClientIdPRestriction).GetNeo4jLabelName();
            IdentityServer4ClientProperty = typeof(Neo4jIdentityServer4ClientProperty).GetNeo4jLabelName();
            IdentityServer4ClientPostLogoutRedirectUri = typeof(Neo4jIdentityServer4ClientPostLogoutRedirectUri).GetNeo4jLabelName();
            IdentityServer4ClientRedirectUri = typeof(Neo4jIdentityServer4ClientRedirectUri).GetNeo4jLabelName();
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

        public async Task<IdentityResult> CreateClientAsync(Neo4jIdentityServer4Client client,
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
                MATCH (c:{IdentityServer4Client})
                OPTIONAL MATCH (c)-[:{Neo4jConstants.Relationships.HasSecret}]->(s:{IdentityServer4ClientSecret})
                WHERE c.ClientId = $p0
                DETACH DELETE s,c";

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
                MATCH (c:{IdentityServer4Client})
                WHERE c.ClientId = $p0
                RETURN c {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(clientId));
            var factor = await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4Client>("c"));
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
                MATCH (c:{IdentityServer4Client} {{ClientId: $p0}})
                MERGE (s:{IdentityServer4ClientSecret} {"$p1".AsMapFor<Neo4jIdentityServer4ClientSecret>()})
                MERGE (c)-[:{Neo4jConstants.Relationships.HasSecret}]->(s)";

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

        public async Task<IdentityResult> UpdateSecretAsync(
            Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientSecret secret,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            try
            {
                var cypher = $@"
                MATCH (c:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasSecret}]->(s:{
                        IdentityServer4ClientSecret
                    })
                WHERE c.ClientId = $p0 AND s.Value = $p1
                SET s = $p2";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId,
                        secret.Value,
                        secret.ConvertToMap()));
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

        public async Task<IdentityResult> DeleteSecretAsync(
            Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientSecret secret,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            try
            {
                var cypher = $@"
                MATCH (c:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasSecret}]->(s:{
                        IdentityServer4ClientSecret
                    })
                WHERE c.ClientId = $p0 AND s.Value = $p1
                DETACH DELETE s";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId,
                        secret.Value));
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

        public async Task<Neo4jIdentityServer4ClientSecret> FindSecretAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientSecret secret,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                MATCH (c:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasSecret}]->(s:{
                    IdentityServer4ClientSecret
                })
                WHERE c.ClientId = $p0 AND s.Value = $p1
                RETURN s{{ .* }}";

            var result = await Session.RunAsync(cypher,
                Params.Create(
                    client.ClientId,
                    secret.Value));

            var neo4jIdentityServer4ClientSecret =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ClientSecret>("s"));

            return neo4jIdentityServer4ClientSecret;
        }

        public async Task<IList<Neo4jIdentityServer4ClientSecret>> GetSecretsAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                MATCH (c:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasSecret}]->(s:{
                    IdentityServer4ClientSecret
                })
                WHERE c.ClientId = $p0
                RETURN s{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var secrets = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientSecret>("s"));
            return secrets;
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

            var clientsResult = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4Client>("r"));
            return clientsResult;
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

        public async Task<Neo4jIdentityServer4ClientGrantType> FindGrantTypeAsync(string grantType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            grantType.ThrowIfNull(nameof(grantType));

            var cypher = $@"
                MATCH (r:{IdentityServer4ClientGrantType})
                WHERE r.GrantType = $p0
                RETURN r {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(grantType));
            var grantTypeRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ClientGrantType>("r"));
            return grantTypeRecord;
        }




    }
}