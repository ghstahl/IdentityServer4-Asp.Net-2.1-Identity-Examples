using System;
using AspNetCore.Identity.Neo4j.Internal;
using Neo4jExtras;

namespace AspNetCore.Identity.Neo4j
{
    [Neo4jLabel(Neo4jConstants.Labels.ChallengeFactor)]
    public class ChallengeFactor
    {
        public string FactorId { get; set; }
        public string Challenge { get; set; }
        public string ChallengeResponseHash { get; set; }

        public static string UniqueFactorId()
        {
            return Unique.G;
        }
        public static string GenerateChallengeResponseHash(string value)
        {
           return SecurePasswordHasher.Hash(value);
        }
    }
}