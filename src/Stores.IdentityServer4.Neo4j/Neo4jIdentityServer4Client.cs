using Neo4jExtras;
using System;
using IdentityServer4.Models;
using IdentityServer4Extras;
using Stores.IdentityServer4.Neo4j.Entities;

namespace Stores.IdentityServer4.Neo4j
{
    /// <summary>
    /// Represents an IdentityServer4 Client
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.IdentityServer4Client)]
    public class Neo4jIdentityServer4Client : ClientRoot
    {
    }

    /// <summary>
    /// Represents an IdentityServer4 Client Secret
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.IdentityServer4ClientSecret)]
    public class Neo4jIdentityServer4ClientSecret : Secret
    {
    }

    [Neo4jLabel(Neo4jConstants.Labels.IdentityServer4ClientGrantType)]
    public class Neo4jIdentityServer4ClientGrantType : ClientGrantType
    {
    }
    
    

    [Neo4jLabel(Neo4jConstants.Labels.IdentityServer4ClientClaim)]
    public class Neo4jIdentityServer4ClientClaim : ClientClaim
    {
    }

    [Neo4jLabel(Neo4jConstants.Labels.IdentityServer4ClientCorsOrigin)]
    public class Neo4jIdentityServer4ClientCorsOrigin : ClientCorsOrigin
    {
    }
    [Neo4jLabel(Neo4jConstants.Labels.IdentityServer4ClientScope)]
    public class Neo4jIdentityServer4ClientScope : ClientScope
    {
    }
    [Neo4jLabel(Neo4jConstants.Labels.IdentityServer4ClientIdPRestriction)]
    public class Neo4jIdentityServer4ClientIdPRestriction : ClientIdPRestriction
    {
    }
    [Neo4jLabel(Neo4jConstants.Labels.IdentityServer4ClientProperty)]
    public class Neo4jIdentityServer4ClientProperty : ClientProperty
    {
    }
    [Neo4jLabel(Neo4jConstants.Labels.IdentityServer4ClientPostLogoutRedirectUri)]
    public class Neo4jIdentityServer4ClientPostLogoutRedirectUri : ClientPostLogoutRedirectUri
    {
    }
    [Neo4jLabel(Neo4jConstants.Labels.IdentityServer4ClientRedirectUri)]
    public class Neo4jIdentityServer4ClientRedirectUri : ClientRedirectUri
    {
    }
     

}

 