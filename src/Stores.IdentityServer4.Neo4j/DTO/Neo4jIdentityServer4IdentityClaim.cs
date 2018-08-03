using Neo4jExtras;

namespace StoresIdentityServer4.Neo4j
{
    /// <summary>
    /// Represents an IdentityServer4 IdentityClaim
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.IdentityClaim)]
    public class Neo4jIdentityServer4IdentityClaim : StoresIdentityServer4.Neo4j.Entities.IdentityClaim
    {
    }
}