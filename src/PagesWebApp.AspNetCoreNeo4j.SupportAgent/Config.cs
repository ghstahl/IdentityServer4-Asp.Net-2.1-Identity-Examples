using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace PagesWebApp
{
    public static class IdentityResourcesExtra
    {
        /// <summary>
        /// Models the standard openid scope
        /// </summary>
        /// <seealso cref="IdentityServer4.Models.IdentityResource" />
        public class SupportAgent : IdentityResource
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SupportAgent"/> class.
            /// </summary>
            public SupportAgent()
            {
                Name = "supportagent";
                DisplayName = "Your support agent delegated user identifier";
                Required = true;
            }
        }
    }

    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResourcesExtra.SupportAgent(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API")
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                 
                new Client
                {
                    ClientId = "support",
                    ClientName = "Support Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireConsent = false,
                    RedirectUris =
                    {
                        "https://localhost:44317/signin-support"
                    },
                    PostLogoutRedirectUris =
                    {

                        "https://localhost:44317/Identity/Account/SignoutCallbackOidc"
                    },
                    FrontChannelLogoutSessionRequired = true,
                    FrontChannelLogoutUri = "https://localhost:44317/Identity/Account/SignoutFrontChannel",
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "supportagent"
                    }
                    
                }
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "Alice"),
                        new Claim("website", "https://alice.com")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "Bob"),
                        new Claim("website", "https://bob.com")
                    }
                }
            };
        }
    }
}