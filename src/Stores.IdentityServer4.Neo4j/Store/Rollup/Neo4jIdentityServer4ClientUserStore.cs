using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras;
using Neo4jExtras.Extensions;
using StoresIdentityServer4.Neo4j;
using StoresIdentityServer4.Neo4j.Entities;
using StoresIdentityServer4.Neo4j.Mappers;
using Client = IdentityServer4.Models.Client;

namespace StoresIdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStore<TUser> where TUser : Neo4jIdentityUser
    {
        private static readonly string IdSrv4ClientRollup;

        public async Task<IdentityResult> RollupAsync(string clientId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var finalResult = new Client();
            var client = await FindClientByClientIdAsync(clientId, cancellationToken);
            var model = client.ToModel();
            var rollup = new ClientRollup()
            {
                Client = model
            };
            var result = await AddRollupToClientAsync(client, rollup, cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<Client> GetRollupAsync(string clientId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> AddRollupToClientAsync(
            Neo4jIdentityServer4Client client,
            ClientRollup rollup,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            rollup.ThrowIfNull(nameof(rollup));
            try
            {
                var cypher = $@"
                MATCH (c:{IdSrv4Client} {{ClientId: $p0}})
                MERGE (rollup:{IdSrv4ClientRollup} {"$p1".AsMapFor<Neo4jIdentityServer4ClientRollup>()})
                MERGE (c)-[:{Neo4jConstants.Relationships.HasRollup}]->(rollup)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, rollup));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }
    }
}