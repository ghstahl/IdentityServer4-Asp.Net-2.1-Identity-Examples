using System;
using System.Collections.Generic;
using System.Text;

namespace Stores.IdentityServer4.Neo4j
{
    /// <summary>
    ///     Represents the result of an identity operation
    /// </summary>
    public class IdentityServer4Result
    {
        private static readonly IdentityServer4Result _success = new IdentityServer4Result(true);

        /// <summary>
        ///     Failure constructor that takes error messages
        /// </summary>
        /// <param name="errors"></param>
        public IdentityServer4Result(params string[] errors) : this((IEnumerable<string>)errors)
        {
        }

        /// <summary>
        ///     Failure constructor that takes error messages
        /// </summary>
        /// <param name="errors"></param>
        public IdentityServer4Result(IEnumerable<string> errors)
        {
            if (errors == null)
            {
                errors = new[] { "An error has occurred" };
            }
            Succeeded = false;
            Errors = errors;
        }

        /// <summary>
        /// Constructor that takes whether the result is successful
        /// </summary>
        /// <param name="success"></param>
        protected IdentityServer4Result(bool success)
        {
            Succeeded = success;
            Errors = new string[0];
        }

        /// <summary>
        ///     True if the operation was successful
        /// </summary>
        public bool Succeeded { get; private set; }

        /// <summary>
        ///     List of errors
        /// </summary>
        public IEnumerable<string> Errors { get; private set; }

        /// <summary>
        ///     Static success result
        /// </summary>
        /// <returns></returns>
        public static IdentityServer4Result Success
        {
            get { return _success; }
        }

        /// <summary>
        ///     Failed helper method
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static IdentityServer4Result Failed(params string[] errors)
        {
            return new IdentityServer4Result(errors);
        }
    }
}
