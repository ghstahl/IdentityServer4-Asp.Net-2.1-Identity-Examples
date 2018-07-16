using AspNetCore.Identity.Neo4jExtras;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Identity.Neo4j
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNeo4jMultiFactorStore(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IMultiFactorStore<ChallengeFactor>, Neo4jMultiFactorStore>();
            serviceCollection.AddScoped<IMultiFactorTest<ChallengeFactor>, Neo4jMultiFactorStore>();
        }
    }
}