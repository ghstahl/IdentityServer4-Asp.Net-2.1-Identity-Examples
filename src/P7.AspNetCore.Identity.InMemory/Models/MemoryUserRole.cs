using System;

namespace P7.AspNetCore.Identity.InMemory
{
    /// <summary>
    ///     EntityType that represents a user belonging to a role
    /// </summary>
    public class MemoryUserRole : MemoryUserRole<string> { }
    /// <summary>
    ///     EntityType that represents a user belonging to a role
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class MemoryUserRole<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        ///     UserId for the user that is in the role
        /// </summary>
        public virtual TKey UserId { get; set; }

        /// <summary>
        ///     RoleId for the role
        /// </summary>
        public virtual TKey RoleId { get; set; }
    }
}