using Neo4jExtras;

namespace AspNetCore.Identity.Neo4j.Test.Models
{
    [Neo4jLabel("TestUser")]
    public class TestUser : Neo4jIdentityUser
    {
    }
}
