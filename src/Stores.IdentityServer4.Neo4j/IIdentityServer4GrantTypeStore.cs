using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace StoresIdentityServer4.Neo4j
{
    public interface IIdentityServer4GrantTypeStore<TGrantType> :
        IDisposable
        where TGrantType : StoresIdentityServer4.Neo4j.Entities.ClientGrantType
    {
        Task<IdentityResult> CreateGrantTypeAsync(TGrantType grantType,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> UpdateGrantTypeAsync(TGrantType grantType,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteGrantTypeAsync(TGrantType grantType,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> DeleteGrantTypesAsync(
            CancellationToken cancellationToken = default(CancellationToken));
        Task<TGrantType> FindGrantTypeAsync(TGrantType grantType,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}