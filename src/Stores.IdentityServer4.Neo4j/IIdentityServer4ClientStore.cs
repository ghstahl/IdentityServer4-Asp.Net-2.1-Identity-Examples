using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4Extras;
using Microsoft.AspNetCore.Identity;
using StoresIdentityServer4.Neo4j.Entities;
using Client = StoresIdentityServer4.Neo4j.Entities.Client;

namespace StoresIdentityServer4.Neo4j
{
    public interface IIdentityServer4ClientStore<in TClient,
        TSecret,
        TGrantType,
        TApiResource,
        TClaim,
        TCorsOrigin,
        TScope,
        TIdPRestriction,
        TProperty,
        TPostLogoutRedirectUri,
        TRedirectUri> :
        IIdentityServer4GrantTypeStore<TGrantType>,
        IIdentityServer4ApiResourceStore<TApiResource>,
        IDisposable
        where TClient : Client
        where TSecret : Secret
        where TGrantType : ClientGrantType
        where TApiResource : StoresIdentityServer4.Neo4j.Entities.ApiResource
        where TClaim : ClientClaim
        where TCorsOrigin : ClientCorsOrigin
        where TScope : ClientScope
        where TIdPRestriction : ClientIDPRestriction
        where TProperty : ClientProperty
        where TPostLogoutRedirectUri : ClientPostLogoutRedirectUri
        where TRedirectUri : ClientRedirectUri

    {
        Task<IdentityServer4.Models.Client> RollupAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<global::IdentityServer4.Models.Client> GetRollupAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> DeleteRollupAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));
        #region Client  
        Task<IdentityResult> CreateClientAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> UpdateClientAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteClientAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<Neo4jIdentityServer4Client> FindClientByClientIdAsync(string clientId,
            CancellationToken cancellationToken = default(CancellationToken));
        #endregion

        #region Secret  

        Task<IdentityResult> AddSecretToClientAsync(TClient client,
            TSecret secret,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> UpdateSecretAsync(TClient client, TSecret secret,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteSecretAsync(TClient client, TSecret secret,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> DeleteSecretsAsync(TClient client,  
            CancellationToken cancellationToken = default(CancellationToken));
        Task<TSecret> FindSecretAsync(TClient client, TSecret secret,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TSecret>> GetSecretsAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion

        #region AllowedGrantType  
        Task<IdentityResult> AddAllowedGrantTypeToClientAsync(TClient client,
            TGrantType grantType,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TGrantType>> GetAllowedGrantTypesAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));
        #endregion
        #region Claim  

        Task<IdentityResult> AddClaimToClientAsync(TClient client,
            TClaim claim,
            CancellationToken cancellationToken = default(CancellationToken));


        Task<IdentityResult> DeleteClaimAsync(TClient client, TClaim claim,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> DeleteClaimsAsync(TClient client, 
            CancellationToken cancellationToken = default(CancellationToken));
        Task<TClaim> FindClaimAsync(TClient client, TClaim claim,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TClaim>> GetClaimsAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
        #region CorsOrigin  

        Task<IdentityResult> AddCorsOriginToClientAsync(TClient client,
            TCorsOrigin corsOrigin,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteCorsOriginAsync(TClient client, TCorsOrigin corsOrigin,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> DeleteCorsOriginsAsync(TClient client, 
            CancellationToken cancellationToken = default(CancellationToken));

        Task<TCorsOrigin> FindCorsOriginAsync(TClient client, TCorsOrigin corsOrigin,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TCorsOrigin>> GetCorsOriginsAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
        #region Scope  

        Task<IdentityResult> AddScopeToClientAsync(TClient client,
            TScope scope,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteScopeAsync(TClient client, TScope scope,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> DeleteScopesAsync(TClient client,  
            CancellationToken cancellationToken = default(CancellationToken));
        Task<TScope> FindScopeAsync(TClient client, TScope scope,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TScope>> GetScopesAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
        #region IdPRestriction  

        Task<IdentityResult> AddIdPRestrictionToClientAsync(TClient client,
            TIdPRestriction idPRestriction,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteIdPRestrictionAsync(TClient client, TIdPRestriction idPRestriction,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> DeleteIdPRestrictionsAsync(TClient client, 
            CancellationToken cancellationToken = default(CancellationToken));
        Task<TIdPRestriction> FindIdPRestrictionAsync(TClient client, TIdPRestriction idPRestriction,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TIdPRestriction>> GetIdPRestrictionsAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
        #region Property  

        Task<IdentityResult> AddPropertyToClientAsync(TClient client,
            TProperty property,
            CancellationToken cancellationToken = default(CancellationToken));


        Task<IdentityResult> DeletePropertyAsync(TClient client, TProperty property,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> DeletePropertiesAsync(TClient client, 
            CancellationToken cancellationToken = default(CancellationToken));
        Task<TProperty> FindPropertyAsync(TClient client, TProperty property,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TProperty>> GetPropertiesAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
        #region PostLogoutRedirectUri  

        Task<IdentityResult> AddPostLogoutRedirectUriToClientAsync(TClient client,
            TPostLogoutRedirectUri postLogoutRedirectUri,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeletePostLogoutRedirectUriAsync(TClient client, TPostLogoutRedirectUri postLogoutRedirectUri,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeletePostLogoutRedirectUrisAsync(TClient client,  
            CancellationToken cancellationToken = default(CancellationToken));

        Task<TPostLogoutRedirectUri> FindPostLogoutRedirectUriAsync(TClient client, TPostLogoutRedirectUri postLogoutRedirectUri,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TPostLogoutRedirectUri>> GetPostLogoutRedirectUrisAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
        #region RedirectUri  

        Task<IdentityResult> AddRedirectUriToClientAsync(TClient client,
            TRedirectUri redirectUri,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteRedirectUriAsync(TClient client, TRedirectUri redirectUri,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> DeleteRedirectUrisAsync(TClient client,  
            CancellationToken cancellationToken = default(CancellationToken));
        Task<TRedirectUri> FindRedirectUriAsync(TClient client, TRedirectUri redirectUri,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TRedirectUri>> GetRedirectUrisAsync(TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}