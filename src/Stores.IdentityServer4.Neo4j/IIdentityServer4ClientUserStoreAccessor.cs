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
        TClaim,
        TCorsOrigin,
        TScope,
        TIdPRestriction,
        TProperty,
        TPostLogoutRedirectUri,
        TRedirectUri>
        where TUser : class
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
        IIdentityServer4ClientUserStore<
            TUser,
            TClient,
            TSecret,
            TGrantType,
            TApiResource,
            TClaim,
            TCorsOrigin,
            TScope,
            TIdPRestriction,
            TProperty,
            TPostLogoutRedirectUri,
            TRedirectUri> IdentityServer4ClientUserStore { get; }
    }
}