using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras;
using Neo4jExtras.Extensions;
using Stores.IdentityServer4Neo4j.Events;

namespace StoresIdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStore<TUser>
        where TUser : Neo4jIdentityUser
    {
        private static readonly string IdSrv4ClientApiResource;
        private static readonly string IdSrv4ClientApiResourceClaim;
        private static readonly string IdSrv4ClientApiSecret;

        private Task RaiseApiResourceChangeEventAsync(Neo4jIdentityServer4ApiResource apiResource)
        {
            return _eventService.RaiseAsync(new ApiResourceChangeEvent<Neo4jIdentityServer4ApiResource>(apiResource));
        }

        public async Task<IdentityResult> CreateApiResourceAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            try
            {
                var cypher = $@"CREATE (r:{IdSrv4ClientApiResource} $p0)";
                await Session.RunAsync(cypher, Params.Create(apiResource.ConvertToMap()));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> UpdateApiResourceAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            try
            {
                var cypher = $@"
                MATCH (r:{IdSrv4ClientApiResource})
                WHERE r.Name = $p0
                SET r = $p1";

                await Session.RunAsync(cypher, Params.Create(apiResource.Name, apiResource.ConvertToMap()));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteApiResourceAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            try
            {
                var cypher = $@"
                MATCH (r:{IdSrv4ClientApiResource})
                WHERE r.Name = $p0
                DETACH DELETE r";

                await Session.RunAsync(cypher, Params.Create(apiResource.Name));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteApiResourcesAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            try
            {
                var cypher = $@"
                MATCH (r:{IdSrv4ClientApiResource})
                DETACH DELETE r";

                await Session.RunAsync(cypher);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<Neo4jIdentityServer4ApiResource> FindApiResourceAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));

            var cypher = $@"
                MATCH (r:{IdSrv4ClientApiResource})
                WHERE r.Name = $p0
                RETURN r {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(apiResource.Name));
            var grantTypeRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ApiResource>("r"));
            return grantTypeRecord;
        }

        public async Task<IdentityResult> AddApiResourceClaimToApiResourceAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            Neo4jIdentityServer4ApiResourceClaim apiResourceClaim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> RemoveApiResourceClaimFromApiResourceAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            Neo4jIdentityServer4ApiResourceClaim apiResourceClaim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Neo4jIdentityServer4ApiResourceClaim>> GetApiResourceClaimsAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        

        public async Task<IdentityResult> AddApiSecretToApiResourceAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            Neo4jIdentityServer4ApiSecret apiSecret, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> RemoveApiSecretFromApiResourceAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            Neo4jIdentityServer4ApiSecret apiSecret,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Neo4jIdentityServer4ApiSecret>> GetApiSecretsAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResource> RollupAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResource> GetRollupAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> DeleteRollupAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
