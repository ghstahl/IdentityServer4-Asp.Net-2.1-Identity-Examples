﻿using System;

namespace P7.AspNetCore.Identity.InMemory
{
    /// <summary>
    ///     EntityType that represents one specific role claim
    /// </summary>
    public class MemoryRoleClaim : MemoryRoleClaim<string> { }

    /// <summary>
    ///     EntityType that represents one specific role claim
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class MemoryRoleClaim<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        ///     Primary key
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        ///     User Id for the role this claim belongs to
        /// </summary>
        public virtual TKey RoleId { get; set; }

        /// <summary>
        ///     Claim type
        /// </summary>
        public virtual string ClaimType { get; set; }

        /// <summary>
        ///     Claim value
        /// </summary>
        public virtual string ClaimValue { get; set; }
    }
}