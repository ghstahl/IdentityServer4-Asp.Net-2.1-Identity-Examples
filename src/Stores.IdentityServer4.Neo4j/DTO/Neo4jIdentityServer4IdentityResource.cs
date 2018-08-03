using Neo4jExtras;

namespace StoresIdentityServer4.Neo4j
{
    /// <summary>
    /// Represents an IdentityServer4 IdentityResource
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.IdentityResource)]
    public class Neo4jIdentityServer4IdentityResource : StoresIdentityServer4.Neo4j.Entities.IdentityResource
    {
    }
}