using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Neo4j
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddNeo4jMultiFactorStore<TFactor>(this IdentityBuilder builder)
            where TFactor : ChallengeFactor
        {
            builder.Services.AddNeo4jMultiFactorStore<TFactor>();
            return builder;
        }
    }
}
