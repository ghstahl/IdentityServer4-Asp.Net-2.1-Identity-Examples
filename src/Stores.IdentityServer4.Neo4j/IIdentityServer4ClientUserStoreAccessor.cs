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
        TRedirectUri,
        TIdentityResource,
        TIdentityClaim>
        where TUser : class
        where TClient : StoresIdentityServer4.Neo4j.Entities.Client
        where TSecret : IdentityServer4.Models.Secret
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
            TRedirectUri,
            TIdentityResource,
            TIdentityClaim> IdentityServer4ClientUserStore { get; }
    }
}