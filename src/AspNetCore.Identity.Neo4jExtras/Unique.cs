using System;

namespace AspNetCore.Identity.Neo4j
{
    static public class Unique
    {
        public static string G => Guid.NewGuid().ToString();
        public static string S => Guid.NewGuid().ToString("N");
        public static string Url => $"https://{S}.domain.com";
        public static string Email => $"{S}@domain.com";
    }
}
