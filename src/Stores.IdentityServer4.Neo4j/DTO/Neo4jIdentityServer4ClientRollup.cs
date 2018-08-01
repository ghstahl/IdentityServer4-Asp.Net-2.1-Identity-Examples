using Neo4jExtras;
using StoresIdentityServer4.Neo4j.Entities;

namespace StoresIdentityServer4.Neo4j
{
    /// <summary>
    /// Represents an IdentityServer4 ClientRollup
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.ClientRollup)]
    public class Neo4jIdentityServer4ClientRollup : ClientRollup
    {
    }
}