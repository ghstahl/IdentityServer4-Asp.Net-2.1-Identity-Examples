using AspNetCore.Identity.Neo4jExtras;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Identity.Neo4j
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNeo4jMultiFactorStore<TUser, TFactor>(this IServiceCollection serviceCollection)
            where TUser : Neo4jIdentityUser
            where TFactor : ChallengeFactor
        {
        
            serviceCollection.AddScoped<IMultiFactorUserStore<TUser,TFactor>, Neo4jMultiFactorStore<TUser,TFactor>>();
            serviceCollection.AddScoped<IMultiFactorTest<TFactor>, Neo4jMultiFactorStore<TUser, TFactor>>();
        }
    }
}