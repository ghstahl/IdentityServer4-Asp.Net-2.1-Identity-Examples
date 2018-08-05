using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4Extras;
using Microsoft.AspNetCore.Identity;
 

namespace StoresIdentityServer4.Neo4j
{
    public interface IIdentityServer4ClientStore<in TClient,
        TSecret,
        TGrantType,
        TApiResource,
        TApiResourceClaim,
        TApiSecret,
        TApiScope,
        TApiScopeClaim,
        TClaim,
        TCorsOrigin,
        TScope,
        TIDPRestriction,
        TProperty,
        TPostLogoutRedirectUri,
        TRedirectUri,
        TIdentityResource,
        TIdentityClaim> :
        IIdentityServer4GrantTypeStore<TGrantType>,
        IIdentityServer4ApiResourceStore<
            TApiResource, 
            TApiResourceClaim,
            TApiSecret, 
            TApiScope,
            TApiScopeClaim>,
        IIdentityServer4IdentityResourceStore<
            TIdentityResource,
            TIdentityClaim>,
        IIdentityServer4ModelsPopulation,
        IDisposable
        where TClient : StoresIdentityServer4.Neo4j.Entities.ClientExtra
        where TSecret : StoresIdentityServer4.Neo4j.Entities.Secret
        where TGrantType : StoresIdentityServer4.Neo4j.Entities.ClientGrantType
        where TApiResource : StoresIdentityServer4.Neo4j.Entities.ApiResource
        where TApiResourceClaim : StoresIdentityServer4.Neo4j.Entities.ApiResourceClaim
        where TIdentityResource : StoresIdentityServer4.Neo4j.Entities.IdentityResource
        where TIdentityClaim : StoresIdentityServer4.Neo4j.Entities.IdentityClaim
        where TApiSecret : StoresIdentityServer4.Neo4j.Entities.ApiSecret
        where TApiScope : StoresIdentityServer4.Neo4j.Entities.ApiScope
        where TApiScopeClaim : StoresIdentityServer4.Neo4j.Entities.ApiScopeClaim
        where TClaim : StoresIdentityServer4.Neo4j.Entities.ClientClaim
        where TCorsOrigin : StoresIdentityServer4.Neo4j.Entities.ClientCorsOrigin
        where TScope : StoresIdentityServer4.Neo4j.Entities.ClientScope
        where TIDPRestriction : StoresIdentityServer4.Neo4j.Entities.ClienTIDPRestriction
        where TProperty : StoresIdentityServer4.Neo4j.Entities.ClientProperty
        where TPostLogoutRedirectUri : StoresIdentityServer4.Neo4j.Entities.ClientPostLogoutRedirectUri
        where TRedirectUri : StoresIdentityServer4.Neo4j.Entities.ClientRedirectUri
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
            TIDPRestriction idPRestriction,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> DeleteIDPRestrictionAsync(TClient client, TIDPRestriction idPRestriction,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> DeleteIDPRestrictionsAsync(TClient client, 
            CancellationToken cancellationToken = default(CancellationToken));
        Task<TIDPRestriction> FindIDPRestrictionAsync(TClient client, TIDPRestriction idPRestriction,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TIDPRestriction>> GetIDPRestrictionsAsync(TClient client,
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