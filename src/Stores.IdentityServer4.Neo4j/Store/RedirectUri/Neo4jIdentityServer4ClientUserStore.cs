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
        private static readonly string IdSrv4ClientRedirectUri;
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
                MATCH (client:{IdSrv4Client} {{ClientId: $p0}})
                MERGE (redirectUri:{IdSrv4ClientRedirectUri} {"$p1".AsMapForNoNull<Neo4jIdentityServer4ClientRedirectUri>(redirectUri)})
                MERGE (client)-[:{Neo4jConstants.Relationships.HasRedirectUri}]->(redirectUri)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, redirectUri));
                await RaiseClientChangeEventAsync(client);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
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
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasRedirectUri}]->(redirectUri:{
                        IdSrv4ClientRedirectUri
                    })
                WHERE client.ClientId = $p0 AND redirectUri.RedirectUri = $p1  
                DETACH DELETE redirectUri";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId,
                        redirectUri.RedirectUri
                    ));
                await RaiseClientChangeEventAsync(client);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
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
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasRedirectUri}]->(redirectUri:{
                        IdSrv4ClientRedirectUri
                    })
                WHERE client.ClientId = $p0  
                DETACH DELETE redirectUri";

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

        public async Task<Neo4jIdentityServer4ClientRedirectUri> FindRedirectUriAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientRedirectUri redirectUri,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            redirectUri.ThrowIfNull(nameof(redirectUri));
            var cypher = $@"
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasRedirectUri}]->(redirectUri:{
                    IdSrv4ClientRedirectUri
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

        public async Task<IList<Neo4jIdentityServer4ClientRedirectUri>> GetRedirectUrisAsync(
            Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                 MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasRedirectUri}]->(red:{
                    IdSrv4ClientRedirectUri
                })
                WHERE client.ClientId = $p0
                RETURN red{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var records = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientRedirectUri>("red"));
            return records;
        }
    }
}
