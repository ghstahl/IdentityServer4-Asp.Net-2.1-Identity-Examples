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
        private static readonly string IdSrv4ClientIDPRestriction;

        public async Task<IdentityResult> AddIdPRestrictionToClientAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientIDPRestriction idpRestriction,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            idpRestriction.ThrowIfNull(nameof(idpRestriction));
            try
            {
                var cypher = $@"
                MATCH (client:{IdSrv4Client} {{ClientId: $p0}})
                MERGE (idp:{IdSrv4ClientIDPRestriction} {"$p1".AsMapFor<Neo4jIdentityServer4ClientIDPRestriction>()})
                MERGE (client)-[:{Neo4jConstants.Relationships.HasIdPRestriction}]->(idp)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, idpRestriction));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteIdPRestrictionAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientIDPRestriction idpRestriction,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            idpRestriction.ThrowIfNull(nameof(idpRestriction));
            try
            {
                var cypher = $@"
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasIdPRestriction}]->(idp:{
                        IdSrv4ClientIDPRestriction
                    })
                WHERE client.ClientId = $p0 AND idp.Provider = $p1 
                DETACH DELETE idp";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId,
                        idpRestriction.Provider
                    ));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
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
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasIdPRestriction}]->(idp:{
                        IdSrv4ClientIDPRestriction
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
                return ex.ToIdentityResult();
            }
        }

        public async Task<Neo4jIdentityServer4ClientIDPRestriction> FindIdPRestrictionAsync(
            Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientIDPRestriction idpRestriction,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            idpRestriction.ThrowIfNull(nameof(idpRestriction));
            var cypher = $@"
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasIdPRestriction}]->(idp:{
                    IdSrv4ClientIDPRestriction
                })
                WHERE client.ClientId = $p0 AND idp.Provider = $p1  
                RETURN idp{{ .* }}";

            var result = await Session.RunAsync(cypher,
                Params.Create(
                    client.ClientId,
                    idpRestriction.Provider
                ));

            var foundRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ClientIDPRestriction>("idp"));

            return foundRecord;
        }

        public async Task<IList<Neo4jIdentityServer4ClientIDPRestriction>> GetIdPRestrictionsAsync(
            Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                 MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasIdPRestriction}]->(idp:{
                    IdSrv4ClientIDPRestriction
                })
                WHERE client.ClientId = $p0
                RETURN idp{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var ipds = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientIDPRestriction>("idp"));
            return ipds;
        }

    }
}
