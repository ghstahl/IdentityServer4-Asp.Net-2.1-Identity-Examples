using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras;
using Neo4jExtras.Extensions;


namespace Stores.IdentityServer4.Neo4j
{
    public interface IIdentityServer4ClientStore: IDisposable
    {
        Task<IdentityServer4Result> CreateAsync(Neo4jIdentityServer4Client client, CancellationToken cancellationToken);

        Task<IdentityServer4Result> DeleteAsync(Neo4jIdentityServer4Client client, CancellationToken cancellationToken);

        Task<Neo4jIdentityServer4Client> FindByClientIdAsync(string clientId, CancellationToken cancellationToken);
    }

    public class IdentityServer4ClientStore : IIdentityServer4ClientStore
    {
        private static readonly string IdentityServer4Client;
        static IdentityServer4ClientStore()
        {
            IdentityServer4Client = typeof(Neo4jIdentityServer4Client).GetNeo4jLabelName();
        }

        public IdentityServer4ClientStore(ISession session)
        {
            Session = session;
        }
        /// <summary>
        /// Gets the database session for this store.
        /// </summary>
        public ISession Session { get; }

        public async Task<IdentityServer4Result> CreateAsync(Neo4jIdentityServer4Client client, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            var cypher = $@"CREATE (r:{IdentityServer4Client} $p0)";
            await Session.RunAsync(cypher, Params.Create(client.ConvertToMap()));
            return IdentityServer4Result.Success;
        }

        public async Task<IdentityServer4Result> DeleteAsync(Neo4jIdentityServer4Client client, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new System.NotImplementedException();
        }

        public async Task<Neo4jIdentityServer4Client> FindByClientIdAsync(string clientId, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Dispose the stores
        /// </summary>
        public void Dispose() => _disposed = true;
        private bool _disposed;
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
    }
}