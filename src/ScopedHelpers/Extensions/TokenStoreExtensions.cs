using Microsoft.Extensions.DependencyInjection;

namespace ScopedHelpers.Extensions
{
    public static class TokenStoreExtensions
    {
        public static IServiceCollection AddTokenStore(this IServiceCollection services)
        {
            services.AddScoped<ITokenStore, TokenStore>();
            return services;
        }
    }
}