using AspNetCore.Identity.Neo4j;
using Neo4jExtras;

namespace Stores.IdentityServer4.Neo4j.Test.Models
{
    [Neo4jLabel("TestUser")]
    public class TestUser : Neo4jIdentityUser
    {
    }
}
