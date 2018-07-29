namespace Stores.IdentityServer4.Neo4j
{
    internal static class Neo4jConstants
    {
        internal static class Labels
        {
            public const string IdentityServer4Client = "IdentityServer4Client";
            public const string IdentityServer4ClientSecret = "IdentityServer4ClientSecret";
            public const string IdentityServer4ClientGrantType = "IdentityServer4ClientGrantType";
            public const string IdentityServer4ClientRedirectUri = "IdentityServer4ClientRedirectUri";
            public const string IdentityServer4ClientPostLogoutRedirectUri = "IdentityServer4ClientPostLogoutRedirectUri";
            public const string IdentityServer4ClientScope = "IdentityServer4ClientScope";
            public const string IdentityServer4ClientIdPRestriction = "IdentityServer4ClientIdPRestriction";
            public const string IdentityServer4ClientClaim = "IdentityServer4ClientClaim";
            public const string IdentityServer4ClientCorsOrigin = "IdentityServer4ClientCorsOrigin";
            public const string IdentityServer4ClientProperty = "IdentityServer4ClientProperty";

        }

        internal static class Relationships
        {
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