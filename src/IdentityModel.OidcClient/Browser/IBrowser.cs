﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading.Tasks;

namespace IdentityModel.OidcClient.Browser
{
    /// <summary>
    /// Models a browser
    /// </summary>
    public interface IBrowser
    {
        /// <summary>
        /// Invokes the browser.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        Task<BrowserResult> InvokeAsync(BrowserOptions options);
    }
}