using Microsoft.Extensions.DependencyInjection;

namespace ScopedHelpers.Extensions
{
    public static class ProtectedCookieExtensions
    {
        public static IServiceCollection AddProtectedCookie(this IServiceCollection services)
        {
            services.AddScoped<IProtectedCookieStore, ProtectedCookieStore>();
            return services;
        }
    }
}