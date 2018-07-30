using Neo4jExtras;
using IdentityServer4.Models;

namespace Stores.IdentityServer4.Neo4j
{
    /// <summary>
    /// Represents an IdentityServer4 Client Secret
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.IdentityServer4ClientSecret)]
    public class Neo4jIdentityServer4ClientSecret : Secret
    {
    }
     

}

 