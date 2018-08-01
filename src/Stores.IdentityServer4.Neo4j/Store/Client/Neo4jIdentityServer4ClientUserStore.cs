using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras;
using Neo4jExtras.Extensions;

namespace StoresIdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStore<TUser> where TUser : Neo4jIdentityUser
    {
        private static readonly string IdSrv4Client;
        public async Task<IdentityResult> AddClientToUserAsync(TUser user, Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            client.ThrowIfNull(nameof(client));
            try
            {
                var cypher = $@"
                MATCH (u:{User} {{Id: $p0}})
                MERGE (l:{IdSrv4Client} {"$p1".AsMapFor<Neo4jIdentityServer4Client>()})
                MERGE (u)-[:{Neo4jConstants.Relationships.HasClient}]->(l)";

                var result = await Session.RunAsync(cypher, Params.Create(user.Id, client));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> CreateClientAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            try
            {
                var cypher = $@"CREATE (r:{IdSrv4Client} $p0)";
                await Session.RunAsync(cypher, Params.Create(client.ConvertToMap()));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> UpdateClientAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            try
            {
                var cypher = $@"
                MATCH (r:{IdSrv4Client})
                WHERE r.ClientId = $p0
                SET r = $p1";

                await Session.RunAsync(cypher, Params.Create(client.ClientId, client.ConvertToMap()));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteClientAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            await DeleteRedirectUrisAsync(client);
            await DeletePostLogoutRedirectUrisAsync(client);
            await DeleteClaimsAsync(client);
            await DeleteCorsOriginsAsync(client);
            await DeleteIdPRestrictionsAsync(client);
            await DeletePropertiesAsync(client);
            await DeleteScopesAsync(client);
            await DeleteSecretsAsync(client);

            try
            {
                var cypher = $@"
                MATCH (c:{IdSrv4Client})
                WHERE c.ClientId = $p0
                DETACH DELETE c";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<Neo4jIdentityServer4Client> FindClientByClientIdAsync(string clientId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            clientId.ThrowIfNull(nameof(clientId));

            var cypher = $@"
                MATCH (c:{IdSrv4Client})
                WHERE c.ClientId = $p0
                RETURN c {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(clientId));
            var factor = await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4Client>("c"));
            return factor;
        }

        public async Task<IList<Neo4jIdentityServer4Client>> GetClientsAsync(TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));


            var cypher = $@"
                MATCH (u:{User})-[:{Neo4jConstants.Relationships.HasClient}]->(r:{IdSrv4Client})
                WHERE u.Id = $p0
                RETURN r{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(user.Id));

            var clientsResult = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4Client>("r"));
            return clientsResult;
        }
    }
}
