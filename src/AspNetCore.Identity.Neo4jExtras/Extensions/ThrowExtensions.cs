using System;

namespace AspNetCore.Identity.Neo4j
{
    public static class ThrowExtensions
    {
        public static void ThrowIfNull<TClass>(this TClass obj, string paramName)
        where TClass:class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
        public static void ThrowIfNullOrEmpty(this string obj, string paramName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
        public static void ThrowIfNullOrWhiteSpace(this string obj, string paramName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}