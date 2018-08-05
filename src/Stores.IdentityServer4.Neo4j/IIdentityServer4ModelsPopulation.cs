using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4Extras;
using Microsoft.AspNetCore.Identity;

namespace StoresIdentityServer4.Neo4j
{
    public interface IIdentityServer4ModelsPopulation
    {
        Task<IdentityResult> EnsureStandardAsync(
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> InsertIdentityResource(
            IdentityServer4.Models.IdentityResource model, 
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> InsertIdentityResources(
            IEnumerable<IdentityServer4.Models.IdentityResource> models,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> InsertApiResource(
            IdentityServer4.Models.ApiResource model,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> InsertApiResources(
            IEnumerable<IdentityServer4.Models.ApiResource> models,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> InsertClient(
            ClientExtra model,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}