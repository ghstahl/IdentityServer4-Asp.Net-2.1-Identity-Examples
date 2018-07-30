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
        private static readonly string IdSrv4ClientGrantType;

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

        public async Task<IdentityResult> CreateGrantTypeAsync(Neo4jIdentityServer4ClientGrantType grantType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            grantType.ThrowIfNull(nameof(grantType));
            try
            {
                var cypher = $@"CREATE (r:{IdSrv4ClientGrantType} $p0)";
                await Session.RunAsync(cypher, Params.Create(grantType.ConvertToMap()));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
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
                MATCH (r:{IdSrv4ClientGrantType})
                WHERE r.GrantType = $p0
                SET r = $p1";

                await Session.RunAsync(cypher, Params.Create(originalGrantType.GrantType, grantType.ConvertToMap()));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
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
                MATCH (r:{IdSrv4ClientGrantType})
                WHERE r.GrantType = $p0
                DETACH DELETE r";

                await Session.RunAsync(cypher, Params.Create(grantType.GrantType));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteGrantTypesAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            try
            {
                var cypher = $@"
                MATCH (r:{IdSrv4ClientGrantType})
                DETACH DELETE r";

                await Session.RunAsync(cypher);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<Neo4jIdentityServer4ClientGrantType> FindGrantTypeAsync(string grantType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            grantType.ThrowIfNull(nameof(grantType));

            var cypher = $@"
                MATCH (r:{IdSrv4ClientGrantType})
                WHERE r.GrantType = $p0
                RETURN r {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(grantType));
            var grantTypeRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ClientGrantType>("r"));
            return grantTypeRecord;
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
