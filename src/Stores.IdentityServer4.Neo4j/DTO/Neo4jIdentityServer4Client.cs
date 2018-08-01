using Neo4jExtras;
using System;
using IdentityServer4Extras;
using StoresIdentityServer4.Neo4j.Entities;

namespace StoresIdentityServer4.Neo4j
{
    /// <summary>
    /// Represents an IdentityServer4 Client
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.Client)]
    public class Neo4jIdentityServer4Client : Client
    {
    }
}

 