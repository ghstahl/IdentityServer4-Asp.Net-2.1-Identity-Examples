using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4Extras;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras;
using Neo4jExtras.Extensions;
using Stores.IdentityServer4.Neo4j.Entities;

namespace Stores.IdentityServer4.Neo4j
{
    public interface IIdentityServer4ClientStore<TClient> :
        IDisposable
        where TClient : ClientRoot
    {
        Task<IdentityResult> CreateAsync(TClient client, CancellationToken cancellationToken);
        Task<IdentityResult> DeleteAsync(TClient client, CancellationToken cancellationToken);
        Task<Neo4jIdentityServer4Client> FindByClientIdAsync(string clientId, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateAsync(TClient client, CancellationToken cancellationToken);

        Task<IdentityResult> AddSecretToClientAsync(TClient client,
            Neo4jIdentityServer4ClientSecret secret, CancellationToken cancellationToken);

    }

    public interface IIdentityServer4ClientUserStore<TUser, TClient> :
        IIdentityServer4ClientStore<TClient>,
        IDisposable
        where TUser : class
        where TClient : ClientRoot

    {
        Task<IdentityResult> AddToClientAsync(TUser user, TClient client,
            CancellationToken cancellationToken);

        Task<IList<TClient>> GetClientsAsync(TUser user, CancellationToken cancellationToken);
    }

    public class Neo4jIdentityServer4ClientUserStore<TUser> :
        IIdentityServer4ClientUserStore<TUser,Neo4jIdentityServer4Client>
        where TUser : Neo4jIdentityUser
    {
        private bool _disposed;
        /// <summary>
        /// Dispose the stores
        /// </summary>
        public void Dispose() => _disposed = true;

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public static string User { get; set; }
        /// <summary>
        /// Gets the database session for this store.
        /// </summary>
        public ISession Session { get; }

        private static readonly string IdentityServer4Client;
        public static readonly string IdentityServer4ClientSecret;

        static Neo4jIdentityServer4ClientUserStore()
        {
            User = typeof(TUser).GetNeo4jLabelName();
            IdentityServer4Client = typeof(Neo4jIdentityServer4Client).GetNeo4jLabelName();
            IdentityServer4ClientSecret = typeof(Neo4jIdentityServer4ClientSecret).GetNeo4jLabelName();
        }

        public Neo4jIdentityServer4ClientUserStore(ISession session)
        {
            Session = session;
        }

        public async Task<IdentityResult> CreateAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"CREATE (r:{IdentityServer4Client} $p0)";
            await Session.RunAsync(cypher, Params.Create(client.ConvertToMap()));
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> UpdateAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                MATCH (r:{IdentityServer4Client})
                WHERE r.ClientId = $p0
                SET r = $p1";

            await Session.RunAsync(cypher, Params.Create(client.ClientId, client.ConvertToMap()));
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> AddSecretToClientAsync(Neo4jIdentityServer4Client client, Neo4jIdentityServer4ClientSecret secret,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            secret.ThrowIfNull(nameof(secret));

            var cypher = $@"
                MATCH (u:{IdentityServer4Client} {{ClientId: $p0}})
                MERGE (l:{IdentityServer4ClientSecret} {"$p1".AsMapFor<Neo4jIdentityServer4ClientSecret>()})
                MERGE (u)-[:{Neo4jConstants.Relationships.HasSecret}]->(l)";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, secret));
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(Neo4jIdentityServer4Client client, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                MATCH (r:{IdentityServer4Client})
                WHERE r.ClientId = $p0
                DETACH DELETE r";

            await Session.RunAsync(cypher, Params.Create(client.ClientId));
            return IdentityResult.Success;
        }
        public async Task<Neo4jIdentityServer4Client> FindByClientIdAsync(string clientId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            clientId.ThrowIfNull(nameof(clientId));

            var cypher = $@"
                MATCH (r:{IdentityServer4Client})
                WHERE r.ClientId = $p0
                RETURN r {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(clientId));
            var factor = await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4Client>("r"));
            return factor;
        }

        public async Task<IdentityResult> AddToClientAsync(TUser user, Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                MATCH (u:{User} {{Id: $p0}})
                MERGE (l:{IdentityServer4Client} {"$p1".AsMapFor<Neo4jIdentityServer4Client>()})
                MERGE (u)-[:{Neo4jConstants.Relationships.HasClient}]->(l)";

            var result = await Session.RunAsync(cypher, Params.Create(user.Id, client));
            return IdentityResult.Success;
        }

        public async Task<IList<Neo4jIdentityServer4Client>> GetClientsAsync(TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));


            var cypher = $@"
                MATCH (u:{User})-[:{Neo4jConstants.Relationships.HasClient}]->(r:{IdentityServer4Client})
                WHERE u.Id = $p0
                RETURN r{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(user.Id));

            var factors = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4Client>("r"));
            return factors;
        }

       
    }
}