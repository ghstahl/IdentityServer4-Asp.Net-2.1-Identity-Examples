﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using P7.AspNetCore.Authentication.OpenIdConnect.Claims;


namespace Microsoft.AspNetCore.Authentication
{
    public static class ClaimActionCollectionUniqueExtensions
    {
        /// <summary>
        /// Selects a top level value from the json user data with the given key name and adds it as a Claim.
        /// This no-ops if the ClaimsIdentity already contains a Claim with the given ClaimType.
        /// This no-ops if the key is not found or the value is empty.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="jsonKey">The top level key to look for in the json user data.</param>
        public static void P7MapUniqueJsonKey(this ClaimActionCollection collection, string claimType, string jsonKey)
        {
            collection.P7MapUniqueJsonKey(claimType, jsonKey, ClaimValueTypes.String);
        }

        /// <summary>
        /// Selects a top level value from the json user data with the given key name and adds it as a Claim.
        /// This no-ops if the ClaimsIdentity already contains a Claim with the given ClaimType.
        /// This no-ops if the key is not found or the value is empty.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="jsonKey">The top level key to look for in the json user data.</param>
        /// <param name="valueType">The value to use for Claim.ValueType when creating a Claim.</param>
        public static void P7MapUniqueJsonKey(this ClaimActionCollection collection, string claimType, string jsonKey, string valueType)
        {
            collection.Add(new UniqueJsonKeyClaimAction(claimType, valueType, jsonKey));
        }
    }
}
