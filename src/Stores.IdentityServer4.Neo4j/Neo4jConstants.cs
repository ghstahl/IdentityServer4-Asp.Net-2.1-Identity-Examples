namespace Stores.IdentityServer4.Neo4j
{
    internal static class Neo4jConstants
    {
        internal static class Labels
        {
            public const string IdentityServer4Client = "IdentityServer4Client";
            public const string IdentityServer4ClientSecret = "IdentityServer4ClientSecret";
        }

        internal static class Relationships
        {
            public const string HasClient = "HAS_CLIENT";
            public const string HasSecret = "HAS_SECRET";
        }
    }
}