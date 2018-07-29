using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4.Models;
using IdentityServer4Extras;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stores.IdentityServer4.Neo4j;
using Stores.IdentityServer4.Neo4j.Entities;

namespace Stores.IdentityServer4.Test.Core.Store
{
    public abstract class UnitTestClientStore<TUser,TClient, TSecret,TGrantType>
        where TUser : Neo4jIdentityUser
        where TClient : ClientRoot
        where TSecret : Secret
        where TGrantType : ClientGrantType
    {
        private INeo4jTest _neo4jtest;
        private IUserStore<TUser> _userStore;
        private IIdentityServer4ClientUserStore<TUser, TClient, TSecret, TGrantType> _clientUserStore;

        public UnitTestClientStore(
            IUserStore<TUser> userStore,
            IIdentityServer4ClientUserStore<TUser, TClient, TSecret, TGrantType> clientUserStore,
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
        [TestInitialize]
        public async Task Initialize()
        {
            await _neo4jtest.DropDatabaseAsync();
            await _clientUserStore.CreateConstraintsAsync();
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

            result = await _clientUserStore.DeleteAsync(client);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldBeNull();

        }
        [TestMethod]
        public async Task Create_Client_ManySecrets_Delete()
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

            var count = 10;
            for (int i = 0; i < count; ++i)
            {
                result = await _clientUserStore.AddSecretToClientAsync(client, CreateTestSecret());
                result.ShouldNotBeNull();
                result.Succeeded.ShouldBeTrue();
            }
           
            for (int i = 0; i < count; ++i)
            {
                var grantType = CreateTestGrantType();

                var addResult =
                    await _clientUserStore.AddAllowedGrantTypeToClientAsync(client, grantType);
                addResult.ShouldNotBeNull();
                addResult.Succeeded.ShouldBeTrue();
            }
            var secrets = await _clientUserStore.GetSecretsAsync(client);
            secrets.ShouldNotBeNull();
            secrets.Count.ShouldBe(count);

            result = await _clientUserStore.DeleteAsync(client);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldBeNull();

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


            result = await _clientUserStore.DeleteSecretAsync(client,secret);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            secretResult = await _clientUserStore.FindSecretAsync(client, secret);
            secretResult.ShouldBeNull();

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

            result = await _clientUserStore.DeleteAsync(client);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            findResult =
                await _clientUserStore.FindClientByClientIdAsync(client.ClientId);
            findResult.ShouldBeNull();

            result = await _clientUserStore.DeleteAsync(client);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();
        }


        [TestMethod]
        public async Task Create_GrantType_Assure_Unique()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var grantType = CreateTestGrantType();

            var result = await _clientUserStore.CreateAsync(grantType);

            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.FindGrantTypeAsync(grantType.GrantType);
            findResult.ShouldNotBeNull();
            findResult.GrantType.ShouldBe(grantType.GrantType);

            // do it again, but this time it should fail
            result = await _clientUserStore.CreateAsync(grantType);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeFalse();
        }

        [TestMethod]
        public async Task Create_GrantType_Update()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var grantType = CreateTestGrantType();

            var result = await _clientUserStore.CreateAsync(grantType);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.FindGrantTypeAsync(grantType.GrantType);
            findResult.ShouldNotBeNull();
            findResult.GrantType.ShouldBe(grantType.GrantType);

            var grantTypeNew = CreateTestGrantType();

            result = await _clientUserStore.UpdateAsync(grantType, grantTypeNew);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            findResult =
                await _clientUserStore.FindGrantTypeAsync(grantType.GrantType);
            findResult.ShouldBeNull();

            findResult =
                await _clientUserStore.FindGrantTypeAsync(grantTypeNew.GrantType);
            findResult.ShouldNotBeNull();
            findResult.GrantType.ShouldBe(grantTypeNew.GrantType);
        }

        [TestMethod]
        public async Task Create_GrantType_Redundant_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var grantType = CreateTestGrantType();

            var result = await _clientUserStore.CreateAsync(grantType);

            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.FindGrantTypeAsync(grantType.GrantType);
            findResult.ShouldNotBeNull();
            findResult.GrantType.ShouldBe(grantType.GrantType);

            // delete it
            result = await _clientUserStore.DeleteAsync(grantType);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            findResult =
                await _clientUserStore.FindGrantTypeAsync(grantType.GrantType);
            findResult.ShouldBeNull();

            // delete it
            result = await _clientUserStore.DeleteAsync(grantType);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();
        }

        [TestMethod]
        public async Task Create_User_Client_update()
        {
            var testUser = CreateTestUser();

            var createUserResult = await _userStore.CreateAsync(testUser, CancellationToken.None);

            var client = CreateTestClient();
            var identityResult = await _clientUserStore.AddToClientAsync(
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

            identityResult = await _clientUserStore.UpdateAsync(client,
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

            result = await _clientUserStore.DeleteAsync(client);
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
            var identityResult = await _clientUserStore.AddToClientAsync(
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
                identityResult = await _clientUserStore.CreateAsync(grantType);
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
            var identityResult = await _clientUserStore.AddToClientAsync(
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
