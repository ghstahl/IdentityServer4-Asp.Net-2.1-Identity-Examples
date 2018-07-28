using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AspNetCore.Identity.Neo4j;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Stores.IdentityServer4.Neo4j.Test;

using Stores.IdentityServer4.Neo4j.Test.Models;
using Stores.IdentityServer4.Test.Core.Store;

namespace AspNetCore.Identity.MultiFactor.Test.Neo4j
{
    [TestClass]
    public class Neo4jUnitTestMultiFactorStore : UnitTestClientStore<TestUser>
    {
        public Neo4jUnitTestMultiFactorStore() : 
            base(
                HostContainer.ServiceProvider.GetService<IUserStore<TestUser>>(), 
                HostContainer.ServiceProvider.GetService<INeo4jTest>())
        {
        }
    }
}
