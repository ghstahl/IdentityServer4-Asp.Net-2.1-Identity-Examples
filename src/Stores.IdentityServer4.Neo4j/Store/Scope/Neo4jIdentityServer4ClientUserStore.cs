using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras;
using Neo4jExtras.Extensions;

namespace Stores.IdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStore<TUser> where TUser : Neo4jIdentityUser
    {
        private static readonly string IdSrv4ClientScope;

        public async Task<IdentityResult> AddScopeToClientAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientScope scope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            scope.ThrowIfNull(nameof(scope));
            try
            {
                var cypher = $@"
                MATCH (client:{IdSrv4Client} {{ClientId: $p0}})
                MERGE (scope:{IdSrv4ClientScope} {"$p1".AsMapFor<Neo4jIdentityServer4ClientScope>()})
                MERGE (client)-[:{Neo4jConstants.Relationships.HasScope}]->(scope)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, scope));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteScopeAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientScope scope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            scope.ThrowIfNull(nameof(scope));
            try
            {
                var cypher = $@"
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasScope}]->(scope:{IdSrv4ClientScope})
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
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteScopesAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            try
            {
                var cypher = $@"
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasScope}]->(scope:{IdSrv4ClientScope})
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
                return ex.ToIdentityResult();
            }
        }

        public async Task<Neo4jIdentityServer4ClientScope> FindScopeAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientScope scope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            scope.ThrowIfNull(nameof(scope));
            var cypher = $@"
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasScope}]->(scope:{IdSrv4ClientScope})
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
                 MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasScope}]->(scope:{IdSrv4ClientScope})
                WHERE client.ClientId = $p0
                RETURN scope{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var scopes = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientScope>("scope"));
            return scopes;
        }
    }
}
