using Neo4jExtras;

namespace AspNetCore.Identity.Neo4j.Test.Models
{
    [Neo4jLabel("TestRole")]
    public class TestRole : Neo4jIdentityRole
    {
    }
}
