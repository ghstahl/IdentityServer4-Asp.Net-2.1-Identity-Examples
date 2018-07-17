using AspNetCore.Identity.MultiFactor.Test.Core;
using AspNetCore.Identity.Neo4j;

namespace AspNetCore.Identity.MultiFactor.Test.Neo4j.Models
{
    [Neo4jLabel("TestFactor")]
    public class TestFactor : ChallengeFactor
    {
        public TestFactor(): base(Unique.S,Unique.S)
        {

        }
        public TestFactor(string challenge, string challengeResponse) : base(challenge, challengeResponse)
        {
        }
    }
}
