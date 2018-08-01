using Neo4jExtras;
using StoresIdentityServer4.Neo4j.Entities;

namespace StoresIdentityServer4.Neo4j
{
    [Neo4jLabel(Neo4jConstants.Labels.GrantType)]
    public class Neo4jIdentityServer4ClientGrantType : ClientGrantType
    {
    }
     

}

 