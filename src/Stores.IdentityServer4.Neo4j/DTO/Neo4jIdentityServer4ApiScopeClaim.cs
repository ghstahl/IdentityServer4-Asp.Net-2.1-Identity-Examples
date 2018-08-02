using Neo4jExtras;

namespace StoresIdentityServer4.Neo4j
{
    /// <summary>
    /// Represents an IdentityServer4 ApiScope
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.ApiScopeClaim)]
    public class Neo4jIdentityServer4ApiScopeClaim :
        StoresIdentityServer4.Neo4j.Entities.ApiScopeClaim
    {
    }
}

