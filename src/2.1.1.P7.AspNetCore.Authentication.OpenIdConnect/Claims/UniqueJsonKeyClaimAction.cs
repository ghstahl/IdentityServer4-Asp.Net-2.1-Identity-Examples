﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Newtonsoft.Json.Linq;

namespace P7.AspNetCore.Authentication.OpenIdConnect.Claims
{
    /// <summary>
    /// A ClaimAction that selects a top level value from the json user data with the given key name and adds it as a Claim.
    /// This no-ops if the ClaimsIdentity already contains a Claim with the given ClaimType.
    /// This no-ops if the key is not found or the value is empty.
    /// </summary>
    public class UniqueJsonKeyClaimAction : JsonKeyClaimAction
    {
        /// <summary>
        /// Creates a new UniqueJsonKeyClaimAction.
        /// </summary>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="valueType">The value to use for Claim.ValueType when creating a Claim.</param>
        /// <param name="jsonKey">The top level key to look for in the json user data.</param>
        public UniqueJsonKeyClaimAction(string claimType, string valueType, string jsonKey)
            : base(claimType, valueType, jsonKey)
        {
        }

        /// <inheritdoc />
        public override void Run(JObject userData, ClaimsIdentity identity, string issuer)
        {
            var value = userData?.Value<string>(JsonKey);
            if (string.IsNullOrEmpty(value))
            {
                // Not found
                return;
            }

            var claim = identity.FindFirst(c => string.Equals(c.Type, JsonKey, System.StringComparison.OrdinalIgnoreCase));
            if (claim != null && string.Equals(claim.Value, value, System.StringComparison.Ordinal))
            {
                // Duplicate
                return;
            }

            claim = identity.FindFirst(c =>
            {
                // If this claimType is mapped by the JwtSeurityTokenHandler, then this property will be set
                return c.Properties.TryGetValue(JwtSecurityTokenHandler.ShortClaimTypeProperty, out var shortType)
                    && string.Equals(shortType, JsonKey, System.StringComparison.OrdinalIgnoreCase);
            });
            if (claim != null && string.Equals(claim.Value, value, System.StringComparison.Ordinal))
            {
                // Duplicate with an alternate name.
                return;
            }

            identity.AddClaim(new Claim(ClaimType, value, ValueType, issuer));
        }
    }
}
