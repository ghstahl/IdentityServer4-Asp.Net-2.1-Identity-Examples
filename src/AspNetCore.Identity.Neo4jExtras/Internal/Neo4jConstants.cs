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
            public const string HasFactor = "HAS_FACTOR";
        }
    }
}