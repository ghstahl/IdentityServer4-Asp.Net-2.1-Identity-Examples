﻿using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace PagesWebApp
{
    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API"),
                new ApiResource("native_api", "Native Client API")
                {
                    ApiSecrets =
                    {
                        new Secret("native_api_secret".Sha256())
                    }
                }
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
                    ClientId = "native.code",
                    ClientName = "Native Client (Code with PKCE)",

                   RedirectUris = {"http://notreal"},
                  PostLogoutRedirectUris = {"http://notreal"},

                    RequireClientSecret = false,
                    
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    AllowedScopes = {"openid", "profile", "native_api"},

                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse
                },
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {"api1"}
                },

                // resource owner password grant client
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {"api1"}
                },

                // OpenID Connect hybrid flow and client credentials client (MVC)
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    RequireConsent = false,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = {"http://localhost:5002/signin-oidc"},
                    PostLogoutRedirectUris = {"http://localhost:5002/signout-callback-oidc"},

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },
                    AllowOfflineAccess = true
                },
                new Client
                {
                    ClientId = "mvc2",
                    ClientName = "MVC2 Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireConsent = true,
                    RedirectUris =
                    {
                        "https://localhost:44343/signin-oidc-pages-webapp"
                    },
                    PostLogoutRedirectUris =
                    {

                        "https://localhost:44343/Account/SignoutCallbackOidc"
                    },
                    FrontChannelLogoutSessionRequired = true,
                    FrontChannelLogoutUri = "https://localhost:44343/Account/SignoutFrontChannel",
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },
                new Client
                {
                    ClientId = "PagesWebAppClient",
                    ClientName = "PagesWebAppClient Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireConsent = true,
                    RedirectUris =
                    {
                        "https://localhost:44307/signin-oidc-pages-webapp-client"
                    },
                    PostLogoutRedirectUris =
                    {

                        "https://localhost:44307/Identity/Account/SignoutCallbackOidc"
                    },
                    FrontChannelLogoutSessionRequired = true,
                    FrontChannelLogoutUri = "https://localhost:44307/Identity/Account/SignoutFrontChannel",
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },
                new Client
                {
                    ClientId = "Neo4j.PagesWebAppClient.NoUserDatabase",
                    ClientName = "Neo4j.PagesWebAppClient.NoUserDatabase Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireConsent = true,
                    RedirectUris =
                    {
                        "https://localhost:44308/signin-oidc-Neo4j"
                    },
                    PostLogoutRedirectUris =
                    {

                        "https://localhost:44308/Identity/Account/SignoutCallbackOidc"
                    },
                    FrontChannelLogoutSessionRequired = true,
                    FrontChannelLogoutUri = "https://localhost:44308/Identity/Account/SignoutFrontChannel",
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
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