using AspNetCore.Identity.Neo4j;
using Microsoft.Extensions.DependencyInjection;

namespace Stores.IdentityServer4.Neo4j
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNeo4jClientStore<TUser>(this IServiceCollection serviceCollection)
            where TUser : Neo4jIdentityUser 
        {
            serviceCollection.AddScoped<IIdentityServer4ClientUserStore<TUser>, IdentityServer4ClientUserStore<TUser>>();
        }
    }
}