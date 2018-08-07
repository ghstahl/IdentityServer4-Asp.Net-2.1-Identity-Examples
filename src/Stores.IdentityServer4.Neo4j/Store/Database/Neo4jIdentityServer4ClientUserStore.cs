using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;

namespace StoresIdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStore<TUser> where TUser : Neo4jIdentityUser
    {
        public async Task CreateConstraintsAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var cypher = $@"CREATE CONSTRAINT ON (r:{IdSrv4Client}) ASSERT r.ClientId IS UNIQUE";
            await Session.RunAsync(cypher);

            cypher = $@"CREATE CONSTRAINT ON (r:{IdSrv4ApiResource}) ASSERT r.Name IS UNIQUE";
            await Session.RunAsync(cypher);

            cypher = $@"CREATE CONSTRAINT ON (r:{IdSrv4IdentityResource}) ASSERT r.Name IS UNIQUE";
            await Session.RunAsync(cypher);
        }
    }
}
