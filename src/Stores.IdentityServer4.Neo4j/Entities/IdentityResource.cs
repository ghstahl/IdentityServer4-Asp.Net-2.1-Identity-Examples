using System.Collections.Generic;
using Newtonsoft.Json;

namespace StoresIdentityServer4.Neo4j.Entities
{
    public class IdentityResource
    {
        public bool Enabled { get; set; } = true;
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        //[JsonIgnore]
      //  public List<IdentityClaim> UserClaims { get; set; }
    }
}