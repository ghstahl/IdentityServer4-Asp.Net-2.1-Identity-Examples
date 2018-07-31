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
    public partial class Neo4jIdentityServer4Store<TUser> where TUser : Neo4jIdentityUser
    {
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
    }
}