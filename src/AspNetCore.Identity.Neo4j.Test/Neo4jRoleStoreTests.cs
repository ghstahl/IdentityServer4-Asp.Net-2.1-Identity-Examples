using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using Neo4j.Driver.V1;
using Xunit;
using AspNetCore.Identity.Neo4j.Test.Models;
using AspNetCore.Identity.Neo4j.Internal;
using Neo4jExtras;

namespace AspNetCore.Identity.Neo4j.Test
{
    public class Neo4jRoleStoreTests
    {
        private IdentityErrorDescriber _identityErrorDescriber;
        private Mock<ISession> _sessionMock;
        private Neo4jRoleStore<TestRole> _store;

        public Neo4jRoleStoreTests()
        {
            _identityErrorDescriber = new IdentityErrorDescriber();
            _sessionMock = new Mock<ISession>();
            _store =  new Neo4jRoleStore<TestRole>(_sessionMock.Object, _identityErrorDescriber);
        }

        [Fact]
        public void ISession_Null_Exception()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Neo4jRoleStore<TestRole>(null, new IdentityErrorDescriber()));
            Assert.Equal(exception.ParamName, "session");
        }

        [Fact]
        public async Task Throw_Disposed()
        {
            _store.Dispose();

            Assert.Equal(_identityErrorDescriber, _store.ErrorDescriber);
            Assert.Equal(_sessionMock.Object, _store.Session);

            var exception = await Assert.ThrowsAsync<ObjectDisposedException>(() => _store.FindByIdAsync(string.Empty, CancellationToken.None));
            Assert.Equal(exception.ObjectName, _store.GetType().Name);
        }

        [Fact]
        public async Task CreateAsync_Throws()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _store.CreateAsync(null));
        }

        [Fact]
        public async Task CreateAsync()
        {
            var role = new TestRole()
            {
                Name = "testrole"
            };

            _sessionMock.Setup(s => s.RunAsync(It.IsAny<string>(), It.IsAny<Params<Dictionary<string,object>>>()))
                .Returns(() => Task.FromResult(new Mock<IStatementResultCursor>().Object));

            var result = await _store.CreateAsync(role);

            _sessionMock.Verify(s => 
                s.RunAsync("CREATE (r:TestRole $p0)", 
                It.Is<Params<Dictionary<string, object>>>(p => (string)p.p0["Id"] == role.Id && (string)p.p0["Name"] == role.Name)), Times.Once);

            Assert.Equal(result, IdentityResult.Success);
        }

        [Fact]
        public async Task UpdateAsync_Throws()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _store.UpdateAsync(null));
        }
    }
}