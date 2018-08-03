using System;
using System.Collections.Generic;
using System.Text;

namespace StoresIdentityServer4.Neo4j.Events
{
    public static class Constants
    {
        public static class Categories
        {
            public static string ClientStore => "ClientStore";
            public static string ApiScopeStore => "ApiScopeStore";
            public static string ApiResourceStore => "ApiResourceStore";
            public static string IdentityResourceStore => "IdentityResourceStore";
        }
    }
}
