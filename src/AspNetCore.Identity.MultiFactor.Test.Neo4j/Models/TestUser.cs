using AspNetCore.Identity.Neo4j;
using Neo4jExtras;

namespace AspNetCore.Identity.MultiFactor.Test.Neo4j.Models
{
    [Neo4jLabel("TestUser")]
    public class TestUser : Neo4jIdentityUser
    {
    }
}
