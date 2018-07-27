namespace Stores.IdentityServer4.Neo4j
{
    internal static class Neo4jConstants
    {
        internal static class Labels
        {
            public const string IdentityServer4Client = "IdentityServer4Client";
        }

        internal static class Relationships
        {
            public const string InRole = "IN_ROLE";
            public const string HasClaim = "HAS_CLAIM";
            public const string HasLogin = "HAS_LOGIN";
            public const string HasToken = "HAS_TOKEN";
        }
    }
}