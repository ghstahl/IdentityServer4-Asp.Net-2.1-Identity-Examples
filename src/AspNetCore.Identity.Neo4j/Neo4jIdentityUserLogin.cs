using Neo4jExtras;

namespace AspNetCore.Identity.Neo4j
{
    /// <summary>
    /// Represents a login and its associated provider for a user.
    /// </summary>
    [Neo4jLabel("Login")]
    public class Neo4jIdentityUserLogin
    {
        /// <summary>
        /// Gets or sets the login provider for the login (e.g. facebook, google)
        /// </summary>
        public virtual string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the unique provider identifier for this login.
        /// </summary>
        public virtual string ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the friendly name used in a UI for this login.
        /// </summary>
        public virtual string ProviderDisplayName { get; set; }
    }
}