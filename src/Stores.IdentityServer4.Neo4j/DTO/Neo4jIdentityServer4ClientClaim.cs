using Neo4jExtras;
using Stores.IdentityServer4.Neo4j.Entities;

namespace Stores.IdentityServer4.Neo4j
{
    [Neo4jLabel(Neo4jConstants.Labels.IdentityServer4ClientClaim)]
    public class Neo4jIdentityServer4ClientClaim : ClientClaim
    {
    }
     

}

 