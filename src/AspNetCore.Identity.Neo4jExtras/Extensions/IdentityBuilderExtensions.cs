using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Neo4j
{
    public static class IdentityBuilderExtensions
    {

        public static IdentityBuilder AddNeo4jMultiFactorStore(this IdentityBuilder builder) 
        {
            builder.Services.AddNeo4jMultiFactorStore();
            return builder;
        }
    }
   
}
