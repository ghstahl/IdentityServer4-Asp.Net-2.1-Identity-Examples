using AspNetCore.Identity.Neo4j;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Stores.IdentityServer4.Neo4j
{
    public static class IdentityBuilderExtensions
    {

        public static IdentityBuilder AddNeo4jClientStore<TUser>(this IdentityBuilder builder)
            where TUser : Neo4jIdentityUser

        {
            builder.Services.AddNeo4jClientStore<TUser>();
            return builder;
        }
    }
}
