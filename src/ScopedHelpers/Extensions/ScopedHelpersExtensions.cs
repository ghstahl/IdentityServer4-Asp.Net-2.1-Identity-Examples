using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace ScopedHelpers.Extensions
{
    public static class ScopedHelpersExtensions
    {
        public static IServiceCollection AddScopedHelpers(this IServiceCollection services)
        {
            services.AddScoped<IScopedOperation, ScopedOperation>();
            return services;
        }

        public static IIdentityServerBuilder AddScopedHelpers(this IIdentityServerBuilder builder)
        {
            builder.Services.AddScoped<IScopedOperation, ScopedOperation>();
            return builder;
        }
    }
}
