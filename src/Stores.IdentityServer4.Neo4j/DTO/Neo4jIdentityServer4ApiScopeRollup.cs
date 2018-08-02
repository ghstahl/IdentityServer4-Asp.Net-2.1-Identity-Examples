using Neo4jExtras;
using StoresIdentityServer4.Neo4j.Entities;

namespace StoresIdentityServer4.Neo4j
{
    /// <summary>
    /// Represents an IdentityServer4 ApiScopeRollup
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.ApiScopeRollup)]
    public class Neo4jIdentityServer4ApiScopeRollup : ApiScopeRollup
    {
    }
}