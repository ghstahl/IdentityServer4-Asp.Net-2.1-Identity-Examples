using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Identity.Neo4j
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddNeo4jDataStores(this IdentityBuilder builder)
        {
            return builder
                .AddNeo4jUserStore()
                .AddNeo4jRoleStore();
        }

        private static IdentityBuilder AddNeo4jUserStore(this IdentityBuilder builder)
        {
            var userStoreType = typeof(Neo4jUserStore<>).MakeGenericType(builder.UserType);
            builder.Services.AddScoped(typeof(IUserStore<>).MakeGenericType(builder.UserType), userStoreType);

            return builder;
        }

        private static IdentityBuilder AddNeo4jRoleStore(this IdentityBuilder builder)
        {
            var roleStoreType = typeof(Neo4jRoleStore<>).MakeGenericType(builder.RoleType);
            builder.Services.AddScoped(typeof(IRoleStore<>).MakeGenericType(builder.RoleType), roleStoreType);

            return builder;
        }
    }
}
