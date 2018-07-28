using IdentityServer4.Models;

namespace IdentityServer4Extras
{
    public class ClientExtra : Client
    {
        private bool? _requireRefreshClientSecret;

        //
        // Summary:
        //     If set to false, no client secret is needed to refresh tokens at the token endpoint
        //     (defaults to RequireClientSecret)
        public bool RequireRefreshClientSecret
        {
            get
            {
                if (_requireRefreshClientSecret == null || RequireClientSecret == false)
                    return RequireClientSecret;
                return (bool)_requireRefreshClientSecret;
            }
            set => _requireRefreshClientSecret = value;
        }

        private bool? _allowArbitraryLocalRedirectUris;

        //
        // Summary:
        //     If set to true, allows arbitrary redirect URIs 
        //     (defaults to RequireClientSecret)
        public bool AllowArbitraryLocalRedirectUris
        {
            get
            {
                if (_allowArbitraryLocalRedirectUris == null)
                    return false;
                return (bool) _allowArbitraryLocalRedirectUris;
            }
            set => _allowArbitraryLocalRedirectUris = value;
        }
    }
}