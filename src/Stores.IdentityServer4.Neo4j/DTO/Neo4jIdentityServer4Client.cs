using Neo4jExtras;
using System;
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
}

 