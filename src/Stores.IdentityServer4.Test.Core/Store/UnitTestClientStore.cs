using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4Extras;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using StoresIdentityServer4.Neo4j;
using StoresIdentityServer4.Neo4j.DTO.Mappers;
using StoresIdentityServer4.Neo4j.Entities;
using StoresIdentityServer4.Neo4j.Mappers;
using Client = IdentityServer4.Models.Client;
using ClientExtra = IdentityServer4.Models.ClientExtra;
using Secret = IdentityServer4.Models.Secret;


namespace StoresIdentityServer4.Test.Core.Store
{
    public abstract class UnitTestClientStore<
        TUser,
        TClient,
        TSecret,
        TGrantType,
        TApiResource,
        TApiResourceClaim,
        TApiSecret,
        TApiScope,
        TApiScopeClaim,
        TClaim,
        TCorsOrigin,
        TScope,
        TIDPRestriction,
        TProperty,
        TPostLogoutRedirectUri,
        TRedirectUri,
        TIdentityResource,
        TIdentityClaim>
        where TUser : Neo4jIdentityUser
        where TClient : StoresIdentityServer4.Neo4j.Entities.ClientExtra
        where TSecret : StoresIdentityServer4.Neo4j.Entities.Secret
        where TGrantType : StoresIdentityServer4.Neo4j.Entities.ClientGrantType
        where TApiResource : StoresIdentityServer4.Neo4j.Entities.ApiResource
        where TApiResourceClaim : StoresIdentityServer4.Neo4j.Entities.ApiResourceClaim
        where TIdentityResource : StoresIdentityServer4.Neo4j.Entities.IdentityResource
        where TIdentityClaim : StoresIdentityServer4.Neo4j.Entities.IdentityClaim
        where TApiSecret : StoresIdentityServer4.Neo4j.Entities.ApiSecret
        where TApiScope : StoresIdentityServer4.Neo4j.Entities.ApiScope
        where TApiScopeClaim : StoresIdentityServer4.Neo4j.Entities.ApiScopeClaim
        where TClaim : StoresIdentityServer4.Neo4j.Entities.ClientClaim
        where TCorsOrigin : StoresIdentityServer4.Neo4j.Entities.ClientCorsOrigin
        where TScope : StoresIdentityServer4.Neo4j.Entities.ClientScope
        where TIDPRestriction : StoresIdentityServer4.Neo4j.Entities.ClienTIDPRestriction
        where TProperty : StoresIdentityServer4.Neo4j.Entities.ClientProperty
        where TPostLogoutRedirectUri : StoresIdentityServer4.Neo4j.Entities.ClientPostLogoutRedirectUri
        where TRedirectUri : StoresIdentityServer4.Neo4j.Entities.ClientRedirectUri
    {
        private INeo4jTest _neo4jtest;
        private IUserStore<TUser> _userStore;

        private IIdentityServer4ClientUserStore<
            TUser,
            TClient,
            TSecret,
            TGrantType,
            TApiResource,
            TApiResourceClaim,
            TApiSecret,
            TApiScope,
            TApiScopeClaim,
            TClaim,
            TCorsOrigin,
            TScope,
            TIDPRestriction,
            TProperty,
            TPostLogoutRedirectUri,
            TRedirectUri,
            TIdentityResource,
            TIdentityClaim> _clientUserStore;

        public UnitTestClientStore(
            IUserStore<TUser> userStore,
            IIdentityServer4ClientUserStore<
                TUser,
                TClient,
                TSecret,
                TGrantType,
                TApiResource,
                TApiResourceClaim,
                TApiSecret,
                TApiScope,
                TApiScopeClaim,
                TClaim,
                TCorsOrigin,
                TScope,
                TIDPRestriction,
                TProperty,
                TPostLogoutRedirectUri,
                TRedirectUri,
                TIdentityResource,
                TIdentityClaim> clientUserStore,
            INeo4jTest neo4jtest)
        {
            _userStore = userStore;
            _clientUserStore = clientUserStore;
            _neo4jtest = neo4jtest;
        }

        protected abstract TClient CreateTestClient();
        protected abstract TUser CreateTestUser();
        protected abstract TGrantType CreateTestGrantType();
        protected abstract TSecret CreateTestSecret();
        protected abstract TClaim CreateTestClaim();
        protected abstract TScope CreateTestScope();
        protected abstract TCorsOrigin CreateTestCorsOrigin();
        protected abstract TPostLogoutRedirectUri CreatePostLogoutRedirectUri();
        protected abstract TRedirectUri CreateTestRedirectUri();
        protected abstract TProperty CreateTestProperty();
        protected abstract TIDPRestriction CreateTesTIDPRestriction();
        protected abstract TApiResource CreateTestApiResource();
        protected abstract TApiScope CreateTestApiScope();
        protected abstract TApiScopeClaim CreateTestApiScopeClaim();
        protected abstract TApiSecret CreateTestApiSecret();
        protected abstract TApiResourceClaim CreateTestApiResourceClaim();
        protected abstract TIdentityResource CreateTestIdentityResource();
        protected abstract TIdentityClaim CreateTestIdentityClaim();

        public static IEnumerable<ClientExtra> GetClient()
        {
            // client credentials client
            return new List<ClientExtra>
            {
                new ClientExtra
                {
                    AllowArbitraryLocalRedirectUris = true,
                    ClientId = "native.hybrid",
                    ClientName = "Native Client (Hybrid with PKCE)",

                    RedirectUris = {"https://test.com", "https://test.com"},
                    PostLogoutRedirectUris = {"https://test.com", "https://test.com"},

                    RequireClientSecret = false,

                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RequirePkce = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OpenId,
                        "profile",
                        "email",
                        "native_api"
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256()),
                        new Secret("secret".Sha256())
                    },
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    Claims = new List<Claim>()
                    {
                        new Claim("A", "B"),
                        new Claim("A", "B"),
                    },
                    IdentityProviderRestrictions = new List<string>()
                    {
                        "test",
                        "test"
                    },
                    AllowedCorsOrigins = new List<string>()
                        {"https://test.com", "https://test.com"}

                }
            };
        }

        public static IEnumerable<ClientExtra> GetClients()
        {
            // client credentials client
            return new List<ClientExtra>
            {
                new ClientExtra
                {
                    AllowArbitraryLocalRedirectUris = true,
                    ClientId = "native.hybrid",
                    ClientName = "Native Client (Hybrid with PKCE)",

                    RedirectUris = {"https://notused"},
                    PostLogoutRedirectUris = {"https://notused"},

                    RequireClientSecret = false,

                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RequirePkce = true,
                    AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.OpenId, "profile", "email", "native_api"},

                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse
                },
                new ClientExtra
                {
                    AllowArbitraryLocalRedirectUris = true,
                    ClientId = "native.code.wpf",
                    ClientName = "Native Client (Code with PKCE)",

                    RedirectUris = {"http://127.0.0.1/sample-wpf-app"},
                    PostLogoutRedirectUris = {"http://127.0.0.1/sample-wpf-app"},

                    RequireClientSecret = false,

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.OpenId, "profile", "native_api"},

                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse
                },
                new ClientExtra
                {
                    AllowArbitraryLocalRedirectUris = true,
                    ClientId = "native.code",
                    ClientName = "Native Client (Code with PKCE)",

                    RedirectUris = {"http://notreal"},
                    PostLogoutRedirectUris = {"http://notreal"},

                    RequireClientSecret = false,

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.OpenId, "profile", "native_api"},

                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse
                },
                new ClientExtra
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
                new ClientExtra
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
                new ClientExtra
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
                new ClientExtra
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
                    },
                    AllowOfflineAccess = true
                },
                new ClientExtra
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
                    },
                    AllowOfflineAccess = true
                },
                new ClientExtra
                {
                    ClientId = "Neo4j.PagesWebAppClient.NoUserDatabase",
                    ClientName = "Neo4j.PagesWebAppClient.NoUserDatabase Client",
                    AllowedGrantTypes =  new[] { GrantType.Hybrid },
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
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.Email
                    },
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    RequireClientSecret = true,
                    AlwaysIncludeUserClaimsInIdToken = true

                }
            };
        }
        [TestInitialize]
        public async Task Initialize()
        {
            await _neo4jtest.DropDatabaseAsync();
            await _clientUserStore.CreateConstraintsAsync();
        }

        [TestMethod]
        public void Map_Client()
        {
            var entityOriginal = CreateTestClient();
            var model = ClientMappers.ToModel(entityOriginal);
            model.ShouldNotBeNull();

            var entity = ClientMappers.ToEntity(model);
            entity.ShouldNotBeNull();
            entity.ClientId.ShouldBe(entityOriginal.ClientId);

            var neo4jEntity = model.ToNeo4jEntity();
            neo4jEntity.ShouldNotBeNull();
            neo4jEntity.ClientId.ShouldBe(entityOriginal.ClientId);
            // Assert
           

        }

        [TestMethod]
        public async Task Valid_DI()
        {
            _userStore.ShouldNotBeNull();
            _neo4jtest.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task Create_Client_Assure_Unique()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var client = CreateTestClient();
            var result = await _clientUserStore.CreateClientAsync(client);

            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldNotBeNull();
            findResult.ClientId.ShouldBe(client.ClientId);

            // do it again, but this time it should fail
            result = await _clientUserStore.CreateClientAsync(client);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeFalse();
        }

        [TestMethod]
        public async Task Create_Client_Delete()
        {
 
            TClient client = CreateTestClient();
            var result = await _clientUserStore.CreateClientAsync(client);

            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldNotBeNull();
            findResult.ClientId.ShouldBe(client.ClientId);

            result = await _clientUserStore.DeleteClientAsync(client);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldBeNull();

        }


        public async Task PopulateManyRelationships(TClient client, int nCount)
        {
            IdentityResult result;
            var count = nCount;
            for (int i = 0; i < count; ++i)
            {
                result = await _clientUserStore.AddSecretToClientAsync(client, CreateTestSecret());
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();
            }

            var secrets = await _clientUserStore.GetSecretsAsync(client);
            secrets.ShouldNotBeNull();
            secrets.Count.ShouldBe(count);

            for (int i = 0; i < count; ++i)
            {
                var grantType = CreateTestGrantType();

                var addResult =
                    await _clientUserStore.AddAllowedGrantTypeToClientAsync(client, grantType);
                addResult.ShouldNotBeNull();
                addResult.Succeeded.ShouldBeTrue();
            }

            var grants = await _clientUserStore.GetAllowedGrantTypesAsync(client);
            grants.ShouldNotBeNull();
            grants.Count.ShouldBe(count);

            for (int i = 0; i < count; ++i)
            {
                var claim = CreateTestClaim();

                var addResult =
                    await _clientUserStore.AddClaimToClientAsync(client, claim);
                addResult.ShouldNotBeNull();
                addResult.Succeeded.ShouldBeTrue();
            }

            var claims = await _clientUserStore.GetClaimsAsync(client);
            claims.ShouldNotBeNull();
            claims.Count.ShouldBe(count);

            for (int i = 0; i < count; ++i)
            {
                var scope = CreateTestScope();
                var scopResult = await _clientUserStore.AddScopeToClientAsync(client, scope);
                scopResult.ShouldNotBeNull();
                scopResult.Succeeded.ShouldBeTrue();
            }

            var scopes = await _clientUserStore.GetScopesAsync(client);
            scopes.ShouldNotBeNull();
            scopes.Count.ShouldBe(count);

            TCorsOrigin corsOrigin = null;

            for (int i = 0; i < count; ++i)
            {
                corsOrigin = CreateTestCorsOrigin();
                var resultCors = await _clientUserStore.AddCorsOriginToClientAsync(client, corsOrigin);
                resultCors.ShouldNotBeNull();
                resultCors.Succeeded.ShouldBeTrue();
            }

            var corsOrigins = await _clientUserStore.GetCorsOriginsAsync(client);
            corsOrigins.ShouldNotBeNull();
            corsOrigins.Count.ShouldBe(count);

            TIDPRestriction idp = null;

            for (int i = 0; i < count; ++i)
            {
                idp = CreateTesTIDPRestriction();
                var resultIdp = await _clientUserStore.AddIdPRestrictionToClientAsync(client, idp);
                resultIdp.ShouldNotBeNull();
                resultIdp.Succeeded.ShouldBeTrue();
            }


            var idps = await _clientUserStore.GetIDPRestrictionsAsync(client);
            idps.ShouldNotBeNull();
            idps.Count.ShouldBe(count);

            TPostLogoutRedirectUri postLogoutRedirectUri = null;

            for (int i = 0; i < count; ++i)
            {
                postLogoutRedirectUri = CreatePostLogoutRedirectUri();
                var resultpostLogoutRedirectUri =
                    await _clientUserStore.AddPostLogoutRedirectUriToClientAsync(client, postLogoutRedirectUri);
                resultpostLogoutRedirectUri.ShouldNotBeNull();
                resultpostLogoutRedirectUri.Succeeded.ShouldBeTrue();
            }


            var postLogoutRedirectUris = await _clientUserStore.GetPostLogoutRedirectUrisAsync(client);
            postLogoutRedirectUris.ShouldNotBeNull();
            postLogoutRedirectUris.Count.ShouldBe(count);

            TRedirectUri redirectUri = null;

            for (int i = 0; i < count; ++i)
            {
                redirectUri = CreateTestRedirectUri();
                var resultredirectUri = await _clientUserStore.AddRedirectUriToClientAsync(client, redirectUri);
                resultredirectUri.ShouldNotBeNull();
                resultredirectUri.Succeeded.ShouldBeTrue();
            }


            var resultredirectUris = await _clientUserStore.GetRedirectUrisAsync(client);
            resultredirectUris.ShouldNotBeNull();
            resultredirectUris.Count.ShouldBe(count);

            TProperty property = null;

            for (int i = 0; i < count; ++i)
            {
                property = CreateTestProperty();
                var resultproperty = await _clientUserStore.AddPropertyToClientAsync(client, property);
                resultproperty.ShouldNotBeNull();
                resultproperty.Succeeded.ShouldBeTrue();
            }


            var properties = await _clientUserStore.GetPropertiesAsync(client);
            properties.ShouldNotBeNull();
            properties.Count.ShouldBe(count);
        }

        [TestMethod]
        public async Task Create_Client_ManyRelationships_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            TClient client = CreateTestClient();

            var result = await _clientUserStore.CreateClientAsync(client);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldNotBeNull();
            findResult.ClientId.ShouldBe(client.ClientId);

            await PopulateManyRelationships(client, 10);

            /////////////////////////////////////////////////////////////

            result = await _clientUserStore.DeleteClientAsync(client);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldBeNull();

        }

        [TestMethod]
        public async Task Create_Client_ManyRelationships_Rollup()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            TClient client = CreateTestClient();

            var result = await _clientUserStore.CreateClientAsync(client);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldNotBeNull();
            findResult.ClientId.ShouldBe(client.ClientId);

            await PopulateManyRelationships(client, 1);

            /////////////////////////////////////////////////////////////

            var model = await _clientUserStore.GetRollupAsync(client);
            model.ShouldNotBeNull();
            model.ClientId.ShouldBe(client.ClientId);

        }

        [TestMethod]
        public async Task Create_Client_Secret_Update_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            TClient client = CreateTestClient();

            var result = await _clientUserStore.CreateClientAsync(client);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldNotBeNull();
            findResult.ClientId.ShouldBe(client.ClientId);

            var secret = CreateTestSecret();
            result = await _clientUserStore.AddSecretToClientAsync(client, secret);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var secrets = await _clientUserStore.GetSecretsAsync(client);
            secrets.ShouldNotBeNull();
            secrets.Count.ShouldBe(1);

            var secretModel = secrets[0].ToModel();
            var secretResult = await _clientUserStore.FindSecretAsync(client, secret);
            secretResult.ShouldNotBeNull();
            secretResult.Value.ShouldBe(secret.Value);
            secretResult.Description.ShouldBe(secret.Description);

            secret.Description = Unique.S;
            result = await _clientUserStore.UpdateSecretAsync(client, secret);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            secretResult = await _clientUserStore.FindSecretAsync(client, secret);
            secretResult.ShouldNotBeNull();
            secretResult.Value.ShouldBe(secret.Value);
            secretResult.Description.ShouldBe(secret.Description);


            result = await _clientUserStore.DeleteSecretAsync(client, secret);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            secretResult = await _clientUserStore.FindSecretAsync(client, secret);
            secretResult.ShouldBeNull();

        }

        [TestMethod]
        public async Task Create_Client_Claim_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            TClient client = CreateTestClient();

            var result = await _clientUserStore.CreateClientAsync(client);

            TClaim claim = null;
            var count = 10;
            for (int i = 0; i < count; ++i)
            {
                claim = CreateTestClaim();
                result = await _clientUserStore.AddClaimToClientAsync(client, claim);
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();
            }

            var claims = await _clientUserStore.GetClaimsAsync(client);
            claims.ShouldNotBeNull();
            claims.Count.ShouldBe(count);

            claim = claims[0];

            var claimResult = await _clientUserStore.FindClaimAsync(client, claim);
            claimResult.ShouldNotBeNull();
            claimResult.Value.ShouldBe(claim.Value);
            claimResult.Type.ShouldBe(claim.Type);

            result = await _clientUserStore.DeleteClaimAsync(client, claim);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            claimResult = await _clientUserStore.FindClaimAsync(client, claim);
            claimResult.ShouldBeNull();

        }

        [TestMethod]
        public async Task Create_Client_Scope_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            TClient client = CreateTestClient();

            await _clientUserStore.CreateClientAsync(client);

            TScope scope = null;
            var count = 10;
            for (int i = 0; i < count; ++i)
            {
                scope = CreateTestScope();
                var result = await _clientUserStore.AddScopeToClientAsync(client, scope);
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();
            }


            var scopes = await _clientUserStore.GetScopesAsync(client);
            scopes.ShouldNotBeNull();
            scopes.Count.ShouldBe(count);

            scope = scopes[0];

            var result2 = await _clientUserStore.FindScopeAsync(client, scope);
            result2.ShouldNotBeNull();
            result2.Scope.ShouldBe(scope.Scope);


            var result3 = await _clientUserStore.DeleteScopeAsync(client, scope);
            result3.ShouldNotBeNull();
            result3.Succeeded.ShouldBeTrue();

            result2 = await _clientUserStore.FindScopeAsync(client, scope);
            result2.ShouldBeNull();

        }

        [TestMethod]
        public async Task Create_Client_postlogout_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            TClient client = CreateTestClient();

            await _clientUserStore.CreateClientAsync(client);

            TPostLogoutRedirectUri record = null;
            var count = 10;
            for (int i = 0; i < count; ++i)
            {
                record = CreatePostLogoutRedirectUri();
                var result = await _clientUserStore.AddPostLogoutRedirectUriToClientAsync(client, record);
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();
            }


            var records = await _clientUserStore.GetPostLogoutRedirectUrisAsync(client);
            records.ShouldNotBeNull();
            records.Count.ShouldBe(count);

            record = records[0];

            var result2 = await _clientUserStore.FindPostLogoutRedirectUriAsync(client, record);
            result2.ShouldNotBeNull();
            result2.PostLogoutRedirectUri.ShouldBe(record.PostLogoutRedirectUri);


            var result3 = await _clientUserStore.DeletePostLogoutRedirectUriAsync(client, record);
            result3.ShouldNotBeNull();
            result3.Succeeded.ShouldBeTrue();

            result2 = await _clientUserStore.FindPostLogoutRedirectUriAsync(client, record);
            result2.ShouldBeNull();

        }

        [TestMethod]
        public async Task Create_Client_redirecturi_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            TClient client = CreateTestClient();

            await _clientUserStore.CreateClientAsync(client);

            TRedirectUri record = null;
            var count = 10;
            for (int i = 0; i < count; ++i)
            {
                record = CreateTestRedirectUri();
                var result = await _clientUserStore.AddRedirectUriToClientAsync(client, record);
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();
            }


            var records = await _clientUserStore.GetRedirectUrisAsync(client);
            records.ShouldNotBeNull();
            records.Count.ShouldBe(count);

            record = records[0];

            var result2 = await _clientUserStore.FindRedirectUriAsync(client, record);
            result2.ShouldNotBeNull();
            result2.RedirectUri.ShouldBe(record.RedirectUri);


            var result3 = await _clientUserStore.DeleteRedirectUriAsync(client, record);
            result3.ShouldNotBeNull();
            result3.Succeeded.ShouldBeTrue();

            result2 = await _clientUserStore.FindRedirectUriAsync(client, record);
            result2.ShouldBeNull();

        }

        [TestMethod]
        public async Task Create_Client_properties_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            TClient client = CreateTestClient();

            await _clientUserStore.CreateClientAsync(client);

            TProperty record = null;
            var count = 10;
            for (int i = 0; i < count; ++i)
            {
                record = CreateTestProperty();
                var result = await _clientUserStore.AddPropertyToClientAsync(client, record);
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();
            }


            var records = await _clientUserStore.GetPropertiesAsync(client);
            records.ShouldNotBeNull();
            records.Count.ShouldBe(count);

            record = records[0];

            var result2 = await _clientUserStore.FindPropertyAsync(client, record);
            result2.ShouldNotBeNull();
            result2.Key.ShouldBe(record.Key);
            result2.Value.ShouldBe(record.Value);

            var result3 = await _clientUserStore.DeletePropertyAsync(client, record);
            result3.ShouldNotBeNull();
            result3.Succeeded.ShouldBeTrue();

            result2 = await _clientUserStore.FindPropertyAsync(client, record);
            result2.ShouldBeNull();

        }



        [TestMethod]
        public async Task Create_Client_idp_restrictions_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            TClient client = CreateTestClient();

            await _clientUserStore.CreateClientAsync(client);

            TIDPRestriction idp = null;
            var count = 10;
            for (int i = 0; i < count; ++i)
            {
                idp = CreateTesTIDPRestriction();
                var result = await _clientUserStore.AddIdPRestrictionToClientAsync(client, idp);
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();
            }


            var idps = await _clientUserStore.GetIDPRestrictionsAsync(client);
            idps.ShouldNotBeNull();
            idps.Count.ShouldBe(count);

            idp = idps[0];

            var result2 = await _clientUserStore.FindIDPRestrictionAsync(client, idp);
            result2.ShouldNotBeNull();
            result2.Provider.ShouldBe(idp.Provider);


            var result3 = await _clientUserStore.DeleteIDPRestrictionAsync(client, idp);
            result3.ShouldNotBeNull();
            result3.Succeeded.ShouldBeTrue();

            result2 = await _clientUserStore.FindIDPRestrictionAsync(client, idp);
            result2.ShouldBeNull();

        }



        [TestMethod]
        public async Task Create_Client_Cors_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            TClient client = CreateTestClient();

            await _clientUserStore.CreateClientAsync(client);

            TCorsOrigin corsOrigin = null;
            var count = 10;
            for (int i = 0; i < count; ++i)
            {
                corsOrigin = CreateTestCorsOrigin();
                var result = await _clientUserStore.AddCorsOriginToClientAsync(client, corsOrigin);
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();
            }


            var corsOrigins = await _clientUserStore.GetCorsOriginsAsync(client);
            corsOrigins.ShouldNotBeNull();
            corsOrigins.Count.ShouldBe(count);

            corsOrigin = corsOrigins[0];

            var result2 = await _clientUserStore.FindCorsOriginAsync(client, corsOrigin);
            result2.ShouldNotBeNull();
            result2.Origin.ShouldBe(corsOrigin.Origin);


            var result3 = await _clientUserStore.DeleteCorsOriginAsync(client, corsOrigin);
            result3.ShouldNotBeNull();
            result3.Succeeded.ShouldBeTrue();

            result2 = await _clientUserStore.FindCorsOriginAsync(client, corsOrigin);
            result2.ShouldBeNull();

        }




        [TestMethod]
        public async Task Create_Client_Redundant_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            TClient client = CreateTestClient();
            var result = await _clientUserStore.CreateClientAsync(client);

            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldNotBeNull();
            findResult.ClientId.ShouldBe(client.ClientId);

            result = await _clientUserStore.DeleteClientAsync(client);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldBeNull();

            result = await _clientUserStore.DeleteClientAsync(client);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();
        }

        [TestMethod]
        public async Task Create_ApiResource_Assure_Unique()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var apiResouce = CreateTestApiResource();

            var result = await _clientUserStore.CreateApiResourceAsync(apiResouce);

            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.GetApiResourceAsync(apiResouce);
            findResult.ShouldNotBeNull();
            findResult.Name.ShouldBe(apiResouce.Name);

            // do it again, but this time it should fail
            result = await _clientUserStore.CreateApiResourceAsync(apiResouce);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeFalse();
        }

        [TestMethod]
        public async Task Insert_Model_Standard_IdentityResources()
        {
            var result = await _clientUserStore.EnsureStandardAsync();
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var identityResources =
                await _clientUserStore.GetIdentityResourcesAsync();
            identityResources.ShouldNotBeNull();
            identityResources.Count.ShouldBeGreaterThan(1);

            var count = identityResources.Count;
            // do it again, but this time it should fail
            await _clientUserStore.EnsureStandardAsync();

            identityResources =
                await _clientUserStore.GetIdentityResourcesAsync();
            identityResources.ShouldNotBeNull();
            identityResources.Count.ShouldBe(count);
        }
        [TestMethod]
        public async Task Mapper_ClientExtra()
        {
            var testClient = CreateTestClient();
            var model = ClientMappers.ToModel(testClient);
            var cc = ClientMappers.ToEntity(model);
            var dd = Neo4jIdentityServer4ClientExtraMappers.ToNeo4jEntity(cc);
            var ee = Neo4jIdentityServer4ClientExtraMappers.ToNeo4jEntity(model);

        }
        [TestMethod] public async Task Insert_Model_ClientExtra()
        {
            var testUser = CreateTestUser();

            var createUserResult = await _userStore.CreateAsync(
                testUser, CancellationToken.None);

            var clients = GetClient();


            var result = await _clientUserStore.InsertClients(testUser,clients);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            
        }

        [TestMethod]
        public async Task Insert_Model_ApiResorces()
        {
            var apiResources = new List<IdentityServer4.Models.ApiResource>
            {
                new IdentityServer4.Models.ApiResource("api1", "My API"),
                new IdentityServer4.Models.ApiResource("native_api", "Native Client API")
                {
                    ApiSecrets =
                    {
                        new IdentityServer4.Models.Secret("native_api_secret".Sha256())
                    }
                }
            };


            var result = await _clientUserStore.InsertApiResources(apiResources);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var foundApiResources =
                await _clientUserStore.GetApiResourcesAsync();
            foundApiResources.ShouldNotBeNull();
            foundApiResources.Count.ShouldBe(apiResources.Count);


            // do it again, but this time it should fail
            result = await _clientUserStore.InsertApiResources(apiResources);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeFalse();

            foundApiResources =
                await _clientUserStore.GetApiResourcesAsync();
            foundApiResources.ShouldNotBeNull();
            foundApiResources.Count.ShouldBe(apiResources.Count);
        }
        [TestMethod]
        public async Task Create_IdentityResource_Assure_Unique()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var identityResource = CreateTestIdentityResource();

            var result = await _clientUserStore.CreateIdentityResourceAsync(identityResource);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.GetIdentityResourceAsync(identityResource);
            findResult.ShouldNotBeNull();
            findResult.Name.ShouldBe(identityResource.Name);

            // do it again, but this time it should fail
            result = await _clientUserStore.CreateIdentityResourceAsync(identityResource);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeFalse();
        }
        [TestMethod]
        public async Task Create_IdentityResource_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var identityResource = CreateTestIdentityResource();

            var result = await _clientUserStore.CreateIdentityResourceAsync(identityResource);

            var findResult =
                await _clientUserStore.GetIdentityResourceAsync(identityResource);
            findResult.ShouldNotBeNull();
            findResult.Name.ShouldBe(identityResource.Name);

            result = await _clientUserStore.DeleteIdentityResourceAsync(identityResource);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            findResult =
                await _clientUserStore.GetIdentityResourceAsync(identityResource);
            findResult.ShouldBeNull();
        }
        [TestMethod]
        public async Task Create_Many_IdentityResource_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;

            int nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                var identityResource = CreateTestIdentityResource();
                var result = await _clientUserStore.CreateIdentityResourceAsync(identityResource);
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();
            }

            var identityResources = await _clientUserStore.GetIdentityResourcesAsync();
            identityResources.ShouldNotBeNull();
            identityResources.Count.ShouldBe(nCount);


            var result2 = await _clientUserStore.DeleteIdentityResourcesAsync();
            result2.ShouldNotBeNull();
            result2.Succeeded.ShouldBeTrue();

            identityResources = await _clientUserStore.GetIdentityResourcesAsync();
            identityResources.ShouldNotBeNull();
            identityResources.Count.ShouldBe(0);
        }
        [TestMethod]
        public async Task Create_IdentityResource_Claims_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var identityResource = CreateTestIdentityResource();
            var identityClaim = CreateTestIdentityClaim();

            var result = await _clientUserStore.CreateIdentityResourceAsync(identityResource);

            result = await _clientUserStore.AddIdentityClaimAsync(identityResource, identityClaim);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.GetIdentityClaimAsync(identityResource, identityClaim);
            findResult.ShouldNotBeNull();
            findResult.Type.ShouldBe(identityClaim.Type);

            result = await _clientUserStore.DeleteIdentityClaimAsync(identityResource, identityClaim);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            findResult =
                await _clientUserStore.GetIdentityClaimAsync(identityResource, identityClaim);
            findResult.ShouldBeNull();
        }
        [TestMethod]
        public async Task Create_IdentityResource_Many_Claims_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var identityResource = CreateTestIdentityResource();
          

            var result = await _clientUserStore.CreateIdentityResourceAsync(identityResource);

            var nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                var identityClaim = CreateTestIdentityClaim();
                result = await _clientUserStore.AddIdentityClaimAsync(identityResource, identityClaim);
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();
            }
        

            var findResult =
                await _clientUserStore.GetIdentityClaimsAsync(identityResource);
            findResult.ShouldNotBeNull();
            findResult.Count.ShouldBe(nCount);

            result = await _clientUserStore.DeleteIdentityClaimsAsync(identityResource);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            findResult =
                await _clientUserStore.GetIdentityClaimsAsync(identityResource);
            findResult.ShouldNotBeNull();
            findResult.Count.ShouldBe(0);
        }
        [TestMethod]
        public async Task Create_IdentityResource_Many_Claims_Rollup()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var identityResource = CreateTestIdentityResource();


            var result = await _clientUserStore.CreateIdentityResourceAsync(identityResource);

            var nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                var identityClaim = CreateTestIdentityClaim();
                result = await _clientUserStore.AddIdentityClaimAsync(identityResource, identityClaim);
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();
            }


            var rollup =
                await _clientUserStore.GetRollupAsync(identityResource);
            rollup.ShouldNotBeNull();
            rollup.Name.ShouldBe(identityResource.Name);
            rollup.UserClaims.Count.ShouldBe(nCount);

            var identityClaim2 = CreateTestIdentityClaim();
            result = await _clientUserStore.AddIdentityClaimAsync(identityResource, identityClaim2);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();


            rollup =
                await _clientUserStore.GetRollupAsync(identityResource);
            rollup.ShouldNotBeNull();
            rollup.Name.ShouldBe(identityResource.Name);
            rollup.UserClaims.Count.ShouldBe(nCount+1);
        }


        [TestMethod]
        public async Task Create_GrantType_Assure_Unique()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var grantType = CreateTestGrantType();

            var result = await _clientUserStore.CreateGrantTypeAsync(grantType);

            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.FindGrantTypeAsync(grantType);
            findResult.ShouldNotBeNull();
            findResult.GrantType.ShouldBe(grantType.GrantType);

            // do it again, but this time it should fail
            result = await _clientUserStore.CreateGrantTypeAsync(grantType);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeFalse();
        }

        [TestMethod]
        public async Task Create_GrantType_Update()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var grantType = CreateTestGrantType();

            var result = await _clientUserStore.CreateGrantTypeAsync(grantType);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.FindGrantTypeAsync(grantType);
            findResult.ShouldNotBeNull();
            findResult.GrantType.ShouldBe(grantType.GrantType);

            var grantTypeNew = CreateTestGrantType();
            grantTypeNew.GrantType = findResult.GrantType;
            grantType = grantTypeNew;
            result = await _clientUserStore.UpdateGrantTypeAsync(grantType);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            findResult =
                await _clientUserStore.FindGrantTypeAsync(grantType);
            findResult.ShouldNotBeNull();
            findResult.GrantType.ShouldBe(grantTypeNew.GrantType);
        }
        [TestMethod]
        public async Task Create_ApiResource_Redundant_Delete()
        {
            var apiResouce = CreateTestApiResource();

            var result = await _clientUserStore.CreateApiResourceAsync(apiResouce);

            // delete it
            result = await _clientUserStore.DeleteApiResourceAsync(apiResouce);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.GetApiResourceAsync(apiResouce);
            findResult.ShouldBeNull();

            // delete it
            result = await _clientUserStore.DeleteApiResourceAsync(apiResouce);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();
        }
       
        [TestMethod]
        public async Task Create_ApiResource_ApiSecret_Delete()
        {
            var apiResouce = CreateTestApiResource();
            await _clientUserStore.CreateApiResourceAsync(apiResouce);

            var apiSecret = CreateTestApiSecret();

            var result = await _clientUserStore.AddApiSecretAsync(apiResouce, apiSecret);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            result = await _clientUserStore.AddApiSecretAsync(apiResouce, apiSecret);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var founApiSecret = await _clientUserStore.GetApiSecretAsync(apiResouce, apiSecret);
            founApiSecret.ShouldNotBeNull();
            founApiSecret.Type.ShouldBe(apiSecret.Type);
            founApiSecret.Value.ShouldBe(apiSecret.Value);

            var foundSecrets = await _clientUserStore.GetApiSecretsAsync(apiResouce);
            foundSecrets.ShouldNotBeNull();
            foundSecrets.Count.ShouldBe(1);
            foundSecrets[0].ShouldNotBeNull();
            foundSecrets[0].Type.ShouldBe(apiSecret.Type);
            foundSecrets[0].Value.ShouldBe(apiSecret.Value);


            result = await _clientUserStore.DeleteApiSecretAsync(apiResouce, apiSecret);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            founApiSecret = await _clientUserStore.GetApiSecretAsync(apiResouce, apiSecret);
            founApiSecret.ShouldBeNull();

        }
        [TestMethod]
        public async Task Create_ApiResource_ApiResourceClaim_Delete()
        {
            var apiResouce = CreateTestApiResource();
            await _clientUserStore.CreateApiResourceAsync(apiResouce);

            var apiResourceClaim = CreateTestApiResourceClaim();

            var result = await _clientUserStore.AddApiResourceClaimAsync(apiResouce, apiResourceClaim);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            result = await _clientUserStore.AddApiResourceClaimAsync(apiResouce, apiResourceClaim);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var founApiResourceClaim = await _clientUserStore.GetApiResourceClaimAsync(apiResouce, apiResourceClaim);
            founApiResourceClaim.ShouldNotBeNull();
            founApiResourceClaim.Type.ShouldBe(apiResourceClaim.Type);
           

            var foundApiResourceClaims = await _clientUserStore.GetApiResourceClaimsAsync(apiResouce);
            foundApiResourceClaims.ShouldNotBeNull();
            foundApiResourceClaims.Count.ShouldBe(1);
            foundApiResourceClaims[0].ShouldNotBeNull();
            foundApiResourceClaims[0].Type.ShouldBe(apiResourceClaim.Type);
 
            result = await _clientUserStore.DeleteApiResourceClaimAsync(apiResouce, apiResourceClaim);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            founApiResourceClaim = await _clientUserStore.GetApiResourceClaimAsync(apiResouce, apiResourceClaim);
            founApiResourceClaim.ShouldBeNull();

            foundApiResourceClaims = await _clientUserStore.GetApiResourceClaimsAsync(apiResouce);
            foundApiResourceClaims.ShouldNotBeNull();
            foundApiResourceClaims.Count.ShouldBe(0);
 
        }
        [TestMethod]
        public async Task Create_ApiResource_Many_ApiResourceClaim_Delete()
        {
            var apiResouce = CreateTestApiResource();
            var result = await _clientUserStore.CreateApiResourceAsync(apiResouce);

         


            var count = 10;

            for (int i = 0; i < count; ++i)
            {
                var apiResourceClaim = CreateTestApiResourceClaim();
                result = await _clientUserStore.AddApiResourceClaimAsync(apiResouce, apiResourceClaim);
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();
            }
        
            var foundApiResourceClaims = await _clientUserStore.GetApiResourceClaimsAsync(apiResouce);
            foundApiResourceClaims.ShouldNotBeNull();
            foundApiResourceClaims.Count.ShouldBe(count);
 

            result = await _clientUserStore.DeleteApiResourceClaimsAsync(apiResouce);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();
 
            foundApiResourceClaims = await _clientUserStore.GetApiResourceClaimsAsync(apiResouce);
            foundApiResourceClaims.ShouldNotBeNull();
            foundApiResourceClaims.Count.ShouldBe(0);

        }


        [TestMethod]
        public async Task Create_ApiResource_Many_ApiSecret_Delete()
        {
            var apiResouce = CreateTestApiResource();
            var result = await _clientUserStore.CreateApiResourceAsync(apiResouce);

            

            var count = 10;
            for (int i = 0; i < count; ++i)
            {
                var apiSecret = CreateTestApiSecret();
                result = await _clientUserStore.AddApiSecretAsync(apiResouce, apiSecret);
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();
            }

            var foundSecrets = await _clientUserStore.GetApiSecretsAsync(apiResouce);
            foundSecrets.ShouldNotBeNull();
            foundSecrets.Count.ShouldBe(count);
         
            result = await _clientUserStore.DeleteApiSecretsAsync(apiResouce);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            foundSecrets = await _clientUserStore.GetApiSecretsAsync(apiResouce);
            foundSecrets.ShouldNotBeNull();
            foundSecrets.Count.ShouldBe(0);

        }

        [TestMethod]
        public async Task Create_ApiResource_ApiScope_Delete()
        {
            var apiResouce = CreateTestApiResource();
            await _clientUserStore.CreateApiResourceAsync(apiResouce);

            var apiScope = CreateTestApiScope();

            var result = await _clientUserStore.AddApiScopeAsync(apiResouce, apiScope);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            result = await _clientUserStore.AddApiScopeAsync(apiResouce, apiScope);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var scope = await _clientUserStore.GetApiScopeAsync(apiResouce, apiScope);
            scope.ShouldNotBeNull();
            scope.Name.ShouldBe(apiScope.Name);

            var scopes = await _clientUserStore.GetApiScopesAsync(apiResouce);
            scopes.ShouldNotBeNull();
            scopes.Count.ShouldBe(1);
            scopes[0].ShouldNotBeNull();
            scopes[0].Name.ShouldBe(apiScope.Name);

            result = await _clientUserStore.DeleteApiScopeAsync(apiResouce, apiScope);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            scope = await _clientUserStore.GetApiScopeAsync(apiResouce, apiScope);
            scope.ShouldBeNull();

        }
        [TestMethod]
        public async Task Create_ApiResource_ApiScope_Claims_Delete()
        {
            var apiResouce = CreateTestApiResource();
            await _clientUserStore.CreateApiResourceAsync(apiResouce);

            var apiScope = CreateTestApiScope();

            var result = await _clientUserStore.AddApiScopeAsync(apiResouce, apiScope);

            var apiClaim = CreateTestApiScopeClaim();
            result = await _clientUserStore.AddApiScopeClaimAsync(
                apiResouce,
                apiScope,
                apiClaim);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var foundScopeClaim = await _clientUserStore.GetApiScopeClaimAsync(
                apiResouce,
                apiScope,
                apiClaim);
            foundScopeClaim.ShouldNotBeNull();
            foundScopeClaim.Type.ShouldBe(apiClaim.Type);

            var scopeClaims = await _clientUserStore.GetApiScopeClaimsAsync(apiResouce,
                apiScope);
            scopeClaims.ShouldNotBeNull();
            scopeClaims.Count.ShouldBe(1);


            var scopes = await _clientUserStore.DeleteApiScopeClaimAsync(apiResouce,
                apiScope,
                apiClaim);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            foundScopeClaim = await _clientUserStore.GetApiScopeClaimAsync(
                apiResouce,
                apiScope,
                apiClaim);
            foundScopeClaim.ShouldBeNull(); 
          

            scopeClaims = await _clientUserStore.GetApiScopeClaimsAsync(apiResouce,
                apiScope);
            scopeClaims.ShouldNotBeNull();
            scopeClaims.Count.ShouldBe(0);
        }
        [TestMethod]
        public async Task Create_ApiResource_ApiScope_Claims_Rollup()
        {
            var apiResouce = CreateTestApiResource();
            await _clientUserStore.CreateApiResourceAsync(apiResouce);

            var apiScope = CreateTestApiScope();

            var result = await _clientUserStore.AddApiScopeAsync(apiResouce, apiScope);

            var apiClaim = CreateTestApiScopeClaim();
            result = await _clientUserStore.AddApiScopeClaimAsync(
                apiResouce,
                apiScope,
                apiClaim);

            var rollup = await _clientUserStore.GetRollupAsync(apiResouce,apiScope);
            rollup.ShouldNotBeNull();
            rollup.Name.ShouldBe(apiScope.Name);
            rollup.UserClaims.Count.ShouldBe(1);

            apiClaim = CreateTestApiScopeClaim();
            result = await _clientUserStore.AddApiScopeClaimAsync(
                apiResouce,
                apiScope,
                apiClaim);

            rollup = await _clientUserStore.GetRollupAsync(apiResouce, apiScope);
            rollup.ShouldNotBeNull();
            rollup.Name.ShouldBe(apiScope.Name);
            rollup.UserClaims.Count.ShouldBe(2);
        }
        [TestMethod]
        public async Task Create_ApiResource_FULL_Rollup()
        {
            var apiResouce = CreateTestApiResource();
            await _clientUserStore.CreateApiResourceAsync(apiResouce);

            var apiScope = CreateTestApiScope();
            var result = await _clientUserStore.AddApiScopeAsync(apiResouce, apiScope);

            var nCount = 2;
            for (int i = 0; i < nCount; ++i)
            {
                apiScope = CreateTestApiScope();

                result = await _clientUserStore.AddApiScopeAsync(apiResouce, apiScope);

                var apiClaim = CreateTestApiScopeClaim();
                result = await _clientUserStore.AddApiScopeClaimAsync(
                    apiResouce,
                    apiScope,
                    apiClaim);

                var apiSecret = CreateTestApiSecret();
                result = await _clientUserStore.AddApiSecretAsync(apiResouce, apiSecret);
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();

                var apiResourceClaim = CreateTestApiResourceClaim();
                result = await _clientUserStore.AddApiResourceClaimAsync(apiResouce, apiResourceClaim);
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();
            }

            var rollup = await _clientUserStore.GetRollupAsync(apiResouce);
            rollup.ShouldNotBeNull();
            rollup.Name.ShouldBe(apiResouce.Name);
            
            rollup.ApiSecrets.Count.ShouldBe(nCount);
            rollup.Scopes.Count.ShouldBe(nCount + 2);  // the api resource name automatically gets added.
            rollup.UserClaims.Count.ShouldBe(nCount);

            var apiClaim2 = CreateTestApiScopeClaim();
             
            
            result = await _clientUserStore.AddApiScopeClaimAsync(
                apiResouce,
                apiScope,
                apiClaim2);

            rollup.ApiSecrets.Count.ShouldBe(nCount);
            rollup.Scopes.Count.ShouldBe(nCount + 2);  // the api resource name automatically gets added.
            rollup.UserClaims.Count.ShouldBe(nCount);
        }
        [TestMethod]
        public async Task Create_ApiResource_ApiScope_Many_Claims_Delete()
        {
            var apiResouce = CreateTestApiResource();
            await _clientUserStore.CreateApiResourceAsync(apiResouce);

            var apiScope = CreateTestApiScope();

            var result = await _clientUserStore.AddApiScopeAsync(apiResouce, apiScope);

            int count = 10;
            for (int i = 0; i < count; ++i)
            {
                var apiClaim = CreateTestApiScopeClaim();
                result = await _clientUserStore.AddApiScopeClaimAsync(
                    apiResouce,
                    apiScope,
                    apiClaim);
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();
            }
 

            var scopeClaims = await _clientUserStore.GetApiScopeClaimsAsync(apiResouce,
                apiScope);
            scopeClaims.ShouldNotBeNull();
            scopeClaims.Count.ShouldBe(count);

            result = await _clientUserStore.DeleteApiScopeClaimsAsync(apiResouce, apiScope);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            scopeClaims = await _clientUserStore.GetApiScopeClaimsAsync(apiResouce,
                apiScope);
            scopeClaims.ShouldNotBeNull();
            scopeClaims.Count.ShouldBe(0);
        }

        [TestMethod]
        public async Task Create_GrantType_Redundant_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var grantType = CreateTestGrantType();

            var result = await _clientUserStore.CreateGrantTypeAsync(grantType);

            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.FindGrantTypeAsync(grantType);
            findResult.ShouldNotBeNull();
            findResult.GrantType.ShouldBe(grantType.GrantType);

            // delete it
            result = await _clientUserStore.DeleteGrantTypeAsync(grantType);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            findResult =
                await _clientUserStore.FindGrantTypeAsync(grantType);
            findResult.ShouldBeNull();

            // delete it
            result = await _clientUserStore.DeleteGrantTypeAsync(grantType);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();
        }

        [TestMethod]
        public async Task Create_User_Client_update()
        {
            var testUser = CreateTestUser();

            var createUserResult = await _userStore.CreateAsync(testUser, CancellationToken.None);

            var client = CreateTestClient();
            var identityResult = await _clientUserStore.AddClientToUserAsync(
                testUser, client);
            identityResult.ShouldNotBeNull();
            identityResult.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldNotBeNull();
            findResult.ClientId.ShouldBe(client.ClientId);
            findResult.ClientName.ShouldBe(client.ClientName);

            var client2 = CreateTestClient();
            client.ClientName = client2.ClientName;

            identityResult = await _clientUserStore.UpdateClientAsync(client,
                CancellationToken.None);
            identityResult.ShouldNotBeNull();
            identityResult.Succeeded.ShouldBeTrue();

            findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldNotBeNull();
            findResult.ClientId.ShouldBe(client.ClientId);
            findResult.ClientName.ShouldBe(client.ClientName);
        }

        [TestMethod]
        public async Task Create_Factor_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            TClient client = CreateTestClient();

            var result = await _clientUserStore.CreateClientAsync(client);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldNotBeNull();
            findResult.ClientId.ShouldBe(client.ClientId);

            result = await _clientUserStore.DeleteClientAsync(client);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldBeNull();

        }

        [TestMethod]
        public async Task Create_User_Client_Many_CreatGrantType_AddAllowedGrantType()
        {
            var testUser = CreateTestUser();

            var createUserResult = await _userStore.CreateAsync(testUser, CancellationToken.None);

            var client = CreateTestClient();


            var identityResult = await _clientUserStore.AddClientToUserAsync(
                testUser, client);
            identityResult.ShouldNotBeNull();
            identityResult.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldNotBeNull();
            findResult.ClientId.ShouldBe(client.ClientId);
            findResult.ClientName.ShouldBe(client.ClientName);

            var count = 10;
            for (int i = 0; i < count; ++i)
            {
                var grantType = CreateTestGrantType();
                identityResult = await _clientUserStore.CreateGrantTypeAsync(grantType);
                identityResult.ShouldNotBeNull();
                identityResult.Succeeded.ShouldBeTrue();

                var addResult =
                    await _clientUserStore.AddAllowedGrantTypeToClientAsync(client, grantType);
                addResult.ShouldNotBeNull();
                addResult.Succeeded.ShouldBeTrue();
            }

            var grantTypes = await _clientUserStore.GetAllowedGrantTypesAsync(client);
            grantTypes.ShouldNotBeNull();
            grantTypes.Count.ShouldBe(count);

        }

        [TestMethod]
        public async Task Create_User_Client_Many_NoGrantType_AddAllowedGrantType()
        {
            var testUser = CreateTestUser();

            var createUserResult = await _userStore.CreateAsync(testUser, CancellationToken.None);

            var client = CreateTestClient();
            var identityResult = await _clientUserStore.AddClientToUserAsync(
                testUser, client);
            identityResult.ShouldNotBeNull();
            identityResult.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldNotBeNull();
            findResult.ClientId.ShouldBe(client.ClientId);
            findResult.ClientName.ShouldBe(client.ClientName);

            var count = 10;
            for (int i = 0; i < count; ++i)
            {
                var grantType = CreateTestGrantType();

                var addResult =
                    await _clientUserStore.AddAllowedGrantTypeToClientAsync(client, grantType);
                addResult.ShouldNotBeNull();
                addResult.Succeeded.ShouldBeTrue();
            }

            var grantTypes = await _clientUserStore.GetAllowedGrantTypesAsync(client);
            grantTypes.ShouldNotBeNull();
            grantTypes.Count.ShouldBe(count);

        }

    }
}
