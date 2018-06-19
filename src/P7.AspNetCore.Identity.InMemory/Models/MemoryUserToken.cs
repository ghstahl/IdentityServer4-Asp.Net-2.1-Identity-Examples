using System;

namespace P7.AspNetCore.Identity.InMemory
{
    /// <summary>
    /// Entity type for a user's token
    /// </summary>
    public class MemoryUserToken : MemoryUserToken<string> { }
    /// <summary>
    /// Entity type for a user's token
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class MemoryUserToken<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        ///     The login provider for the login (i.e. facebook, google)
        /// </summary>
        public virtual string LoginProvider { get; set; }

        /// <summary>
        ///     Key representing the login for the provider
        /// </summary>
        public virtual string TokenName { get; set; }

        /// <summary>
        ///     Display name for the login
        /// </summary>
        public virtual string TokenValue { get; set; }

        /// <summary>
        ///     User Id for the user who owns this login
        /// </summary>
        public virtual TKey UserId { get; set; }
    }
}