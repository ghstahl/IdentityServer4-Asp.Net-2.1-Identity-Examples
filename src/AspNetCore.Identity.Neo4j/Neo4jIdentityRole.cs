using System;
using AspNetCore.Identity.Neo4j.Internal;

namespace AspNetCore.Identity.Neo4j
{
    /// <summary>
    /// Represents a role in the identity system
    /// </summary>
    [Neo4jLabel(Neo4jConstants.Labels.Role)]
    public class Neo4jIdentityRole
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Neo4jIdentityRole"/>.
        /// </summary>
        /// <remarks>
        /// The Id property is initialized to form a new GUID string value.
        /// </remarks>
        public Neo4jIdentityRole()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Neo4jIdentityRole"/>.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        public Neo4jIdentityRole(string roleName) : this()
        {
            Name = roleName;
        }

        /// <summary>
        /// Gets or sets the primary key for this role.
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Gets or sets the name for this role.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the normalized name for this role.
        /// </summary>
        public virtual string NormalizedName { get; set; }

        /// <summary>
        /// Returns the name of the role.
        /// </summary>
        /// <returns>The name of the role.</returns>
        public override string ToString() => Name;
    }
}
