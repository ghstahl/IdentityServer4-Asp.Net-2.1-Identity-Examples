using IdentityServer4.Models;
using StoresIdentityServer4.Neo4j.Entities;
using Client = StoresIdentityServer4.Neo4j.Entities.Client;

namespace StoresIdentityServer4.Neo4j
{
    public interface IIdentityServer4ClientUserStoreAccessor<
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
        TRedirectUri>
        where TUser : class
        where TClient : Client
        where TSecret : Secret
        where TGrantType : ClientGrantType
        where TApiResource : StoresIdentityServer4.Neo4j.Entities.ApiResource
        where TApiResourceClaim : StoresIdentityServer4.Neo4j.Entities.ApiResourceClaim
        where TApiSecret : StoresIdentityServer4.Neo4j.Entities.ApiSecret
        where TApiScope : StoresIdentityServer4.Neo4j.Entities.ApiScope
        where TApiScopeClaim : StoresIdentityServer4.Neo4j.Entities.ApiScopeClaim
        where TClaim : ClientClaim
        where TCorsOrigin : ClientCorsOrigin
        where TScope : ClientScope
        where TIDPRestriction : ClienTIDPRestriction
        where TProperty : ClientProperty
        where TPostLogoutRedirectUri : ClientPostLogoutRedirectUri
        where TRedirectUri : ClientRedirectUri
    {
        IIdentityServer4ClientUserStore<
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
            TRedirectUri> IdentityServer4ClientUserStore { get; }
    }
}