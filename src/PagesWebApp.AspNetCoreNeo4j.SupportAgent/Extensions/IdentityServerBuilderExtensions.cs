using IdentityServer4.AspNetIdentity;
using IdentityServer4.Services;
using IdentityServer4Extras.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace PagesWebApp.Extensions
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddSupportAgentAspNetIdentity<TUser>(this IIdentityServerBuilder builder)
            where TUser : class
        {
            builder.Services.Remove<IProfileService>();
            builder.AddProfileService<PagesWebApp.SupportAgent.ProfileService<TUser>>();
            return builder;
        }
    }
}