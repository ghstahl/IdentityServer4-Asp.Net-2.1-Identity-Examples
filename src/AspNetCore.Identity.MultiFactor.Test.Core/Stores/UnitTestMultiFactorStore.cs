using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AspNetCore.Identity.MultiFactor.Test.Core.Stores
{
    public abstract class UnitTestMultiFactorStore<TUser, TFactor>
        where TUser : Neo4jIdentityUser
        where TFactor : ChallengeFactor
    {
       
        private IMultiFactorTest<TFactor> _multiFactorTest;
        private IUserStore<TUser> _userStore;
        private IMultiFactorUserStore<TUser, TFactor> _multiFactorUserStore;

        public UnitTestMultiFactorStore(
            IUserStore<TUser> userStore,
            IMultiFactorUserStore<TUser,TFactor> multiFactorUserStore,
            IMultiFactorTest<TFactor> multiFactorTest)
        {
            _userStore = userStore;
            _multiFactorUserStore = multiFactorUserStore;
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
            _userStore.ShouldNotBeNull();
            _multiFactorUserStore.ShouldNotBeNull();
            _multiFactorTest.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task Create_User_many_ChallengeFactor()
        {
            var testUser = CreateTestUser();
            

            var createUserResult = await _userStore.CreateAsync(testUser, CancellationToken.None);
            createUserResult.ShouldNotBeNull();
            createUserResult.Succeeded.ShouldBeTrue();
            int nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                var challengeFactor = CreateTestFactor();
                await _multiFactorUserStore.AddToFactorAsync(
                    testUser, challengeFactor, CancellationToken.None);

                var findResult = await _multiFactorUserStore.FindByIdAsync(challengeFactor.Id,
                    CancellationToken.None);
                findResult.ShouldNotBeNull();
                findResult.Id.ShouldBe(challengeFactor.Id);
            }

            var factors = await _multiFactorUserStore.GetFactorsAsync(testUser, CancellationToken.None);
            factors.ShouldNotBeNull();
            factors.Count.ShouldBe(nCount);
           
        }

        [TestMethod]
        public async Task Create_User_ChallengeFactor()
        {
            var testUser = CreateTestUser();

            var createUserResult = await _userStore.CreateAsync(testUser, CancellationToken.None);

            var challengeFactor = CreateTestFactor();
            await _multiFactorUserStore.AddToFactorAsync(
                testUser, challengeFactor, CancellationToken.None);

            var findResult = await _multiFactorUserStore.FindByIdAsync(challengeFactor.Id, 
                CancellationToken.None);
            findResult.ShouldNotBeNull();
            findResult.Id.ShouldBe(challengeFactor.Id);

        }

        protected abstract TUser CreateTestUser();

        [TestMethod]
        public async Task Create_ChallengeFactor()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var challengeFactor = CreateTestFactor();
            var result = await _multiFactorUserStore.CreateAsync(challengeFactor, CancellationToken.None);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult = await _multiFactorUserStore.FindByIdAsync(challengeFactor.Id, CancellationToken.None);
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
            var result = await _multiFactorUserStore.CreateAsync(challengeFactor, CancellationToken.None);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult = await _multiFactorUserStore.FindByIdAsync(challengeFactor.Id, CancellationToken.None);
            findResult.ShouldNotBeNull();
            findResult.Id.ShouldBe(challengeFactor.Id);

            result = await _multiFactorUserStore.DeleteAsync(challengeFactor, CancellationToken.None);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            findResult = await _multiFactorUserStore.FindByIdAsync(challengeFactor.Id, CancellationToken.None);
            findResult.ShouldBeNull();

        }
    }
}
