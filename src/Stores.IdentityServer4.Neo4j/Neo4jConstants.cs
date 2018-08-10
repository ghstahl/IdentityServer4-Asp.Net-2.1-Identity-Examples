namespace StoresIdentityServer4.Neo4j
{
    internal static class Neo4jConstants
    {
        internal static class Labels
        {
            public const string IdentityClaim = "IdSrv4IdentityClaim";
            public const string IdentityResource = "IdSrv4IdentityResource";
            public const string ApiResource = "IdSrv4ApiResource";
            public const string ApiResourceClaim = "IdSrv4ApiResourceClaim";
            public const string ApiScope = "IdSrv4ApiScope";
            public const string ApiScopeClaim = "IdSrv4ApiScopeClaim";
            public const string ApiSecret = "IdSrv4ApiSecret";
            public const string ClientRollup = "IdSrv4ClientRollup";
            public const string ApiResourceRollup = "IdSrv4ApiResourceRollup";
            public const string ApiResourcesRollup = "IdSrv4ApiResourcesRollup";
            
            public const string IdentityResourceRollup = "IdSrv4IdentityResourceRollup";
            
            public const string ApiScopeRollup = "IdSrv4ApiScopeRollup";
            public const string Client = "IdSrv4Client";
            public const string Secret = "IdSrv4ClientSecret";
            public const string GrantType = "IdSrv4ClientGrantType";
            public const string RedirectUri = "IdSrv4ClientRedirectUri";
            public const string PostLogoutRedirectUri = "IdSrv4ClientPostLogoutRedirectUri";
            public const string Scope = "IdSrv4ClientScope";
            public const string IDPRestriction = "IdSrv4ClientIDPRestriction";
            public const string Claim = "IdSrv4ClientClaim";
            public const string CorsOrigin = "IdSrv4ClientCorsOrigin";
            public const string Property = "IdSrv4ClientProperty";

        }

        internal static class Relationships
        {
            public const string HasRollup = "HAS_ROLLUP";
            public const string HasClient = "HAS_CLIENT";
            public const string HasSecret = "HAS_SECRET";
            public const string HasGrantType = "HAS_GRANTTYPE";
            public const string HasRedirectUri = "HAS_REDIRECTURI";
            public const string HasPostLogoutRedirectUri = "HAS_POSTLOGOUTREDIRECTURI";
            public const string HasScope = "HAS_SCOPE";
            public const string HasIdPRestriction = "HAS_IDPRESTRICTION";
            public const string HasClaim = "HAS_CLAIM";
            public const string HasCorsOrigin = "HAS_CORSORIGIN";
            public const string HasProperty = "HAS_PROPERTY";
        }
    }
}