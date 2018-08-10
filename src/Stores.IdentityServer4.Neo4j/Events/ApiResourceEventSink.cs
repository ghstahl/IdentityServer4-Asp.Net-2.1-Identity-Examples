using System;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4.Events;
using Microsoft.Extensions.Logging;
using StoresIdentityServer4.Neo4j;

namespace Stores.IdentityServer4Neo4j.Events
{
    public class ApiResourceEventSink<TUser> : ClientEventSinkBase<TUser>
        where TUser : Neo4jIdentityUser

    {
        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <param name="evt">The event.</param>
        /// <exception cref="System.ArgumentNullException">evt</exception>
        public override async Task PersistAsync(Event evt)
        {
            if (evt == null)
                throw new ArgumentNullException(nameof(evt));
            if (evt.Category == StoresIdentityServer4.Neo4j.Events.Constants.Categories.ApiResourceStore)
            {
                ApiResourceChangeEvent<Neo4jIdentityServer4ApiResource> apiResourceChangeEvent = evt
                    as ApiResourceChangeEvent<Neo4jIdentityServer4ApiResource>;
                await _accessor.IdentityServer4ClientUserStore.DeleteApiResoucesRollupAsync();
                await _accessor.IdentityServer4ClientUserStore.DeleteRollupAsync(
                    apiResourceChangeEvent.ApiResource);
            }
        }
        public ApiResourceEventSink(IIdentityServer4ClientUserStoreAccessor<
            TUser, 
            Neo4jIdentityServer4Client, 
            Neo4jIdentityServer4ClientSecret, Neo4JIdentityServer4IdentityServer4GrantType, Neo4jIdentityServer4ApiResource, Neo4jIdentityServer4ApiResourceClaim, Neo4jIdentityServer4ApiSecret, Neo4jIdentityServer4ApiScope, Neo4jIdentityServer4ApiScopeClaim, Neo4jIdentityServer4ClientClaim, Neo4jIdentityServer4ClientCorsOrigin, Neo4jIdentityServer4ClientScope, Neo4JIdentityServer4ClientIdpRestriction, Neo4jIdentityServer4ClientProperty, Neo4jIdentityServer4ClientPostLogoutRedirectUri, Neo4jIdentityServer4ClientRedirectUri, Neo4jIdentityServer4IdentityResource, Neo4jIdentityServer4IdentityClaim> accessor) : base(accessor)
        {
        }
    }
}