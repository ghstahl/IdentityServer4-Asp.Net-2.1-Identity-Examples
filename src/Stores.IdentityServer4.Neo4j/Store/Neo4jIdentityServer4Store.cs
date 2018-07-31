using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras;
using Neo4jExtras.Extensions;

namespace Stores.IdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4Store<TUser> :
        IIdentityServer4ClientUserStore2<TUser, Neo4jIdentityServer4Client, Neo4jIdentityServer4ClientGrantType>
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
        private static readonly string IdSrv4Client;
        private static readonly string IdSrv4ClientGrantType;

        static Neo4jIdentityServer4Store()
        {
            User = typeof(TUser).GetNeo4jLabelName();
            IdSrv4Client = typeof(Neo4jIdentityServer4Client).GetNeo4jLabelName();
            IdSrv4ClientGrantType = typeof(Neo4jIdentityServer4ClientGrantType).GetNeo4jLabelName();
        }

        public Neo4jIdentityServer4Store(ISession session)
        {
            Session = session;
        }

        public async Task CreateConstraintsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var cypher = $@"CREATE CONSTRAINT ON (r:{IdSrv4Client}) ASSERT r.ClientId IS UNIQUE";
            await Session.RunAsync(cypher);
            cypher = $@"CREATE CONSTRAINT ON (r:{IdSrv4ClientGrantType}) ASSERT r.GrantType IS UNIQUE";
            await Session.RunAsync(cypher);
        }
        
    }
}