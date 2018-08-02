using Neo4jExtras;

namespace StoresIdentityServer4.Neo4j
{
    /// <summary>
    /// Represents an IdentityServer4 ApiScope
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.ApiScope)]
    public class Neo4jIdentityServer4ApiScope :
        StoresIdentityServer4.Neo4j.Entities.ApiScope
    {
    }
}

