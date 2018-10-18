// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Authentication;
using P7.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OpenIdConnectExtensions
    {
        public static AuthenticationBuilder P7AddOpenIdConnect(this AuthenticationBuilder builder)
            => builder.P7AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder P7AddOpenIdConnect(this AuthenticationBuilder builder, Action<OpenIdConnectOptions> configureOptions)
            => builder.P7AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder P7AddOpenIdConnect(this AuthenticationBuilder builder, string authenticationScheme, Action<OpenIdConnectOptions> configureOptions)
            => builder.P7AddOpenIdConnect(authenticationScheme, OpenIdConnectDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder P7AddOpenIdConnect(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<OpenIdConnectOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<OpenIdConnectOptions>, OpenIdConnectPostConfigureOptions>());
            return builder.AddRemoteScheme<OpenIdConnectOptions, OpenIdConnectHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
