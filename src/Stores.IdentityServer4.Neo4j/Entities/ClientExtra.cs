namespace StoresIdentityServer4.Neo4j.Entities
{
    public class ClientExtra : StoresIdentityServer4.Neo4j.Entities.Client
    {
        public bool RequireRefreshClientSecret { get; set; }
        public bool AllowArbitraryLocalRedirectUris { get; set; }
    }
}