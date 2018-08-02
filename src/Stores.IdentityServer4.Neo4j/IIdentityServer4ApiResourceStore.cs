using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;

namespace StoresIdentityServer4.Neo4j
{
    public interface IIdentityServer4ApiResourceStore<TApiResource> :
        IDisposable
        where TApiResource : ApiResource
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
    }
}