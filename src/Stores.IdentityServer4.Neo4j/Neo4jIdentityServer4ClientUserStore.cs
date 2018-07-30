using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras;
using Neo4jExtras.Extensions;

namespace Stores.IdentityServer4.Neo4j
{
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

        public async Task<IdentityResult> DeleteClientAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            await DeleteRedirectUrisAsync(client);
            await DeletePostLogoutRedirectUrisAsync(client);
            await DeleteClaimsAsync(client);
            await DeleteCorsOriginsAsync(client);
            await DeleteIdPRestrictionsAsync(client);
            await DeletePropertiesAsync(client);
            await DeleteScopesAsync(client);
            await DeleteSecretsAsync(client);

            try
            {
                var cypher = $@"
                MATCH (c:{IdentityServer4Client})
                WHERE c.ClientId = $p0
                DETACH DELETE c";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));
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
            secret.ThrowIfNull(nameof(secret));
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
            secret.ThrowIfNull(nameof(secret));
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

        public async Task<IdentityResult> DeleteSecretsAsync(Neo4jIdentityServer4Client client,
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
                WHERE c.ClientId = $p0 
                DETACH DELETE s";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId));
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
            secret.ThrowIfNull(nameof(secret));

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

            var foundRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ClientSecret>("s"));

            return foundRecord;
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

        public async Task<IdentityResult> AddClaimToClientAsync(Neo4jIdentityServer4Client client, 
            Neo4jIdentityServer4ClientClaim claim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            claim.ThrowIfNull(nameof(claim));
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client} {{ClientId: $p0}})
                MERGE (claim:{IdentityServer4ClientClaim} {"$p1".AsMapFor<Neo4jIdentityServer4ClientClaim>()})
                MERGE (client)-[:{Neo4jConstants.Relationships.HasClaim}]->(claim)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, claim));
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


        public async Task<IdentityResult> DeleteClaimAsync(Neo4jIdentityServer4Client client, 
            Neo4jIdentityServer4ClientClaim claim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            claim.ThrowIfNull(nameof(claim));
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasClaim}]->(claim:{
                        IdentityServer4ClientClaim
                    })
                WHERE client.ClientId = $p0 AND claim.Type = $p1 AND claim.Value = $p2
                DETACH DELETE claim";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId,
                        claim.Type,
                        claim.Value
                        ));
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

        public async Task<IdentityResult> DeleteClaimsAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
          
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasClaim}]->(claim:{
                        IdentityServer4ClientClaim
                    })
                WHERE client.ClientId = $p0  
                DETACH DELETE claim";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId
                    ));
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

        public async Task<Neo4jIdentityServer4ClientClaim> FindClaimAsync(Neo4jIdentityServer4Client client, 
            Neo4jIdentityServer4ClientClaim claim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            claim.ThrowIfNull(nameof(claim));
            var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasClaim}]->(claim:{
                    IdentityServer4ClientClaim
                })
                WHERE client.ClientId = $p0 AND claim.Type = $p1 AND claim.Value = $p2
                RETURN claim{{ .* }}";

            var result = await Session.RunAsync(cypher,
                Params.Create(
                    client.ClientId,
                    claim.Type,
                    claim.Value
                ));

            var foundRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ClientClaim>("claim"));

            return foundRecord;
        }

        public async Task<IList<Neo4jIdentityServer4ClientClaim>> GetClaimsAsync(
            Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                 MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasClaim}]->(claim:{
                    IdentityServer4ClientClaim
                })
                WHERE client.ClientId = $p0
                RETURN claim{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var claims = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientClaim>("claim"));
            return claims;
        }

        public async Task<IdentityResult> AddCorsOriginToClientAsync(Neo4jIdentityServer4Client client, 
            Neo4jIdentityServer4ClientCorsOrigin corsOrigin,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            corsOrigin.ThrowIfNull(nameof(corsOrigin));
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client} {{ClientId: $p0}})
                MERGE (corsOrigin:{IdentityServer4ClientCorsOrigin} {"$p1".AsMapFor<Neo4jIdentityServer4ClientCorsOrigin>()})
                MERGE (client)-[:{Neo4jConstants.Relationships.HasCorsOrigin}]->(corsOrigin)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, corsOrigin));
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

        public async Task<IdentityResult> DeleteCorsOriginAsync(Neo4jIdentityServer4Client client, Neo4jIdentityServer4ClientCorsOrigin corsOrigin,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            corsOrigin.ThrowIfNull(nameof(corsOrigin));
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasCorsOrigin}]->(corsOrigin:{
                        IdentityServer4ClientCorsOrigin
                    })
                WHERE client.ClientId = $p0 AND corsOrigin.Origin = $p1 
                DETACH DELETE corsOrigin";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId,
                        corsOrigin.Origin
                    ));
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

        public async Task<IdentityResult> DeleteCorsOriginsAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
          
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasCorsOrigin}]->(corsOrigin:{
                        IdentityServer4ClientCorsOrigin
                    })
                WHERE client.ClientId = $p0 
                DETACH DELETE corsOrigin";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId
                    ));
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

        public async Task<Neo4jIdentityServer4ClientCorsOrigin> FindCorsOriginAsync(Neo4jIdentityServer4Client client, 
            Neo4jIdentityServer4ClientCorsOrigin corsOrigin,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            corsOrigin.ThrowIfNull(nameof(corsOrigin));
            var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasCorsOrigin}]->(corsOrigin:{
                    IdentityServer4ClientCorsOrigin
                })
                WHERE client.ClientId = $p0 AND corsOrigin.Origin = $p1 
                RETURN corsOrigin{{ .* }}";

            var result = await Session.RunAsync(cypher,
                Params.Create(
                    client.ClientId,
                    corsOrigin.Origin
                ));

            var foundRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ClientCorsOrigin>("corsOrigin"));

            return foundRecord;
        }

        public async Task<IList<Neo4jIdentityServer4ClientCorsOrigin>> GetCorsOriginsAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                 MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasCorsOrigin}]->(corsOrigin:{
                    IdentityServer4ClientCorsOrigin
                })
                WHERE client.ClientId = $p0
                RETURN corsOrigin{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var corsOrigins = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientCorsOrigin>("corsOrigin"));
            return corsOrigins;
        }

        public async Task<IdentityResult> AddScopeToClientAsync(Neo4jIdentityServer4Client client, Neo4jIdentityServer4ClientScope scope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            scope.ThrowIfNull(nameof(scope));
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client} {{ClientId: $p0}})
                MERGE (scope:{IdentityServer4ClientScope} {"$p1".AsMapFor<Neo4jIdentityServer4ClientScope>()})
                MERGE (client)-[:{Neo4jConstants.Relationships.HasScope}]->(scope)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, scope));
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

        public async Task<IdentityResult> DeleteScopeAsync(Neo4jIdentityServer4Client client, Neo4jIdentityServer4ClientScope scope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            scope.ThrowIfNull(nameof(scope));
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasScope}]->(scope:{
                        IdentityServer4ClientScope
                    })
                WHERE client.ClientId = $p0 AND scope.Scope = $p1 
                DETACH DELETE scope";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId,
                        scope.Scope
                    ));
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

        public async  Task<IdentityResult> DeleteScopesAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasScope}]->(scope:{
                        IdentityServer4ClientScope
                    })
                WHERE client.ClientId = $p0
                DETACH DELETE scope";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId
                    ));
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

        public async Task<Neo4jIdentityServer4ClientScope> FindScopeAsync(Neo4jIdentityServer4Client client, Neo4jIdentityServer4ClientScope scope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            scope.ThrowIfNull(nameof(scope));
            var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasScope}]->(scope:{
                    IdentityServer4ClientScope
                })
                WHERE client.ClientId = $p0 AND scope.Scope = $p1  
                RETURN scope{{ .* }}";

            var result = await Session.RunAsync(cypher,
                Params.Create(
                    client.ClientId,
                    scope.Scope
                ));

            var foundRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ClientScope>("scope"));

            return foundRecord;
        }

        public async Task<IList<Neo4jIdentityServer4ClientScope>> GetScopesAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                 MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasScope}]->(scope:{
                    IdentityServer4ClientScope
                })
                WHERE client.ClientId = $p0
                RETURN scope{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var scopes = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientScope>("scope"));
            return scopes;
        }

        public async Task<IdentityResult> AddIdPRestrictionToClientAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientIdPRestriction idPRestriction,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            idPRestriction.ThrowIfNull(nameof(idPRestriction));
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client} {{ClientId: $p0}})
                MERGE (idp:{IdentityServer4ClientIdPRestriction} {"$p1".AsMapFor<Neo4jIdentityServer4ClientIdPRestriction>()})
                MERGE (client)-[:{Neo4jConstants.Relationships.HasIdPRestriction}]->(idp)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, idPRestriction));
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

        public async Task<IdentityResult> DeleteIdPRestrictionAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientIdPRestriction idPRestriction,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            idPRestriction.ThrowIfNull(nameof(idPRestriction));
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasIdPRestriction}]->(idp:{
                        IdentityServer4ClientIdPRestriction
                    })
                WHERE client.ClientId = $p0 AND idp.Provider = $p1 
                DETACH DELETE idp";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId,
                        idPRestriction.Provider
                    ));
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

        public async Task<IdentityResult> DeleteIdPRestrictionsAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasIdPRestriction}]->(idp:{
                        IdentityServer4ClientIdPRestriction
                    })
                WHERE client.ClientId = $p0 
                DETACH DELETE idp";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId
                    ));
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

        public async Task<Neo4jIdentityServer4ClientIdPRestriction> FindIdPRestrictionAsync(Neo4jIdentityServer4Client client, 
            Neo4jIdentityServer4ClientIdPRestriction idPRestriction,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            idPRestriction.ThrowIfNull(nameof(idPRestriction));
            var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasIdPRestriction}]->(idp:{
                    IdentityServer4ClientIdPRestriction
                })
                WHERE client.ClientId = $p0 AND idp.Provider = $p1  
                RETURN idp{{ .* }}";

            var result = await Session.RunAsync(cypher,
                Params.Create(
                    client.ClientId,
                    idPRestriction.Provider
                ));

            var foundRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ClientIdPRestriction>("idp"));

            return foundRecord;
        }

        public async Task<IList<Neo4jIdentityServer4ClientIdPRestriction>> GetIdPRestrictionsAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                 MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasIdPRestriction}]->(idp:{
                    IdentityServer4ClientIdPRestriction
                })
                WHERE client.ClientId = $p0
                RETURN idp{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var ipds = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientIdPRestriction>("idp"));
            return ipds;
        }

        public async Task<IdentityResult> AddPropertyToClientAsync(Neo4jIdentityServer4Client client, 
            Neo4jIdentityServer4ClientProperty property,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            property.ThrowIfNull(nameof(property));
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client} {{ClientId: $p0}})
                MERGE (property:{IdentityServer4ClientProperty} {"$p1".AsMapFor<Neo4jIdentityServer4ClientProperty>()})
                MERGE (client)-[:{Neo4jConstants.Relationships.HasProperty}]->(property)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, property));
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


        public async Task<IdentityResult> DeletePropertyAsync(Neo4jIdentityServer4Client client, Neo4jIdentityServer4ClientProperty property,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            property.ThrowIfNull(nameof(property));
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasProperty}]->(property:{
                        IdentityServer4ClientProperty
                    })
                WHERE client.ClientId = $p0 AND property.Key = $p1 AND property.Value = $p2 
                DETACH DELETE property";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId,
                        property.Key,
                        property.Value
                    ));
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

        public async Task<IdentityResult> DeletePropertiesAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
          
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasProperty}]->(property:{
                        IdentityServer4ClientProperty
                    })
                WHERE client.ClientId = $p0  
                DETACH DELETE property";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId
                    ));
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

        public async Task<Neo4jIdentityServer4ClientProperty> FindPropertyAsync(Neo4jIdentityServer4Client client, 
            Neo4jIdentityServer4ClientProperty property,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            property.ThrowIfNull(nameof(property));
            var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasProperty}]->(property:{
                    IdentityServer4ClientProperty
                })
                WHERE client.ClientId = $p0 AND property.Key = $p1 AND property.Value = $p2  
                RETURN property{{ .* }}";

            var result = await Session.RunAsync(cypher,
                Params.Create(
                    client.ClientId,
                    property.Key,
                    property.Value

                ));

            var foundRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ClientProperty>("property"));

            return foundRecord;
        }

        public async Task<IList<Neo4jIdentityServer4ClientProperty>> GetPropertiesAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                 MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasProperty}]->(property:{
                    IdentityServer4ClientProperty
                })
                WHERE client.ClientId = $p0
                RETURN property{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var records = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientProperty>("property"));
            return records;
        }

        public async Task<IdentityResult> AddPostLogoutRedirectUriToClientAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientPostLogoutRedirectUri postLogoutRedirectUri,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            postLogoutRedirectUri.ThrowIfNull(nameof(postLogoutRedirectUri));
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client} {{ClientId: $p0}})
                MERGE (postLogoutRedirectUri:{IdentityServer4ClientPostLogoutRedirectUri} {"$p1".AsMapFor<Neo4jIdentityServer4ClientPostLogoutRedirectUri>()})
                MERGE (client)-[:{Neo4jConstants.Relationships.HasPostLogoutRedirectUri}]->(postLogoutRedirectUri)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, postLogoutRedirectUri));
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

        public async Task<IdentityResult> DeletePostLogoutRedirectUriAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientPostLogoutRedirectUri postLogoutRedirectUri,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            postLogoutRedirectUri.ThrowIfNull(nameof(postLogoutRedirectUri));
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasPostLogoutRedirectUri}]->(postLogoutRedirectUri:{
                        IdentityServer4ClientPostLogoutRedirectUri
                    })
                WHERE client.ClientId = $p0 AND postLogoutRedirectUri.PostLogoutRedirectUri = $p1  
                DETACH DELETE postLogoutRedirectUri";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId,
                        postLogoutRedirectUri.PostLogoutRedirectUri
                    ));
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

        public async Task<IdentityResult> DeletePostLogoutRedirectUrisAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasPostLogoutRedirectUri}]->(p:{
                        IdentityServer4ClientPostLogoutRedirectUri
                    })
                WHERE client.ClientId = $p0  
                DETACH DELETE p";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId
                    ));
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

        public async Task<Neo4jIdentityServer4ClientPostLogoutRedirectUri> FindPostLogoutRedirectUriAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientPostLogoutRedirectUri postLogoutRedirectUri,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            postLogoutRedirectUri.ThrowIfNull(nameof(postLogoutRedirectUri));
            var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasPostLogoutRedirectUri}]->(postLogoutRedirectUri:{
                    IdentityServer4ClientPostLogoutRedirectUri
                })
                WHERE client.ClientId = $p0 AND postLogoutRedirectUri.PostLogoutRedirectUri = $p1  
                RETURN postLogoutRedirectUri{{ .* }}";

            var result = await Session.RunAsync(cypher,
                Params.Create(
                    client.ClientId,
                    postLogoutRedirectUri.PostLogoutRedirectUri

                ));

            var foundRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ClientPostLogoutRedirectUri>("postLogoutRedirectUri"));

            return foundRecord;
        }

        public async Task<IList<Neo4jIdentityServer4ClientPostLogoutRedirectUri>> GetPostLogoutRedirectUrisAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                 MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasPostLogoutRedirectUri}]->(post:{
                    IdentityServer4ClientPostLogoutRedirectUri
                })
                WHERE client.ClientId = $p0
                RETURN post{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var records = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientPostLogoutRedirectUri>("post"));
            return records;
        }

        public async Task<IdentityResult> AddRedirectUriToClientAsync(Neo4jIdentityServer4Client client, 
            Neo4jIdentityServer4ClientRedirectUri redirectUri,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            redirectUri.ThrowIfNull(nameof(redirectUri));
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client} {{ClientId: $p0}})
                MERGE (redirectUri:{IdentityServer4ClientRedirectUri} {"$p1".AsMapFor<Neo4jIdentityServer4ClientRedirectUri>()})
                MERGE (client)-[:{Neo4jConstants.Relationships.HasRedirectUri}]->(redirectUri)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, redirectUri));
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

        public async Task<IdentityResult> DeleteRedirectUriAsync(Neo4jIdentityServer4Client client, 
            Neo4jIdentityServer4ClientRedirectUri redirectUri,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            redirectUri.ThrowIfNull(nameof(redirectUri));
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasRedirectUri}]->(redirectUri:{
                        IdentityServer4ClientRedirectUri
                    })
                WHERE client.ClientId = $p0 AND redirectUri.RedirectUri = $p1  
                DETACH DELETE redirectUri";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId,
                        redirectUri.RedirectUri
                    ));
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

        public async Task<IdentityResult> DeleteRedirectUrisAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
       
            try
            {
                var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasRedirectUri}]->(redirectUri:{
                        IdentityServer4ClientRedirectUri
                    })
                WHERE client.ClientId = $p0  
                DETACH DELETE redirectUri";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId
                    ));
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

        public async Task<Neo4jIdentityServer4ClientRedirectUri> FindRedirectUriAsync(Neo4jIdentityServer4Client client, 
            Neo4jIdentityServer4ClientRedirectUri redirectUri,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            redirectUri.ThrowIfNull(nameof(redirectUri));
            var cypher = $@"
                MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasRedirectUri}]->(redirectUri:{
                    IdentityServer4ClientRedirectUri
                })
                WHERE client.ClientId = $p0 AND redirectUri.RedirectUri = $p1  
                RETURN redirectUri{{ .* }}";

            var result = await Session.RunAsync(cypher,
                Params.Create(
                    client.ClientId,
                    redirectUri.RedirectUri

                ));

            var foundRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ClientRedirectUri>("redirectUri"));

            return foundRecord;
        }

        public async Task<IList<Neo4jIdentityServer4ClientRedirectUri>> GetRedirectUrisAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                 MATCH (client:{IdentityServer4Client})-[:{Neo4jConstants.Relationships.HasRedirectUri}]->(red:{
                    IdentityServer4ClientRedirectUri
                })
                WHERE client.ClientId = $p0
                RETURN red{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var records = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientRedirectUri>("red"));
            return records;
        }


        public async Task<IdentityResult> AddClientToUserAsync(TUser user, Neo4jIdentityServer4Client client,
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


        public async Task<IdentityResult> CreateGrantTypeAsync(Neo4jIdentityServer4ClientGrantType grantType,
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

        public async Task<IdentityResult> UpdateGrantTypeAsync(
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

        public async Task<IdentityResult> DeleteGrantTypeAsync(Neo4jIdentityServer4ClientGrantType grantType,
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

        public async Task<IdentityResult> DeleteGrantTypesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
         
            try
            {
                var cypher = $@"
                MATCH (r:{IdentityServer4ClientGrantType})
                DETACH DELETE r";

                await Session.RunAsync(cypher);
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