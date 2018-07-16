namespace AspNetCore.Identity.Neo4j.Internal
{
    internal static class Neo4jConstants
    {
        internal static class Labels
        {
            public const string ChallengeFactor = "ChallengeFactor";
            
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