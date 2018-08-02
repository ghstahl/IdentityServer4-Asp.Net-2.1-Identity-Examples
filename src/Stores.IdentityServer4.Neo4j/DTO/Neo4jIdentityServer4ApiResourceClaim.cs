using Neo4jExtras;

namespace StoresIdentityServer4.Neo4j
{
    /// <summary>
    /// Represents an IdentityServer4 ApiResourceClaim
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.ApiResourceClaim)]
    public class Neo4jIdentityServer4ApiResourceClaim : 
        StoresIdentityServer4.Neo4j.Entities.ApiResourceClaim
    {
    }
}

