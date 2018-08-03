using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace StoresIdentityServer4.Neo4j
{
    public interface IIdentityServer4IdentityResourceStore<
        TIdentityResource,
        TIdentityClaim> :
        IDisposable
        where TIdentityResource : StoresIdentityServer4.Neo4j.Entities.IdentityResource
        where TIdentityClaim : StoresIdentityServer4.Neo4j.Entities.IdentityClaim
    {
        Task<IdentityResult> CreateIdentityResourceAsync(
            TIdentityResource identityResource,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> UpdateIdentityResourceAsync(
            TIdentityResource identityResource,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteIdentityResourceAsync(
            TIdentityResource identityResource,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteIdentityResourcesAsync(
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TIdentityResource>> GetIdentityResourcesAsync(
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> AddIdentityClaimAsync(
            TIdentityResource identityResource,
            TIdentityClaim identityClaim,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<TIdentityClaim> GetIdentityClaimAsync(
            TIdentityResource identityResource,
            TIdentityClaim identityClaim,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteIdentityClaimAsync(
            TIdentityResource identityResource,
            TIdentityClaim identityClaim,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TIdentityClaim>> GetIdentityClaimsAsync(
            TIdentityResource identityResource,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityServer4.Models.IdentityResource> RollupAsync(
            TIdentityResource identityResource,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<global::IdentityServer4.Models.IdentityResource> GetRollupAsync(
            TIdentityResource identityResource,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> DeleteRollupAsync(
            TIdentityResource identityResource,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}