using Neo4jExtras;

namespace StoresIdentityServer4.Neo4j
{
    /// <summary>
    /// Represents an IdentityServer4 ApiResource
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.ApiResource)]
    public class Neo4jIdentityServer4ApiResource : StoresIdentityServer4.Neo4j.Entities.ApiResource
    {
    }
}

