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
using Stores.IdentityServer4Neo4j.Events;

namespace StoresIdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStore<TUser> where TUser : Neo4jIdentityUser
    {
        private static readonly string IdSrv4ClientSecret;


        public async Task<IdentityResult> AddSecretToClientAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientSecret secret,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            secret.ThrowIfNull(nameof(secret));
            try
            {
                var cypher = $@"
                MATCH (c:{IdSrv4Client} {{ClientId: $p0}})
                CREATE UNIQUE 
                    (c)-[:{Neo4jConstants.Relationships.HasSecret}]->
                    (s:{IdSrv4ClientSecret} {"$p1".AsMapForNoNull(secret)})";
                
                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, secret));

                await RaiseClientChangeEventAsync(client);

                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> UpdateSecretAsync(
            Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientSecret secret,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            secret.ThrowIfNull(nameof(secret));
            try
            {
                var cypher = $@"
                MATCH (c:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasSecret}]->(s:{IdSrv4ClientSecret})
                WHERE c.ClientId = $p0 AND s.Value = $p1
                SET s = $p2";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId,
                        secret.Value,
                        secret.ConvertToMap()));
                await RaiseClientChangeEventAsync(client);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteSecretAsync(
            Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientSecret secret,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            secret.ThrowIfNull(nameof(secret));
            try
            {
                var cypher = $@"
                MATCH (c:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasSecret}]->(s:{IdSrv4ClientSecret})
                WHERE c.ClientId = $p0 AND s.Value = $p1
                DETACH DELETE s";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId,
                        secret.Value));
                await RaiseClientChangeEventAsync(client);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteSecretsAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            try
            {
                var cypher = $@"
                MATCH (c:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasSecret}]->(s:{IdSrv4ClientSecret})
                WHERE c.ClientId = $p0 
                DETACH DELETE s";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId));
                await RaiseClientChangeEventAsync(client);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<Neo4jIdentityServer4ClientSecret> FindSecretAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientSecret secret,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            secret.ThrowIfNull(nameof(secret));

            var cypher = $@"
                MATCH (c:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasSecret}]->(s:{IdSrv4ClientSecret})
                WHERE c.ClientId = $p0 AND s.Value = $p1
                RETURN s{{ .* }}";

            var result = await Session.RunAsync(cypher,
                Params.Create(
                    client.ClientId,
                    secret.Value));

            var foundRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ClientSecret>("s"));

            return foundRecord;
        }

        public async Task<IList<Neo4jIdentityServer4ClientSecret>> GetSecretsAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                MATCH (c:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasSecret}]->(s:{IdSrv4ClientSecret})
                WHERE c.ClientId = $p0
                RETURN s{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var secrets = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientSecret>("s"));
            return secrets;
        }

    }
}
