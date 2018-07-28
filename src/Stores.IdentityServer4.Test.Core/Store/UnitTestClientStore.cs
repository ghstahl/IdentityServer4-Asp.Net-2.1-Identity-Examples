using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Stores.IdentityServer4.Test.Core.Store
{
    public abstract class UnitTestClientStore<TUser>
        where TUser : Neo4jIdentityUser
    {
        private INeo4jTest _neo4jtest;
        private IUserStore<TUser> _userStore;

        public UnitTestClientStore(
            IUserStore<TUser> userStore, 
            INeo4jTest neo4jtest)
        {
            _userStore = userStore;
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
    }
}
