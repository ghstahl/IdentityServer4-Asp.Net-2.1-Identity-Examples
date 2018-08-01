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
        private static readonly string IdSrv4ClientClaim;
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
                MATCH (client:{IdSrv4Client} {{ClientId: $p0}})
                MERGE (claim:{IdSrv4ClientClaim} {"$p1".AsMapFor<Neo4jIdentityServer4ClientClaim>()})
                MERGE (client)-[:{Neo4jConstants.Relationships.HasClaim}]->(claim)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, claim));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
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
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasClaim}]->(claim:{IdSrv4ClientClaim})
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
                return ex.ToIdentityResult();
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
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasClaim}]->(claim:{IdSrv4ClientClaim})
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
                return ex.ToIdentityResult();
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
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasClaim}]->(claim:{IdSrv4ClientClaim})
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
                 MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasClaim}]->(claim:{IdSrv4ClientClaim})
                WHERE client.ClientId = $p0
                RETURN claim{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var claims = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientClaim>("claim"));
            return claims;
        }
    }
}
