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
        protected abstract TFactor CreateTestFactor();

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
        public async Task Create_User_many_Factor()
        {
            var testUser = CreateTestUser();
            

            var createUserResult = await _userStore.CreateAsync(testUser, CancellationToken.None);
            createUserResult.ShouldNotBeNull();
            createUserResult.Succeeded.ShouldBeTrue();
            int nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                var challengeFactor = CreateTestFactor();
                var identityResult =await _multiFactorUserStore.AddToFactorAsync(
                    testUser, challengeFactor, CancellationToken.None);
                identityResult.ShouldNotBeNull();
                identityResult.Succeeded.ShouldBeTrue();

                var findResult = await _multiFactorUserStore.FindByIdAsync(challengeFactor.FactorId,
                    CancellationToken.None);
                findResult.ShouldNotBeNull();
                findResult.FactorId.ShouldBe(challengeFactor.FactorId);
            }

            var factors = await _multiFactorUserStore.GetFactorsAsync(testUser, CancellationToken.None);
            factors.ShouldNotBeNull();
            factors.Count.ShouldBe(nCount);
            foreach (var factor in factors)
            {
                factor.ShouldNotBeNull();
                factor.Challenge.ShouldNotBeNull();
                factor.ChallengeResponseHash.ShouldNotBeNull();
                factor.FactorId.ShouldNotBeNull();
            }

        }

        [TestMethod]
        public async Task Create_User_Factor()
        {
            var testUser = CreateTestUser();

            var createUserResult = await _userStore.CreateAsync(testUser, CancellationToken.None);

            var challengeFactor = CreateTestFactor();
            var identityResult = await _multiFactorUserStore.AddToFactorAsync(
                testUser, challengeFactor, CancellationToken.None);
            identityResult.ShouldNotBeNull();
            identityResult.Succeeded.ShouldBeTrue();

            var findResult = await _multiFactorUserStore.FindByIdAsync(challengeFactor.FactorId, 
                CancellationToken.None);
            findResult.ShouldNotBeNull();
            findResult.FactorId.ShouldBe(challengeFactor.FactorId);

        }
        [TestMethod]
        public async Task Create_User_Factor_Update()
        {
            var testUser = CreateTestUser();

            var createUserResult = await _userStore.CreateAsync(testUser, CancellationToken.None);

            var challengeFactor = CreateTestFactor();
            var identityResult = await _multiFactorUserStore.AddToFactorAsync(
                testUser, challengeFactor, CancellationToken.None);
            identityResult.ShouldNotBeNull();
            identityResult.Succeeded.ShouldBeTrue();

            var findResult = await _multiFactorUserStore.FindByIdAsync(challengeFactor.FactorId,
                CancellationToken.None);
            findResult.ShouldNotBeNull();
            findResult.FactorId.ShouldBe(challengeFactor.FactorId);
            findResult.ChallengeResponseHash.ShouldBe(challengeFactor.ChallengeResponseHash);

            var challengeFactor2 = CreateTestFactor();
            challengeFactor.ChallengeResponseHash = challengeFactor2.ChallengeResponseHash;

            identityResult = await _multiFactorUserStore.UpdateAsync(challengeFactor,
                CancellationToken.None);
            identityResult.ShouldNotBeNull();
            identityResult.Succeeded.ShouldBeTrue();

            findResult = await _multiFactorUserStore.FindByIdAsync(challengeFactor.FactorId,
                CancellationToken.None);
            findResult.ShouldNotBeNull();
            findResult.FactorId.ShouldBe(challengeFactor.FactorId);
            findResult.ChallengeResponseHash.ShouldBe(challengeFactor.ChallengeResponseHash);


        }

        protected abstract TUser CreateTestUser();

        [TestMethod]
        public async Task Create_Factor()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var challengeFactor = CreateTestFactor();
            var result = await _multiFactorUserStore.CreateAsync(challengeFactor, CancellationToken.None);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult = await _multiFactorUserStore.FindByIdAsync(challengeFactor.FactorId, CancellationToken.None);
            findResult.ShouldNotBeNull();
            findResult.FactorId.ShouldBe(challengeFactor.FactorId);
        }

       

        [TestMethod]
        public async Task Create_Factor_Delete()
        {
            var challenge = Unique.S;
            var challengeResponse = Unique.S;
            var challengeFactor = CreateTestFactor();
            var result = await _multiFactorUserStore.CreateAsync(challengeFactor, CancellationToken.None);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            var findResult = await _multiFactorUserStore.FindByIdAsync(challengeFactor.FactorId, CancellationToken.None);
            findResult.ShouldNotBeNull();
            findResult.FactorId.ShouldBe(challengeFactor.FactorId);

            result = await _multiFactorUserStore.DeleteAsync(challengeFactor, CancellationToken.None);
            result.ShouldNotBeNull();
            result.Succeeded.ShouldBeTrue();

            findResult = await _multiFactorUserStore.FindByIdAsync(challengeFactor.FactorId, CancellationToken.None);
            findResult.ShouldBeNull();

        }
    }
}
