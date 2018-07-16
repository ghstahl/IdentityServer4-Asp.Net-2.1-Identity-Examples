using System;

namespace AspNetCore.Identity.MultiFactor.Test.Core
{
    static class Unique
    {
        public static string G => Guid.NewGuid().ToString();
        public static string S => Guid.NewGuid().ToString("N");
        public static string Url => $"https://{S}.domain.com";
     }
}
