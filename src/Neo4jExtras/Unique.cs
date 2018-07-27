using System;

namespace Neo4jExtras
{
    public static class Unique
    {
        public static string G => Guid.NewGuid().ToString();
        public static string S => Guid.NewGuid().ToString("N");
        public static string Url => $"https://{S}.domain.com";
        public static string Email => $"{S}@domain.com";
    }
}
