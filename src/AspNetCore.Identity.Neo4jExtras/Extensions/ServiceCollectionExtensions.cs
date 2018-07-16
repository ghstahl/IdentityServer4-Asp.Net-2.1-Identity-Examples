using AspNetCore.Identity.Neo4jExtras;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Identity.Neo4j
{
    public static class ServiceCollectionExtensions
    {

        public static void AddNeo4jMultiFactorStore<TFactor>(this IServiceCollection serviceCollection)
            where TFactor : ChallengeFactor
        {
            serviceCollection.AddScoped<IMultiFactorStore<TFactor>, Neo4jMultiFactorStore<TFactor>>();
        }
    }
}