namespace StoresIdentityServer4.Neo4j.Entities
{
    public class ApiScope
    {
        public string Name { get; set; }  //  required
        private string _displayName;
        public string DisplayName
        {
            get => _displayName ?? "";
            set { _displayName = value; }
        }
        private string _description;
        public string Description
        {
            get => _description ?? "";
            set { _description = value; }
        }

        public bool Required{ get; set; }

        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        //     public List<ApiScopeClaim> UserClaims { get; set; }
    }
}