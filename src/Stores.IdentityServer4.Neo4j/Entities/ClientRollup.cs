using Newtonsoft.Json;

namespace StoresIdentityServer4.Neo4j.Entities
{
    public class ClientRollup
    {
        public string ClientJson { get; set; }
        // System claims can't bet deserialized, so we will serialize our own claims.
        public string ClaimsJson { get; set; }
    }
}