using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace StoresIdentityServer4.Neo4j
{
    public interface IIdentityServer4ApiResourceStore<
        TApiResource,
        TApiResourceClaim,
        TApiSecret,
        TApiScope, 
        TApiScopeClaim> :
        IIdentityServer4ApiScopeStore<TApiScope, TApiScopeClaim>,
        IDisposable
        where TApiResource : StoresIdentityServer4.Neo4j.Entities.ApiResource
        where TApiResourceClaim : StoresIdentityServer4.Neo4j.Entities.ApiResourceClaim
        where TApiSecret : StoresIdentityServer4.Neo4j.Entities.ApiSecret
        where TApiScope : StoresIdentityServer4.Neo4j.Entities.ApiScope
        where TApiScopeClaim : StoresIdentityServer4.Neo4j.Entities.ApiScopeClaim
    {
        Task<IdentityResult> CreateApiResourceAsync(TApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> UpdateApiResourceAsync(TApiResource apiResource, 
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteApiResourceAsync(TApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteApiResourcesAsync(
            CancellationToken cancellationToken = default(CancellationToken));

        Task<TApiResource> FindApiResourceAsync(TApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> AddApiResourceClaimToApiResourceAsync(
            TApiResource apiResource,
            TApiResourceClaim apiResourceClaim,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> RemoveApiResourceClaimFromApiResourceAsync(
            TApiResource apiResource,
            TApiResourceClaim apiResourceClaim,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TApiResourceClaim>> GetApiResourceClaimsAsync(TApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> AddApiScopeToApiResourceAsync(
            TApiResource apiResource,
            TApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> RemoveApiScopeFromApiResourceAsync(
            TApiResource apiResource,
            TApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TApiScope>> GetApiScopesAsync(TApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> AddApiSecretToApiResourceAsync(
            TApiResource apiResource,
            TApiSecret apiSecret,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> RemoveApiSecretFromApiResourceAsync(
            TApiResource apiResource,
            TApiSecret apiSecret,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TApiSecret>> GetApiSecretsAsync(TApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityServer4.Models.ApiResource> RollupAsync(TApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<global::IdentityServer4.Models.ApiResource> GetRollupAsync(TApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> DeleteRollupAsync(TApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}