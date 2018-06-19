using System;
using System.Collections.Generic;

namespace P7.AspNetCore.Identity.InMemory
{
    /// <summary>
    ///     Represents a Role entity
    /// </summary>
    public class MemoryRole : MemoryRole<string>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public MemoryRole()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="roleName"></param>
        public MemoryRole(string roleName) : this()
        {
            Name = roleName;
        }
    }

    /// <summary>
    ///     Represents a Role entity
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class MemoryRole<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public MemoryRole() { }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="roleName"></param>
        public MemoryRole(string roleName) : this()
        {
            Name = roleName;
        }

        /// <summary>
        ///     Role id
        /// </summary>
        public virtual TKey Id { get; set; }

        /// <summary>
        /// Navigation property for claims in the role
        /// </summary>
        public virtual ICollection<MemoryRoleClaim<TKey>> Claims { get; private set; } = new List<MemoryRoleClaim<TKey>>();

        /// <summary>
        ///     Role name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Normalized name used for equality
        /// </summary>
        public virtual string NormalizedName { get; set; }

        /// <summary>
        /// A random value that should change whenever a role is persisted to the store
        /// </summary>
        public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }
}