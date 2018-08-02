namespace StoresIdentityServer4.Neo4j.Entities
{
    public class ApiResource
    {
        public bool Enabled { get; set; } = true;
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
      //  public List<ApiSecret> Secrets { get; set; }
       // public List<ApiScope> Scopes { get; set; }
     //   public List<ApiResourceClaim> UserClaims { get; set; }
    }
}
