using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4.Events;
using StoresIdentityServer4.Neo4j;

namespace Stores.IdentityServer4Neo4j.Events
{
    public abstract class ClientEventSinkBase<TUser> : INeo4jEventSink
        where TUser : Neo4jIdentityUser

    {

        protected IIdentityServer4ClientUserStoreAccessor<
            TUser,
            Neo4jIdentityServer4Client,
            Neo4jIdentityServer4ClientSecret,
            Neo4JIdentityServer4GrantType,
            Neo4jIdentityServer4ApiResource,
            Neo4jIdentityServer4ApiResourceClaim,
            Neo4jIdentityServer4ApiSecret,
            Neo4jIdentityServer4ApiScope,
            Neo4jIdentityServer4ApiScopeClaim,
            Neo4jIdentityServer4ClientClaim,
            Neo4jIdentityServer4ClientCorsOrigin,
            Neo4jIdentityServer4ClientScope,
            Neo4JIdentityServer4ClientIdpRestriction,
            Neo4jIdentityServer4ClientProperty,
            Neo4jIdentityServer4ClientPostLogoutRedirectUri,
            Neo4jIdentityServer4ClientRedirectUri,
            Neo4jIdentityServer4IdentityResource,
            Neo4jIdentityServer4IdentityClaim> _accessor;


        /// <summary>
        /// Initializes a new instance of the <see cref="ClientEventSink"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ClientEventSinkBase(
            IIdentityServer4ClientUserStoreAccessor<TUser,
                Neo4jIdentityServer4Client,
                Neo4jIdentityServer4ClientSecret,
                Neo4JIdentityServer4GrantType,
                Neo4jIdentityServer4ApiResource,
                Neo4jIdentityServer4ApiResourceClaim,
                Neo4jIdentityServer4ApiSecret,
                Neo4jIdentityServer4ApiScope,
                Neo4jIdentityServer4ApiScopeClaim,
                Neo4jIdentityServer4ClientClaim,
                Neo4jIdentityServer4ClientCorsOrigin,
                Neo4jIdentityServer4ClientScope,
                Neo4JIdentityServer4ClientIdpRestriction,
                Neo4jIdentityServer4ClientProperty,
                Neo4jIdentityServer4ClientPostLogoutRedirectUri,
                Neo4jIdentityServer4ClientRedirectUri,
                Neo4jIdentityServer4IdentityResource,
                Neo4jIdentityServer4IdentityClaim> accessor)
        {
            _accessor = accessor;
        }


        public abstract Task PersistAsync(Event evt);
    }
}