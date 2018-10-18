using System;
using System.Linq;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using IdentityServer4Extras.Configuration.DependencyInjection;
using IdentityServer4Extras.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4Extras.Extensions
{
    public static class IdentityServerBuilderExtensionsCoreExtras
    {
        public static void AddTransientDecorator<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddDecorator<TService>();
            services.AddTransient<TService, TImplementation>();
        }
        internal static void AddDecorator<TService>(this IServiceCollection services)
        {
            var registration = services.LastOrDefault(x => x.ServiceType == typeof(TService));
            if (registration == null)
            {
                throw new InvalidOperationException("Service type: " + typeof(TService).Name + " not registered.");
            }
            if (services.Any(x => x.ServiceType == typeof(Decorator<TService>)))
            {
                throw new InvalidOperationException("Decorator already registered for type: " + typeof(TService).Name + ".");
            }

            services.Remove(registration);

            if (registration.ImplementationInstance != null)
            {
                var type = registration.ImplementationInstance.GetType();
                var innerType = typeof(Decorator<,>).MakeGenericType(typeof(TService), type);
                services.Add(new ServiceDescriptor(typeof(Decorator<TService>), innerType, ServiceLifetime.Transient));
                services.Add(new ServiceDescriptor(type, registration.ImplementationInstance));
            }
            else if (registration.ImplementationFactory != null)
            {
                services.Add(new ServiceDescriptor(typeof(Decorator<TService>), provider =>
                {
                    return new DisposableDecorator<TService>((TService)registration.ImplementationFactory(provider));
                }, registration.Lifetime));
            }
            else
            {
                var type = registration.ImplementationType;
                var innerType = typeof(Decorator<,>).MakeGenericType(typeof(TService), registration.ImplementationType);
                services.Add(new ServiceDescriptor(typeof(Decorator<TService>), innerType, ServiceLifetime.Transient));
                services.Add(new ServiceDescriptor(type, type, registration.Lifetime));
            }
        }

        /// <summary>
        /// Adds a redirect URI validator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder SwapOutRedirectUriValidator<T>(this IIdentityServerBuilder builder)
            where T : class, IRedirectUriValidator
        {
            builder.Services.Remove<IRedirectUriValidator>();
            builder.Services.AddTransient<IRedirectUriValidator, T>();

            return builder;
        }

        public static IIdentityServerBuilder SwapOutAspNetIdentityProfileService<TUser>(this IIdentityServerBuilder builder)
            where TUser : class
        {
            builder.Services.Remove<IProfileService>();
            builder.Services.AddTransient<IdentityServer4.AspNetIdentity.ProfileService<TUser>>();
            builder.Services.AddTransient<IProfileServicePlugin, EndUserKBAProfileService>();
            builder.AddProfileService<IdentityServer4Extras.Services.ProfileServiceAggregator<TUser>>();
            return builder;
        }
    }
}