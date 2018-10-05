using System;
using AspNetCore.Identity.Neo4j.Test.Models;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;

namespace AspNetCore.Identity.Neo4j.Test.Common
{
    class TenantStore : ITenantStore
    {
        public string TenantId { get; set; }
    }
    public class DatabaseFixture : IDisposable
    {
        private readonly IDriver _driver;
        private readonly ISession _session;

        public DatabaseFixture()
        {
            _driver = GraphDatabase.Driver("bolt://127.0.0.1:7687", AuthTokens.Basic("neo4j", "password"));
            _session = _driver.Session();
            var identityErrorDescriber = new IdentityErrorDescriber();

            UserStore = new Neo4jUserStore<TestUser, TestRole>(_session,new TenantStore()
            {
                TenantId = "TestTenant"
            }, identityErrorDescriber);
            RoleStore = new Neo4jRoleStore<TestRole>(_session, identityErrorDescriber);
        }

        public void Dispose()
        {
            _session.Run("MATCH (u:TestUser) DETACH DELETE u");
            _session.Run("MATCH (r:TestRole) DETACH DELETE r");
            _session.Dispose();
            _driver.Dispose();
        }

        public Neo4jUserStore<TestUser, TestRole> UserStore { get; }
        public Neo4jRoleStore<TestRole> RoleStore { get; set; }
    }
}
