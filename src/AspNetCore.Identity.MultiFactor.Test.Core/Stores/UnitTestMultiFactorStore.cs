using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AspNetCore.Identity.MultiFactor.Test.Core.Stores
{
    public abstract class UnitTestMultiFactorStore<TFactor>
        where TFactor : ChallengeFactor
    {
        private IMultiFactorStore<TFactor> _multiFactorStore;

        public UnitTestMultiFactorStore(IMultiFactorStore<TFactor> multiFactorStore)
        {
            _multiFactorStore = multiFactorStore;

        }
        [TestInitialize]
        public async Task Initialize()
        {
           // await _restHookStoreTest.DropAsync();
        }
        [TestMethod]
        public async Task Valid_DI()
        {
            _multiFactorStore.ShouldNotBeNull();
        }
    }
}
