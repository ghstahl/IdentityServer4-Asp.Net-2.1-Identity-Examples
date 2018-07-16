using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
        private IMultiFactorTest<TFactor> _multiFactorTest;

        public UnitTestMultiFactorStore(
            IMultiFactorStore<TFactor> multiFactorStore,
            IMultiFactorTest<TFactor> multiFactorTest)
        {
            _multiFactorStore = multiFactorStore;
            _multiFactorTest = multiFactorTest;

        }
        [TestInitialize]
        public async Task Initialize()
        {
           await _multiFactorTest.DropDatabaseAsync();
        }
        [TestMethod]
        public async Task Valid_DI()
        {
            _multiFactorStore.ShouldNotBeNull();
            _multiFactorTest.ShouldNotBeNull();
        }
        [TestMethod]
        public async Task Create_ChallengeFactor()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var challengeFactor = CreateTestFactor();
            var result = await _multiFactorStore.CreateAsync(challengeFactor, CancellationToken.None);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult = await _multiFactorStore.FindByIdAsync(challengeFactor.Id, CancellationToken.None);
            findResult.ShouldNotBeNull();
            findResult.Id.ShouldBe(challengeFactor.Id);
        }

        protected abstract TFactor CreateTestFactor();

        [TestMethod]
        public async Task Create_ChallengeFactor_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var challengeFactor = CreateTestFactor();
            var result = await _multiFactorStore.CreateAsync(challengeFactor, CancellationToken.None);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult = await _multiFactorStore.FindByIdAsync(challengeFactor.Id, CancellationToken.None);
            findResult.ShouldNotBeNull();
            findResult.Id.ShouldBe(challengeFactor.Id);

            result = await _multiFactorStore.DeleteAsync(challengeFactor, CancellationToken.None);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            findResult = await _multiFactorStore.FindByIdAsync(challengeFactor.Id, CancellationToken.None);
            findResult.ShouldBeNull();
           
        }
    }
}
