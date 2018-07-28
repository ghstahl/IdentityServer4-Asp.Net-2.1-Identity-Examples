using Neo4jExtras;
using System;
using IdentityServer4.Models;
using IdentityServer4Extras;
using Stores.IdentityServer4.Neo4j.Entities;

namespace Stores.IdentityServer4.Neo4j
{
    /// <summary>
    /// Represents an IdentityServer4 Client Secret
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.IdentityServer4ClientSecret)]
    public class Neo4jIdentityServer4ClientSecret : Secret
    {
    }

    /// <summary>
    /// Represents an IdentityServer4 Client
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.IdentityServer4Client)]
    public class Neo4jIdentityServer4Client : ClientRoot
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Neo4jIdentityServer4Client"/>.
        /// </summary>
        /// <remarks>
        /// The Id property is initialized to form a new GUID string value.
        /// </remarks>
        public Neo4jIdentityServer4Client()
        {
            
        }

    }
}
