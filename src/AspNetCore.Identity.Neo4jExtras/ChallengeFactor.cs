using System;
using AspNetCore.Identity.Neo4j.Internal;

namespace AspNetCore.Identity.Neo4j
{
    [Neo4jLabel(Neo4jConstants.Labels.ChallengeFactor)]
    public class ChallengeFactor
    {
        public string Id { get; set; }
        public string Challenge { get; set; }
        public string ChallengeResponseHash { get; set; }
        public ChallengeFactor()
        {
            Id = Guid.NewGuid().ToString();
        }
        public ChallengeFactor(string challenge,string challengeResponse)
        {
            Id = Guid.NewGuid().ToString();
            Challenge = challenge;
            ChallengeResponseHash = SecurePasswordHasher.Hash(challengeResponse);
        }
    }
}