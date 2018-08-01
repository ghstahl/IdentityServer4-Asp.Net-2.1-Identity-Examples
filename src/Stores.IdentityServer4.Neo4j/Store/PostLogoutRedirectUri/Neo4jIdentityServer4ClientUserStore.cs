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
        private static readonly string IdSrv4ClientPostLogoutRedirectUri;
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
                MATCH (client:{IdSrv4Client} {{ClientId: $p0}})
                MERGE (postLogoutRedirectUri:{IdSrv4ClientPostLogoutRedirectUri} {
                        "$p1".AsMapFor<Neo4jIdentityServer4ClientPostLogoutRedirectUri>()
                    })
                MERGE (client)-[:{Neo4jConstants.Relationships.HasPostLogoutRedirectUri}]->(postLogoutRedirectUri)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, postLogoutRedirectUri));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
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
                MATCH (client:{IdSrv4Client})-[:{
                        Neo4jConstants.Relationships.HasPostLogoutRedirectUri
                    }]->(postLogoutRedirectUri:{IdSrv4ClientPostLogoutRedirectUri})
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
                return ex.ToIdentityResult();
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
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasPostLogoutRedirectUri}]->(p:{
                        IdSrv4ClientPostLogoutRedirectUri
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
                return ex.ToIdentityResult();
            }
        }

        public async Task<Neo4jIdentityServer4ClientPostLogoutRedirectUri> FindPostLogoutRedirectUriAsync(
            Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientPostLogoutRedirectUri postLogoutRedirectUri,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            postLogoutRedirectUri.ThrowIfNull(nameof(postLogoutRedirectUri));
            var cypher = $@"
                MATCH (client:{IdSrv4Client})-[:{
                    Neo4jConstants.Relationships.HasPostLogoutRedirectUri
                }]->(postLogoutRedirectUri:{IdSrv4ClientPostLogoutRedirectUri})
                WHERE client.ClientId = $p0 AND postLogoutRedirectUri.PostLogoutRedirectUri = $p1  
                RETURN postLogoutRedirectUri{{ .* }}";

            var result = await Session.RunAsync(cypher,
                Params.Create(
                    client.ClientId,
                    postLogoutRedirectUri.PostLogoutRedirectUri

                ));

            var foundRecord =
                await result.SingleOrDefaultAsync(r =>
                    r.MapTo<Neo4jIdentityServer4ClientPostLogoutRedirectUri>("postLogoutRedirectUri"));

            return foundRecord;
        }

        public async Task<IList<Neo4jIdentityServer4ClientPostLogoutRedirectUri>> GetPostLogoutRedirectUrisAsync(
            Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                 MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasPostLogoutRedirectUri}]->(post:{
                    IdSrv4ClientPostLogoutRedirectUri
                })
                WHERE client.ClientId = $p0
                RETURN post{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var records =
                await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientPostLogoutRedirectUri>("post"));
            return records;
        }
    }
}
