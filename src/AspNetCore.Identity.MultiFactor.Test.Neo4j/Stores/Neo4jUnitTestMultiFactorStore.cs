using System.Data;
using AspNetCore.Identity.MultiFactor.Test.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AspNetCore.Identity.MultiFactor.Test.Core.Stores;
using AspNetCore.Identity.Neo4j;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore.Identity.MultiFactor.Test.Neo4j.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.MultiFactor.Test.Neo4j
{
    [TestClass]
    public class Neo4jUnitTestMultiFactorStore : UnitTestMultiFactorStore<TestUser,TestFactor>
    {
        public Neo4jUnitTestMultiFactorStore() : base(
            HostContainer.ServiceProvider.GetService<IUserStore<TestUser>>(),
            HostContainer.ServiceProvider.GetService<IMultiFactorUserStore<TestUser, TestFactor>>(),
            HostContainer.ServiceProvider.GetService<IMultiFactorTest<TestFactor>>())
        {
        }

        protected override TestUser CreateTestUser()
        {
            return new TestUser()
            {
                UserName = Unique.S
            };
        }

        protected override TestFactor CreateTestFactor()
        {
            return new TestFactor();
        }
    }
}
