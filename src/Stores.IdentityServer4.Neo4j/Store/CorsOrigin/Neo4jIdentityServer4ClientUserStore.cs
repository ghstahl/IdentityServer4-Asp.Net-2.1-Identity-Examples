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

namespace StoresIdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStore<TUser> where TUser : Neo4jIdentityUser
    {
        private static readonly string IdSrv4ClientCorsOrigin;

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
                MATCH (client:{IdSrv4Client} {{ClientId: $p0}})
                MERGE (corsOrigin:{IdSrv4ClientCorsOrigin} {"$p1".AsMapForNoNull<Neo4jIdentityServer4ClientCorsOrigin>(corsOrigin)})
                MERGE (client)-[:{Neo4jConstants.Relationships.HasCorsOrigin}]->(corsOrigin)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, corsOrigin));
                await RaiseClientChangeEventAsync(client);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteCorsOriginAsync(Neo4jIdentityServer4Client client,
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
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasCorsOrigin}]->(corsOrigin:{
                        IdSrv4ClientCorsOrigin
                    })
                WHERE client.ClientId = $p0 AND corsOrigin.Origin = $p1 
                DETACH DELETE corsOrigin";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId,
                        corsOrigin.Origin
                    ));
                await RaiseClientChangeEventAsync(client);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
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
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasCorsOrigin}]->(corsOrigin:{
                        IdSrv4ClientCorsOrigin
                    })
                WHERE client.ClientId = $p0 
                DETACH DELETE corsOrigin";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId
                    ));
                await RaiseClientChangeEventAsync(client);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
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
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasCorsOrigin}]->(corsOrigin:{
                    IdSrv4ClientCorsOrigin
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

        public async Task<IList<Neo4jIdentityServer4ClientCorsOrigin>> GetCorsOriginsAsync(
            Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                 MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasCorsOrigin}]->(corsOrigin:{
                    IdSrv4ClientCorsOrigin
                })
                WHERE client.ClientId = $p0
                RETURN corsOrigin{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var corsOrigins =
                await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientCorsOrigin>("corsOrigin"));
            return corsOrigins;
        }
    }
}
