using Neo4jExtras;

namespace StoresIdentityServer4.Neo4j
{
    /// <summary>
    /// Represents an IdentityServer4 ApiSecret
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.ApiSecret)]
    public class Neo4jIdentityServer4ApiSecret :
        StoresIdentityServer4.Neo4j.Entities.ApiSecret
    {
    }
}

