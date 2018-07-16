using System;
using Xunit;
using AspNetCore.Identity.Neo4j.Internal.Extensions;
using AspNetCore.Identity.Neo4j.Test.Models;

namespace AspNetCore.Identity.Neo4j.Test
{
    public class ExtensionTests
    {
        [Fact]
        public void AsMap_Success()
        {
            var map = "$p0".AsMapFor<Neo4jIdentityUserLogin>();

            Assert.Equal(map, "{LoginProvider: $p0.LoginProvider, ProviderKey: $p0.ProviderKey, ProviderDisplayName: $p0.ProviderDisplayName}");
        }

        [Theory]
        [InlineData(typeof(Neo4jIdentityUser), "User")]
        [InlineData(typeof(Neo4jIdentityRole), "Role")]
        [InlineData(typeof(Neo4jIdentityUserClaim), "Claim")]
        [InlineData(typeof(Neo4jIdentityUserLogin), "Login")]
        [InlineData(typeof(Neo4jIdentityUserToken), "Token")]
        [InlineData(typeof(Neo4jIdentityRoleClaim), "Claim")]
        [InlineData(typeof(TestUser), "TestUser")]
        [InlineData(typeof(TestRole), "TestRole")]
        public void GetNeo4jLabelName_Success(Type type, string labal)
        {
            Assert.Equal(type.GetNeo4jLabelName(), labal);
        }

        [Fact]
        public void GetNeo4jLabelName_Failure()
        {
            Assert.Throws<InvalidProgramException>(() => typeof(object).GetNeo4jLabelName());
        }
    }
}