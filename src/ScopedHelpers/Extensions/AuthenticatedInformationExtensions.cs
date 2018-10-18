using Microsoft.Extensions.DependencyInjection;

namespace ScopedHelpers.Extensions
{
    public static class AuthenticatedInformationExtensions
    {
        
        public static IServiceCollection AddAuthenticatedInformation(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticatedInformation, AuthenticatedInformation>();
            return services;
        }
    }
}