using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoresIdentityServer4.Neo4j.Test.Models;
using StoresIdentityServer4.Test.Core.Store;

namespace StoresIdentityServer4.Neo4j.Test.Stores
{

    [TestClass]
    public class Neo4jUnitTestUserClientStore :
        UnitTestClientStore<
            TestUser,
            Neo4jIdentityServer4Client,
            Neo4jIdentityServer4ClientSecret,
            Neo4jIdentityServer4ClientGrantType,
            Neo4jIdentityServer4ApiResource,
            Neo4jIdentityServer4ApiResourceClaim,
            Neo4jIdentityServer4ApiSecret,
            Neo4jIdentityServer4ApiScope,
            Neo4jIdentityServer4ApiScopeClaim,
            Neo4jIdentityServer4ClientClaim,
            Neo4jIdentityServer4ClientCorsOrigin,
            Neo4jIdentityServer4ClientScope,
            Neo4jIdentityServer4ClienTIDPRestriction,
            Neo4jIdentityServer4ClientProperty,
            Neo4jIdentityServer4ClientPostLogoutRedirectUri,
            Neo4jIdentityServer4ClientRedirectUri,
            Neo4jIdentityServer4IdentityResource,
            Neo4jIdentityServer4IdentityClaim>
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
                        Neo4jIdentityServer4ApiResource,
                        Neo4jIdentityServer4ApiResourceClaim,
                        Neo4jIdentityServer4ApiSecret,
                        Neo4jIdentityServer4ApiScope,
                        Neo4jIdentityServer4ApiScopeClaim,
                        Neo4jIdentityServer4ClientClaim,
                        Neo4jIdentityServer4ClientCorsOrigin,
                        Neo4jIdentityServer4ClientScope,
                        Neo4jIdentityServer4ClienTIDPRestriction,
                        Neo4jIdentityServer4ClientProperty,
                        Neo4jIdentityServer4ClientPostLogoutRedirectUri,
                        Neo4jIdentityServer4ClientRedirectUri,
                        Neo4jIdentityServer4IdentityResource,
                        Neo4jIdentityServer4IdentityClaim>>(),
                HostContainer.ServiceProvider.GetService<INeo4jTest>())
        {
        }

        protected override Neo4jIdentityServer4ApiResource CreateTestApiResource()
        {
            return new Neo4jIdentityServer4ApiResource()
            {
                Name = Unique.S,
                Description = Unique.S,
                DisplayName = Unique.S,
                Enabled = true
            };
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

                ExpirationJson = DateTime.UtcNow.AddDays(1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"),
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

        protected override Neo4jIdentityServer4ClienTIDPRestriction CreateTesTIDPRestriction()
        {
            return new Neo4jIdentityServer4ClienTIDPRestriction()
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

        protected override Neo4jIdentityServer4ApiScope CreateTestApiScope()
        {
            return new Neo4jIdentityServer4ApiScope()
            {
                Name = Unique.S,
                Description = Unique.S,
                DisplayName = Unique.S,
                Emphasize = true,Required = true,ShowInDiscoveryDocument = true
            };
        }

        protected override Neo4jIdentityServer4ApiScopeClaim CreateTestApiScopeClaim()
        {
            return new Neo4jIdentityServer4ApiScopeClaim()
            {
                 Type = Unique.S
            };
        }

        protected override Neo4jIdentityServer4ApiSecret CreateTestApiSecret()
        {
            return new Neo4jIdentityServer4ApiSecret()
            {
                Value = Unique.S.Sha256(),
                Description = Unique.S,

                ExpirationJson = DateTime.UtcNow.AddDays(1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"),
                Type = IdentityServerConstants.SecretTypes.SharedSecret
            };
        }

        protected override Neo4jIdentityServer4ApiResourceClaim CreateTestApiResourceClaim()
        {
            return new Neo4jIdentityServer4ApiResourceClaim()
            {
                Type = Unique.S
            };
        }

        protected override Neo4jIdentityServer4IdentityResource CreateTestIdentityResource()
        {
            return new Neo4jIdentityServer4IdentityResource()
            {
                Name = Unique.S,
                Description = Unique.S,
                DisplayName = Unique.S,
                Emphasize = true,
                Enabled = true,
                Required = true,
                ShowInDiscoveryDocument = true
            };
        }

        protected override Neo4jIdentityServer4IdentityClaim CreateTestIdentityClaim()
        {
            return new Neo4jIdentityServer4IdentityClaim()
            {
                Type = Unique.S
            };
        }
    }
}
