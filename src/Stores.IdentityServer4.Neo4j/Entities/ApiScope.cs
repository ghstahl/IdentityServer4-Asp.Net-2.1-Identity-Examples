﻿namespace StoresIdentityServer4.Neo4j.Entities
{
    public class ApiScope
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        //     public List<ApiScopeClaim> UserClaims { get; set; }
    }
}