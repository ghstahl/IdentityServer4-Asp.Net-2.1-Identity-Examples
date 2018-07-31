using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Stores.IdentityServer4.Neo4j.Entities;

namespace Stores.IdentityServer4.Neo4j
{
    public interface IIdentityServer4ClientStore2<TClient, TGrantType> :
        IIdentityServer4GrantTypeStore<TGrantType>,
        IDisposable
        where TClient : ClientRoot
        where TGrantType : ClientGrantType
    {
        #region Client  

        Task<IdentityResult> CreateClientAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> UpdateClientAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteClientAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<TClient> FindClientByClientIdAsync(string clientId,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion


        #region AllowedGrantType  

        Task<IdentityResult> AddAllowedGrantTypeToClientAsync(TClient client,
            TGrantType grantType,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TGrantType>> GetAllowedGrantTypesAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion

        #region Secret
        Task<IdentityResult> AddSecretAsync(TClient client, Secret secret,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> RemoveSecretAsync(TClient client, Secret secret,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<Secret> GetSecretAsync(TClient client,Secret secret,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IList<Secret>> GetSecretsAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));
        #endregion
    }
}