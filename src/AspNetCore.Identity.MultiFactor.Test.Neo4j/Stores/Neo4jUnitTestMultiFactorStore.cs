using System.Data;
using AspNetCore.Identity.MultiFactor.Test.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AspNetCore.Identity.MultiFactor.Test.Core.Stores;
using AspNetCore.Identity.Neo4j;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore.Identity.MultiFactor.Test.Neo4j.Models;
namespace AspNetCore.Identity.MultiFactor.Test.Neo4j
{
    [TestClass]
    public class Neo4jUnitTestMultiFactorStore : UnitTestMultiFactorStore<TestFactor>
    {
        public Neo4jUnitTestMultiFactorStore() : base(
            HostContainer.ServiceProvider.GetService<IMultiFactorStore<TestFactor>>(),
            HostContainer.ServiceProvider.GetService<IMultiFactorTest<TestFactor>>())
        {
        }

        protected override TestFactor CreateTestFactor()
        {
            return new TestFactor();
        }
    }
}
