using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Xunit;
using Moq;
using Neo4j.Driver.V1;
using AspNetCore.Identity.Neo4j.Test.Models;

namespace AspNetCore.Identity.Neo4j.Test
{
    public class Neo4jUserStoreTests
    {
        [Fact]
        public void ISession_Null_Exception()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Neo4jUserStore<TestUser, TestRole>(null, new IdentityErrorDescriber()));
            Assert.Equal(exception.ParamName, "session");
        }

        [Fact]
        public async Task Throw_Disposed()
        {
            var identityErrorDescriber = new IdentityErrorDescriber();
            var store = new Neo4jUserStore<TestUser, TestRole>(new Mock<ISession>().Object, identityErrorDescriber);
            store.Dispose();

            Assert.Equal(identityErrorDescriber, store.ErrorDescriber);

            var exception = await Assert.ThrowsAsync<ObjectDisposedException>(() => store.FindByIdAsync(string.Empty, CancellationToken.None));
            Assert.Equal(exception.ObjectName, store.GetType().Name);
        }
    }
}