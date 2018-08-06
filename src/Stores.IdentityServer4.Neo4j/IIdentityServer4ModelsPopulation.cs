using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4Extras;
using Microsoft.AspNetCore.Identity;

namespace StoresIdentityServer4.Neo4j
{
    public interface IIdentityServer4ModelsPopulation<TUser>
        where TUser : class
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
            TUser user,
            IdentityServer4.Models.ClientExtra model,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> InsertClients(
            TUser user,
            IEnumerable<IdentityServer4.Models.ClientExtra> models,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}