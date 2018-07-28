using AspNetCore.Identity.MultiFactor.Test.Core;
using AspNetCore.Identity.Neo4j;
using Neo4jExtras;

namespace AspNetCore.Identity.MultiFactor.Test.Neo4j.Models
{
    [Neo4jLabel("TestFactor")]
    public class TestFactor : ChallengeFactor
    {
        
    }
}
