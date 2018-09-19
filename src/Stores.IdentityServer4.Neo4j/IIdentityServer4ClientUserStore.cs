using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;


namespace StoresIdentityServer4.Neo4j
{
    public interface IIdentityServer4ClientUserStore<
        TUser,
        TClient,
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
        IIdentityServer4ClientStore<
            TClient,
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
            TIdentityClaim>,
        IIdentityServer4ModelsPopulation<TUser>,
        INeo4jIdentityServer4Database,
        IClientStore,
        IResourceStore,
        IDisposable
        where TUser : class
        where TClient : StoresIdentityServer4.Neo4j.Entities.ClientExtra
        where TSecret : StoresIdentityServer4.Neo4j.Entities.Secret
        where TGrantType : StoresIdentityServer4.Neo4j.Entities.IdentityServer4GrantType
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
           Task<IdentityResult> AddClientToUserAsync(TUser user, TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TClient>> GetClientsAsync(TUser user,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}