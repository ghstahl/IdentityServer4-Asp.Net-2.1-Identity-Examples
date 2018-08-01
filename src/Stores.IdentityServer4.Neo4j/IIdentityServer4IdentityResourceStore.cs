using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using Stores.IdentityServer4.Neo4j.Entities;
using IdentityResource = Stores.IdentityServer4.Neo4j.Entities.IdentityResource;

namespace Stores.IdentityServer4.Neo4j
{
    public interface IIdentityServer4ResouceStore<TIdentityResource,TApiResource> : 
        IResourceStore,
        IIdentityServer4ApiResourceStore<TApiResource>,
        IIdentityServer4IdentityResourceStore<TIdentityResource>
        where TApiResource : ApiResource
        where TIdentityResource : IdentityResource
    {

    }

    public interface IIdentityServer4IdentityResourceStore<TIdentityResource> :
        IDisposable
        where TIdentityResource : IdentityResource
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

        Task<TIdentityResource> FindIdentityResourceAsync(
            string identityResource,
            CancellationToken cancellationToken = default(CancellationToken));
    }
    public interface IIdentityServer4ApiResourceStore<TApiResource> :
        IDisposable
        where TApiResource : ApiResource
    {
        Task<IdentityResult> CreateApiResourceAsync(
            TApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> UpdateApiResourceAsync(
            TApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteApiResourceAsync(
            TApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteApiResourcesAsync(
            CancellationToken cancellationToken = default(CancellationToken));

        Task<TApiResource> FindApiResourceAsync(
            string name,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}