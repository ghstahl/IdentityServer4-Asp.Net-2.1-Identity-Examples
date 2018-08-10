using Neo4jExtras;
using StoresIdentityServer4.Neo4j.Entities;

namespace StoresIdentityServer4.Neo4j
{
    /// <summary>
    /// Represents an IdentityServer4 IdentityResourcesRollup
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.IdentityResourcesRollup)]
    public class Neo4jIdentityServer4IdentityResourcesRollup : IdentityResourcesRollup
    {
        public string Name { get; set; }
    }
}