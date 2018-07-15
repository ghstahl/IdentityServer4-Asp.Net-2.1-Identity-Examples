namespace AspNetCore.Identity.Neo4j
{
    /// <summary>
    /// Represents an authentication token for a user.
    /// </summary>
    [Neo4jLabel("Token")]
    public class Neo4jIdentityUserToken
    {
        /// <summary>
        /// Gets or sets the LoginProvider this token is from.
        /// </summary>
        public virtual string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the name of the token.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the token value.
        /// </summary>
        public virtual string Value { get; set; }
    }
}