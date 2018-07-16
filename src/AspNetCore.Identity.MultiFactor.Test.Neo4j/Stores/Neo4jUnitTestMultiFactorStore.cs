using Microsoft.VisualStudio.TestTools.UnitTesting;
using AspNetCore.Identity.MultiFactor.Test.Core.Stores;
using AspNetCore.Identity.Neo4j;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Identity.MultiFactor.Test.Neo4j
{
    [TestClass]
    public class Neo4jUnitTestMultiFactorStore : UnitTestMultiFactorStore<ChallengeFactor>
    {
        public Neo4jUnitTestMultiFactorStore() : base(
            HostContainer.ServiceProvider.GetService<IMultiFactorStore<ChallengeFactor>>())
        {
        }
    }
}
