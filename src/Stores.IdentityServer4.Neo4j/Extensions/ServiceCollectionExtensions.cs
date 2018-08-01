using AspNetCore.Identity.Neo4j;
using IdentityServer4Extras;
using Microsoft.Extensions.DependencyInjection;

namespace StoresIdentityServer4.Neo4j
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNeo4jClientStore<TUser>(this IServiceCollection serviceCollection)
            where TUser : Neo4jIdentityUser
        {
            serviceCollection
                .AddScoped<IIdentityServer4ClientUserStore<
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
                    Neo4jIdentityServer4ClientRedirectUri>, Neo4jIdentityServer4ClientUserStore<TUser>>();
         }
    }
}