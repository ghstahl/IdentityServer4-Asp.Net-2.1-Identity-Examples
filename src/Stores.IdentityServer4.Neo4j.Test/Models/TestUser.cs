using AspNetCore.Identity.Neo4j;

namespace Stores.IdentityServer4.Neo4j.Test.Models
{
    [Neo4jLabel("TestUser")]
    public class TestUser : Neo4jIdentityUser
    {
    }
}
