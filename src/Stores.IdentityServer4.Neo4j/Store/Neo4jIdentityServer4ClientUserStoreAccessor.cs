using System;
using AspNetCore.Identity.Neo4j;

namespace StoresIdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStoreAccessor<TUser> :
        IIdentityServer4ClientUserStoreAccessor<
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
            Neo4JIdentityServer4ClientIdpRestriction,
            Neo4jIdentityServer4ClientProperty,
            Neo4jIdentityServer4ClientPostLogoutRedirectUri,
            Neo4jIdentityServer4ClientRedirectUri,
            Neo4jIdentityServer4IdentityResource,
            Neo4jIdentityServer4IdentityClaim
        >
        where TUser : Neo4jIdentityUser
    {
       

        private IServiceProvider _serviceProvider;
        public Neo4jIdentityServer4ClientUserStoreAccessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IIdentityServer4ClientUserStore<
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
            Neo4JIdentityServer4ClientIdpRestriction, 
            Neo4jIdentityServer4ClientProperty, 
            Neo4jIdentityServer4ClientPostLogoutRedirectUri, 
            Neo4jIdentityServer4ClientRedirectUri,
            Neo4jIdentityServer4IdentityResource,
            Neo4jIdentityServer4IdentityClaim
        > IdentityServer4ClientUserStore
        {
            get
            {
                var service = _serviceProvider.GetService(typeof(IIdentityServer4ClientUserStore<
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
                        Neo4JIdentityServer4ClientIdpRestriction,
                        Neo4jIdentityServer4ClientProperty,
                        Neo4jIdentityServer4ClientPostLogoutRedirectUri,
                        Neo4jIdentityServer4ClientRedirectUri,
                        Neo4jIdentityServer4IdentityResource,
                        Neo4jIdentityServer4IdentityClaim
                    >))
                    as IIdentityServer4ClientUserStore<
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
                        Neo4JIdentityServer4ClientIdpRestriction,
                        Neo4jIdentityServer4ClientProperty,
                        Neo4jIdentityServer4ClientPostLogoutRedirectUri,
                        Neo4jIdentityServer4ClientRedirectUri,
                        Neo4jIdentityServer4IdentityResource,
                        Neo4jIdentityServer4IdentityClaim
                    >;
                return service;
            }
        }
    }
}