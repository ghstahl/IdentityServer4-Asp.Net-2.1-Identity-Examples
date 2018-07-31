using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras;
using Neo4jExtras.Extensions;

namespace Stores.IdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4Store<TUser> where TUser : Neo4jIdentityUser
    {
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
                MATCH (u:{IdSrv4Client} {{ClientId: $p0}})
                MERGE (l:{IdSrv4ClientGrantType} {"$p1".AsMapFor<Neo4jIdentityServer4ClientGrantType>()})
                MERGE (u)-[:{Neo4jConstants.Relationships.HasGrantType}]->(l)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, grantType));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IList<Neo4jIdentityServer4ClientGrantType>> GetAllowedGrantTypesAsync(
            Neo4jIdentityServer4Client client, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                MATCH (c:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasGrantType}]->(g:{IdSrv4ClientGrantType})
                WHERE c.ClientId = $p0
                RETURN g{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var grantTypes = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientGrantType>("g"));
            return grantTypes;

        }
 
    }
}