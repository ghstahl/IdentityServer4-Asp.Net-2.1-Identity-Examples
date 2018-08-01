using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras.Extensions;

namespace Stores.IdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ResourceStore<TUser> :
        IIdentityServer4ResouceStore<
            Neo4jIdentityServer4IdentityResource,
            Neo4jIdentityServer4ApiResource>

    {        
        /// <summary>
        /// Gets the database session for this store.
        /// </summary>
        public ISession Session { get; }

        private static readonly string IdSrv4IdentityResource;
        private static readonly string IdSrv4ApiResource;

        static Neo4jIdentityServer4ResourceStore()
        {

            IdSrv4IdentityResource = typeof(Neo4jIdentityServer4IdentityResource).GetNeo4jLabelName();
            IdSrv4ApiResource = typeof(Neo4jIdentityServer4ApiResource).GetNeo4jLabelName();
        }

        public Neo4jIdentityServer4ResourceStore(ISession session)
        {
            Session = session;
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            throw new NotImplementedException();
        }

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
    }
}