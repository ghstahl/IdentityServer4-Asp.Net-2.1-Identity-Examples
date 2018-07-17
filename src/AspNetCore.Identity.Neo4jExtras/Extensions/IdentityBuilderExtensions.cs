using AspNetCore.Identity.Neo4jExtras;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Identity.Neo4j
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddNeo4jMultiFactorStore<TFactor>(this IdentityBuilder builder)
            where TFactor : ChallengeFactor
        {
            var neo4jMultiFactorStoreType = typeof(Neo4jMultiFactorStore<,>).MakeGenericType(builder.UserType,typeof(TFactor));

            builder.Services.AddScoped(
                typeof(IMultiFactorUserStore<,>).MakeGenericType(builder.UserType, typeof(TFactor)), 
                neo4jMultiFactorStoreType);
            return builder;
        }
        public static IdentityBuilder AddNeo4jMultiFactorStore(this IdentityBuilder builder)
            
        {
            var neo4jMultiFactorStoreType = typeof(Neo4jMultiFactorStore<,>).MakeGenericType(builder.UserType, typeof(ChallengeFactor));

            builder.Services.AddScoped(
                typeof(IMultiFactorUserStore<,>).MakeGenericType(builder.UserType, typeof(ChallengeFactor)),
                neo4jMultiFactorStoreType);
            return builder;
        }
    }
}
