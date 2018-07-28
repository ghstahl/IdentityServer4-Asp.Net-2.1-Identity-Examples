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
    public class Neo4jUnitTestMultiFactorStore : UnitTestClientStore<TestUser, Neo4jIdentityServer4Client>
    {


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

        public Neo4jUnitTestMultiFactorStore() : 
            base(
                HostContainer.ServiceProvider.GetService<IUserStore<TestUser>>(),
                HostContainer.ServiceProvider.GetService<IIdentityServer4ClientUserStore<TestUser, Neo4jIdentityServer4Client>>(),
                HostContainer.ServiceProvider.GetService<INeo4jTest>())
        {
        }
    }
}
