using System.Collections.Generic;
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
using StoresIdentityServer4.Neo4j.Entities;
using StoresIdentityServer4.Neo4j.Mappers;
using ApiResource = StoresIdentityServer4.Neo4j.Entities.ApiResource;
using Client = StoresIdentityServer4.Neo4j.Entities.Client;

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
        TIdPRestriction,
        TProperty,
        TPostLogoutRedirectUri,
        TRedirectUri>
        where TUser : Neo4jIdentityUser
        where TClient : Client
        where TSecret : Secret
        where TGrantType : ClientGrantType
        where TApiResource : ApiResource
        where TApiResourceClaim : StoresIdentityServer4.Neo4j.Entities.ApiResourceClaim
        where TApiSecret : StoresIdentityServer4.Neo4j.Entities.ApiSecret
        where TApiScope : StoresIdentityServer4.Neo4j.Entities.ApiScope
        where TApiScopeClaim : StoresIdentityServer4.Neo4j.Entities.ApiScopeClaim
        where TClaim : ClientClaim
        where TCorsOrigin : ClientCorsOrigin
        where TScope : ClientScope
        where TIdPRestriction : ClientIDPRestriction
        where TProperty : ClientProperty
        where TPostLogoutRedirectUri : ClientPostLogoutRedirectUri
        where TRedirectUri : ClientRedirectUri
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
            TIdPRestriction,
            TProperty,
            TPostLogoutRedirectUri,
            TRedirectUri> _clientUserStore;

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
                TIdPRestriction,
                TProperty,
                TPostLogoutRedirectUri,
                TRedirectUri> clientUserStore,
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
        protected abstract TIdPRestriction CreateTestIdpRestriction();

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
            var entity = ClientMappers.ToEntity(model);

            // Assert
            entity.ShouldNotBeNull();
            model.ShouldNotBeNull();

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

            TIdPRestriction idp = null;

            for (int i = 0; i < count; ++i)
            {
                idp = CreateTestIdpRestriction();
                var resultIdp = await _clientUserStore.AddIdPRestrictionToClientAsync(client, idp);
                resultIdp.ShouldNotBeNull();
                resultIdp.Succeeded.ShouldBeTrue();
            }


            var idps = await _clientUserStore.GetIdPRestrictionsAsync(client);
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

            await PopulateManyRelationships(client, 10);

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

            TIdPRestriction idp = null;
            var count = 10;
            for (int i = 0; i < count; ++i)
            {
                idp = CreateTestIdpRestriction();
                var result = await _clientUserStore.AddIdPRestrictionToClientAsync(client, idp);
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();
            }


            var idps = await _clientUserStore.GetIdPRestrictionsAsync(client);
            idps.ShouldNotBeNull();
            idps.Count.ShouldBe(count);

            idp = idps[0];

            var result2 = await _clientUserStore.FindIdPRestrictionAsync(client, idp);
            result2.ShouldNotBeNull();
            result2.Provider.ShouldBe(idp.Provider);


            var result3 = await _clientUserStore.DeleteIdPRestrictionAsync(client, idp);
            result3.ShouldNotBeNull();
            result3.Succeeded.ShouldBeTrue();

            result2 = await _clientUserStore.FindIdPRestrictionAsync(client, idp);
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
