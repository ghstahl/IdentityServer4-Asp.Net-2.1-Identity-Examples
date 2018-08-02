using System;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4.Events;
using Microsoft.Extensions.Logging;
using StoresIdentityServer4.Neo4j;
using StoresIdentityServer4.Neo4j.Entities;

namespace Stores.IdentityServer4Neo4j.Events
{

    public class ClientEventSink<TUser> : INeo4jEventSink
        where TUser : Neo4jIdentityUser

    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        private IIdentityServer4ClientUserStoreAccessor<
            TUser,
            Neo4jIdentityServer4Client,
            Neo4jIdentityServer4ClientSecret,
            Neo4jIdentityServer4ClientGrantType,
            Neo4jIdentityServer4ApiResource,
            Neo4jIdentityServer4ApiResourceClaim,
            Neo4jIdentityServer4ApiSecret,
            Neo4jIdentityServer4ApiScope,
            Neo4jIdentityServer4ApiScopeClaim,
            Neo4jIdentityServer4ClientClaim,
            Neo4jIdentityServer4ClientCorsOrigin,
            Neo4jIdentityServer4ClientScope,
            Neo4jIdentityServer4ClientIDPRestriction,
            Neo4jIdentityServer4ClientProperty,
            Neo4jIdentityServer4ClientPostLogoutRedirectUri,
            Neo4jIdentityServer4ClientRedirectUri> _accessor;


        /// <summary>
        /// Initializes a new instance of the <see cref="ClientEventSink"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ClientEventSink(
            IIdentityServer4ClientUserStoreAccessor<TUser,
                Neo4jIdentityServer4Client,
                Neo4jIdentityServer4ClientSecret,
                Neo4jIdentityServer4ClientGrantType,
                Neo4jIdentityServer4ApiResource,
                Neo4jIdentityServer4ApiResourceClaim,
                Neo4jIdentityServer4ApiSecret,
                Neo4jIdentityServer4ApiScope,
                Neo4jIdentityServer4ApiScopeClaim,
                Neo4jIdentityServer4ClientClaim,
                Neo4jIdentityServer4ClientCorsOrigin,
                Neo4jIdentityServer4ClientScope,
                Neo4jIdentityServer4ClientIDPRestriction,
                Neo4jIdentityServer4ClientProperty,
                Neo4jIdentityServer4ClientPostLogoutRedirectUri,
                Neo4jIdentityServer4ClientRedirectUri> accessor,
            ILogger<ClientEventSink<TUser>> logger)
        {
            _accessor = accessor;
            _logger = logger;
        }

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <param name="evt">The event.</param>
        /// <exception cref="System.ArgumentNullException">evt</exception>
        public virtual async Task PersistAsync(Event evt)
        {
            if (evt == null)
                throw new ArgumentNullException(nameof(evt));
            if (evt.Category == "ClientStore")
            {
                ClientChangeEvent<Neo4jIdentityServer4Client> clientChangeEvent = evt
                    as ClientChangeEvent<Neo4jIdentityServer4Client>;
                await _accessor.IdentityServer4ClientUserStore.DeleteRollupAsync(clientChangeEvent.Client);
            }
        }
    }
}
