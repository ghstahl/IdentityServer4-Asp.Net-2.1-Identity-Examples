using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace StoresIdentityServer4.Neo4j
{
    public interface IIdentityServer4ApiScopeStore<
        TApiResource, 
        TApiScope,
        TApiScopeClaim> :
        IDisposable
        where TApiResource : StoresIdentityServer4.Neo4j.Entities.ApiResource
        where TApiScope : StoresIdentityServer4.Neo4j.Entities.ApiScope
        where TApiScopeClaim : StoresIdentityServer4.Neo4j.Entities.ApiScopeClaim
    {
        Task<IdentityResult> AddApiScopeAsync(
            TApiResource apiResource,
            TApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<TApiScope> GetApiScopeAsync(TApiResource apiResource, TApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken));
       
        Task<IList<TApiScope>> GetApiScopesAsync(TApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> UpdateApiScopeAsync(TApiResource apiResource, TApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteApiScopeAsync(TApiResource apiResource, TApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> DeleteApiScopesAsync(TApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken));


        Task<IdentityResult> AddApiScopeClaimToApiScopeAsync(
            TApiResource apiResource, 
            TApiScope apiScope,
            TApiScopeClaim apiScopeClaim,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> DeleteApiScopeClaimFromApiScopeAsync(
            TApiResource apiResource,
            TApiScope apiScope,
            TApiScopeClaim apiScopeClaim,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> DeleteApiScopeClaimsFromApiScopeAsync(
            TApiResource apiResource,
            TApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IList<TApiScopeClaim>> GetApiScopeClaimsAsync(
            TApiResource apiResource,
            TApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<TApiScopeClaim> GetApiScopeClaimAsync(
            TApiResource apiResource,
            TApiScope apiScope,
            TApiScopeClaim apiScopeClaim,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityServer4.Models.Scope> RollupAsync(
            TApiResource apiResource,
            TApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<global::IdentityServer4.Models.Scope> GetRollupAsync(
            TApiResource apiResource,
            TApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteRollupAsync(
            TApiResource apiResource,
            TApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}