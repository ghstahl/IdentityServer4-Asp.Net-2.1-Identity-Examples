using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using Neo4j.Driver.V1;
using Neo4jExtras.Extensions;
using Stores.IdentityServer4.Neo4j;
using Stores.IdentityServer4.Neo4j.Entities;

namespace Stores.IdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStore<TUser> :
        IIdentityServer4ClientUserStore<
            TUser,
            Neo4jIdentityServer4Client,
            Neo4jIdentityServer4ClientSecret,
            Neo4jIdentityServer4ClientGrantType,
            Neo4jIdentityServer4ClientClaim,
            Neo4jIdentityServer4ClientCorsOrigin,
            Neo4jIdentityServer4ClientScope,
            Neo4jIdentityServer4ClientIDPRestriction,
            Neo4jIdentityServer4ClientProperty,
            Neo4jIdentityServer4ClientPostLogoutRedirectUri,
            Neo4jIdentityServer4ClientRedirectUri>
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

        static Neo4jIdentityServer4ClientUserStore()
        {
            User = typeof(TUser).GetNeo4jLabelName();
            IdSrv4Client = typeof(Neo4jIdentityServer4Client).GetNeo4jLabelName();
            IdSrv4ClientSecret = typeof(Neo4jIdentityServer4ClientSecret).GetNeo4jLabelName();
            IdSrv4ClientGrantType = typeof(Neo4jIdentityServer4ClientGrantType).GetNeo4jLabelName();
            IdSrv4ClientClaim = typeof(Neo4jIdentityServer4ClientClaim).GetNeo4jLabelName();
            IdSrv4ClientCorsOrigin = typeof(Neo4jIdentityServer4ClientCorsOrigin).GetNeo4jLabelName();
            IdSrv4ClientScope = typeof(Neo4jIdentityServer4ClientScope).GetNeo4jLabelName();
            IdSrv4ClientIDPRestriction = typeof(Neo4jIdentityServer4ClientIDPRestriction).GetNeo4jLabelName();
            IdSrv4ClientProperty = typeof(Neo4jIdentityServer4ClientProperty).GetNeo4jLabelName();
            IdSrv4ClientPostLogoutRedirectUri = typeof(Neo4jIdentityServer4ClientPostLogoutRedirectUri).GetNeo4jLabelName();
            IdSrv4ClientRedirectUri = typeof(Neo4jIdentityServer4ClientRedirectUri).GetNeo4jLabelName();
        }

        public async Task CreateConstraintsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var cypher = $@"CREATE CONSTRAINT ON (r:{IdSrv4Client}) ASSERT r.ClientId IS UNIQUE";
            await Session.RunAsync(cypher);
            cypher = $@"CREATE CONSTRAINT ON (r:{IdSrv4ClientGrantType}) ASSERT r.GrantType IS UNIQUE";
            await Session.RunAsync(cypher);
        }

        public Neo4jIdentityServer4ClientUserStore(ISession session)
        {
            Session = session;
        }
    }
}