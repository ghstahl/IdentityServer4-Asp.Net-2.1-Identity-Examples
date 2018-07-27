using Neo4jExtras;
using System;
using IdentityServer4Extras;

namespace Stores.IdentityServer4.Neo4j
{
    /// <summary>
    /// Represents an IdentityServer4 Client
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.IdentityServer4Client)]
    public class Neo4jIdentityServer4Client : ClientExtra
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Neo4jIdentityRole"/>.
        /// </summary>
        /// <remarks>
        /// The Id property is initialized to form a new GUID string value.
        /// </remarks>
        public Neo4jIdentityServer4Client()
        {
            Id = Unique.G;
        }

        public string Id { get; set; }
    }
}
