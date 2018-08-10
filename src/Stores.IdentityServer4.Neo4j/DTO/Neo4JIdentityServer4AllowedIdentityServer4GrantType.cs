using Neo4jExtras;
using StoresIdentityServer4.Neo4j.Entities;

namespace StoresIdentityServer4.Neo4j
{
    [Neo4jLabel(Neo4jConstants.Labels.AllowedGrantType)]
    public class Neo4JIdentityServer4AllowedIdentityServer4GrantType : IdentityServer4GrantType
    {
    }
}