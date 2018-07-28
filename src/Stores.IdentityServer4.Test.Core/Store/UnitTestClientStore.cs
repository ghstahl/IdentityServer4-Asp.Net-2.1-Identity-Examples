using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4Extras;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stores.IdentityServer4.Neo4j;
using Stores.IdentityServer4.Neo4j.Entities;

namespace Stores.IdentityServer4.Test.Core.Store
{
    public abstract class UnitTestClientStore<TUser, TClient>
        where TUser : Neo4jIdentityUser
        where TClient : ClientRoot
    {
        private INeo4jTest _neo4jtest;
        private IUserStore<TUser> _userStore;
        private IIdentityServer4ClientUserStore<TUser, TClient> _clientUserStore;

        public UnitTestClientStore(
            IUserStore<TUser> userStore,
            IIdentityServer4ClientUserStore<TUser, TClient> clientUserStore,
            INeo4jTest neo4jtest)
        {
            _userStore = userStore;
            _clientUserStore = clientUserStore;
            _neo4jtest = neo4jtest;
        }

        [TestInitialize]
        public async Task Initialize()
        {
            await _neo4jtest.DropDatabaseAsync();
        }

        [TestMethod]
        public async Task Valid_DI()
        {
            _userStore.ShouldNotBeNull();
            _neo4jtest.ShouldNotBeNull();
        }

        protected abstract TClient CreateTestClient();

        [TestMethod]
        public async Task Create_Client()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            TClient client = CreateTestClient();
            var result = await _clientUserStore.CreateAsync(client, CancellationToken.None);

            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult =
                await _clientUserStore.FindByClientIdAsync(client.ClientId, CancellationToken.None);
            findResult.ShouldNotBeNull();
            findResult.ClientId.ShouldBe(client.ClientId);

        }

    }
}
