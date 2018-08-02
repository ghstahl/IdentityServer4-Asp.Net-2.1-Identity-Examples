using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace StoresIdentityServer4.Neo4j
{
    public interface IIdentityServer4ApiScopeStore<TApiScope,TApiScopeClaim> :
        IDisposable
        where TApiScope : StoresIdentityServer4.Neo4j.Entities.ApiScope
        where TApiScopeClaim : StoresIdentityServer4.Neo4j.Entities.ApiScopeClaim
    {
        Task<IdentityResult> CreateApiScopeAsync(TApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> UpdateApiScopeAsync(TApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteApiScopeAsync(TApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> DeleteApiScopesAsync(
            CancellationToken cancellationToken = default(CancellationToken));
        Task<TApiScope> FindApiScopeAsync(TApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> AddApiScopeClaimToApiScopeAsync(TApiScope apiScope,
            TApiScopeClaim apiScopeClaim,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> RemoveApiScopeClaimFromApiScopeAsync(TApiScope apiScope,
            TApiScopeClaim apiScopeClaim,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IList<TApiScopeClaim>> GetApiScopeClaimsAsync(TApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}