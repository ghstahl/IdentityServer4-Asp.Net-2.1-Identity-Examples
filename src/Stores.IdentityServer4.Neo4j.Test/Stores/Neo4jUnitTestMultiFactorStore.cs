using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AspNetCore.Identity.Neo4j;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Stores.IdentityServer4.Neo4j;
using Stores.IdentityServer4.Neo4j.Test;

using Stores.IdentityServer4.Neo4j.Test.Models;
using Stores.IdentityServer4.Test.Core.Store;

namespace AspNetCore.Identity.MultiFactor.Test.Neo4j
{
    [TestClass]
    public class Neo4jUnitTestUserClientStore2 :
        UnitTestClientStore2<
            TestUser,
            Neo4jIdentityServer4Client,
            Neo4jIdentityServer4ClientGrantType>
    {
        public Neo4jUnitTestUserClientStore2() :
            base(
                HostContainer.ServiceProvider.GetService<IUserStore<TestUser>>(),
                HostContainer.ServiceProvider.GetService<
                    IIdentityServer4ClientUserStore2<
                        TestUser,
                        Neo4jIdentityServer4Client,
                        Neo4jIdentityServer4ClientGrantType>>(),
                HostContainer.ServiceProvider.GetService<INeo4jTest>())
        {
        }

        protected override TestUser CreateTestUser()
        {
            return new TestUser()
            {
                UserName = Unique.S,
                Email = Unique.Email
            };
        }

        protected override Neo4jIdentityServer4Client CreateTestClient()
        {
            return new Neo4jIdentityServer4Client()
            {
                ClientId = Unique.G,
                ClientName = Unique.S,
                AllowOfflineAccess = true,
                RequireClientSecret = true,
                AllowArbitraryLocalRedirectUris = true,
                AbsoluteRefreshTokenLifetime = 3600,
                AccessTokenLifetime = 3600,
                AccessTokenType = (int)AccessTokenType.Jwt,
                AllowAccessTokensViaBrowser = true,
                AllowPlainTextPkce = true,
                AllowRememberConsent = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                AlwaysSendClientClaims = true,
                AuthorizationCodeLifetime = 3600,
                BackChannelLogoutSessionRequired = true,
                BackChannelLogoutUri = Unique.Url,
                ClientClaimsPrefix = Unique.S,
                ClientUri = Unique.Url,
                ConsentLifetime = 3600,
                Description = Unique.S,
                EnableLocalLogin = true,
                Enabled = true,
                FrontChannelLogoutSessionRequired = true,
                FrontChannelLogoutUri = Unique.Url,
                IdentityTokenLifetime = 3600,
                IncludeJwtId = true,
                LogoUri = Unique.Url,
                PairWiseSubjectSalt = Unique.S,
                ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                RefreshTokenExpiration = 3600,
                RefreshTokenUsage = (int)TokenUsage.OneTimeOnly,
                RequireConsent = true,
                RequirePkce = true,
                RequireRefreshClientSecret = true,
                SlidingRefreshTokenLifetime = 3600,
                UpdateAccessTokenClaimsOnRefresh = true
            };
        }

        protected override Neo4jIdentityServer4ClientGrantType CreateTestGrantType()
        {
            return new Neo4jIdentityServer4ClientGrantType()
            {
                GrantType = Unique.S
            };
        }
    }

    [TestClass]
    public class Neo4jUnitTestUserClientStore :
        UnitTestClientStore<
            TestUser,
            Neo4jIdentityServer4Client,
            Neo4jIdentityServer4ClientSecret,
            Neo4jIdentityServer4ClientGrantType,
            Neo4jIdentityServer4ClientClaim,
            Neo4jIdentityServer4ClientCorsOrigin,
            Neo4jIdentityServer4ClientScope,
            Neo4jIdentityServer4ClientIDPRestriction,
            Neo4jIdentityServer4ClientProperty,
            Neo4jIdentityServer4ClientPostLogoutRedirectUri,
            Neo4jIdentityServer4ClientRedirectUri>
    {
        public Neo4jUnitTestUserClientStore() :
            base(
                HostContainer.ServiceProvider.GetService<IUserStore<TestUser>>(),
                HostContainer.ServiceProvider.GetService<
                    IIdentityServer4ClientUserStore<
                        TestUser,
                        Neo4jIdentityServer4Client,
                        Neo4jIdentityServer4ClientSecret,
                        Neo4jIdentityServer4ClientGrantType,
                        Neo4jIdentityServer4ClientClaim,
                        Neo4jIdentityServer4ClientCorsOrigin,
                        Neo4jIdentityServer4ClientScope,
                        Neo4jIdentityServer4ClientIDPRestriction,
                        Neo4jIdentityServer4ClientProperty,
                        Neo4jIdentityServer4ClientPostLogoutRedirectUri,
                        Neo4jIdentityServer4ClientRedirectUri>>(),
                HostContainer.ServiceProvider.GetService<INeo4jTest>())
        {
        }

        protected override TestUser CreateTestUser()
        {
            return new TestUser()
            {
                UserName = Unique.S,
                Email = Unique.Email
            };
        }

        protected override Neo4jIdentityServer4ClientGrantType CreateTestGrantType()
        {
            return new Neo4jIdentityServer4ClientGrantType()
            {
                GrantType = Unique.S
            };
        }

        protected override Neo4jIdentityServer4ClientSecret CreateTestSecret()
        {
            return new Neo4jIdentityServer4ClientSecret()
            {
                Value = Unique.S.Sha256(),
                Description = Unique.S,

                Expiration = DateTime.Now.AddDays(1),
                Type = IdentityServerConstants.SecretTypes.SharedSecret
            };
        }

        protected override Neo4jIdentityServer4ClientClaim CreateTestClaim()
        {
            return new Neo4jIdentityServer4ClientClaim()
            {
                Type = Unique.S,
                Value = Unique.S
            };
        }

        protected override Neo4jIdentityServer4ClientScope CreateTestScope()
        {
            return new Neo4jIdentityServer4ClientScope()
            {
                Scope = Unique.S
            };
        }

        protected override Neo4jIdentityServer4ClientCorsOrigin CreateTestCorsOrigin()
        {
            return new Neo4jIdentityServer4ClientCorsOrigin()
            {
                Origin = Unique.Url
            };
        }

        protected override Neo4jIdentityServer4ClientPostLogoutRedirectUri CreatePostLogoutRedirectUri()
        {
            return new Neo4jIdentityServer4ClientPostLogoutRedirectUri()
            {
                PostLogoutRedirectUri = Unique.Url
            };
        }

        protected override Neo4jIdentityServer4ClientRedirectUri CreateTestRedirectUri()
        {
            return new Neo4jIdentityServer4ClientRedirectUri()
            {
                RedirectUri = Unique.Url
            };
        }

        protected override Neo4jIdentityServer4ClientProperty CreateTestProperty()
        {
            return new Neo4jIdentityServer4ClientProperty()
            {
                Key = Unique.S,
                Value = Unique.S
            };
        }

        protected override Neo4jIdentityServer4ClientIDPRestriction CreateTestIdpRestriction()
        {
            return new Neo4jIdentityServer4ClientIDPRestriction()
            {
                Provider = Unique.S
            };
        }

        protected override Neo4jIdentityServer4Client CreateTestClient()
        {
            return new Neo4jIdentityServer4Client()
            {
                ClientId = Unique.G,
                ClientName = Unique.S,
                AllowOfflineAccess = true,
                RequireClientSecret = true,
                AllowArbitraryLocalRedirectUris = true,
                AbsoluteRefreshTokenLifetime = 3600,
                AccessTokenLifetime = 3600,
                AccessTokenType = (int) AccessTokenType.Jwt,
                AllowAccessTokensViaBrowser = true,
                AllowPlainTextPkce = true,
                AllowRememberConsent = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                AlwaysSendClientClaims = true,
                AuthorizationCodeLifetime = 3600,
                BackChannelLogoutSessionRequired = true,
                BackChannelLogoutUri = Unique.Url,
                ClientClaimsPrefix = Unique.S,
                ClientUri = Unique.Url,
                ConsentLifetime = 3600,
                Description = Unique.S,
                EnableLocalLogin = true,
                Enabled = true,
                FrontChannelLogoutSessionRequired = true,
                FrontChannelLogoutUri = Unique.Url,
                IdentityTokenLifetime = 3600,
                IncludeJwtId = true,
                LogoUri = Unique.Url,
                PairWiseSubjectSalt = Unique.S,
                ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                RefreshTokenExpiration = 3600,
                RefreshTokenUsage = (int) TokenUsage.OneTimeOnly,
                RequireConsent = true,
                RequirePkce = true,
                RequireRefreshClientSecret = true,
                SlidingRefreshTokenLifetime = 3600,
                UpdateAccessTokenClaimsOnRefresh = true
            };
        }



    }
}
