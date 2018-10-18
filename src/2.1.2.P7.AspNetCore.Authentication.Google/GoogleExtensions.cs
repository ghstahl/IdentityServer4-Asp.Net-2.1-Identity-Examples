// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Authentication;
using P7.AspNetCore.Authentication.Google;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GoogleExtensions
    {
        public static AuthenticationBuilder P7AddGoogle(this AuthenticationBuilder builder)
            => builder.P7AddGoogle(GoogleDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder P7AddGoogle(this AuthenticationBuilder builder, Action<GoogleOptions> configureOptions)
            => builder.P7AddGoogle(GoogleDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder P7AddGoogle(this AuthenticationBuilder builder, string authenticationScheme, Action<GoogleOptions> configureOptions)
            => builder.P7AddGoogle(authenticationScheme, GoogleDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder P7AddGoogle(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<GoogleOptions> configureOptions)
            => builder.AddOAuth<GoogleOptions, GoogleHandler>(authenticationScheme, displayName, configureOptions);
    }
}
