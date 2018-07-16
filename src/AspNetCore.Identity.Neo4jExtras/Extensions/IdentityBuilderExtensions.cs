using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Neo4j
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddNeo4jMultiFactorStore<TUser, TFactor>(this IdentityBuilder builder)
            where TUser : Neo4jIdentityUser
            where TFactor : ChallengeFactor
        {
            builder.Services.AddNeo4jMultiFactorStore<TUser, TFactor>();
            return builder;
        }
    }
}
