﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Claims;
using P7.AspNetCore.Authentication.OAuth.Claims;
using Newtonsoft.Json.Linq;

namespace P7.AspNetCore.Authentication
{
    public static class ClaimActionCollectionMapExtensions
    {
        /// <summary>
        /// Select a top level value from the json user data with the given key name and add it as a Claim.
        /// This no-ops if the key is not found or the value is empty.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="jsonKey">The top level key to look for in the json user data.</param>
        public static void MapJsonKey(this ClaimActionCollection collection, string claimType, string jsonKey)
        {
            collection.MapJsonKey(claimType, jsonKey, ClaimValueTypes.String);
        }

        /// <summary>
        /// Select a top level value from the json user data with the given key name and add it as a Claim.
        /// This no-ops if the key is not found or the value is empty.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="jsonKey">The top level key to look for in the json user data.</param>
        /// <param name="valueType">The value to use for Claim.ValueType when creating a Claim.</param>
        public static void MapJsonKey(this ClaimActionCollection collection, string claimType, string jsonKey, string valueType)
        {
            collection.Add(new JsonKeyClaimAction(claimType, valueType, jsonKey));
        }

        /// <summary>
        /// Select a second level value from the json user data with the given top level key name and second level sub key name and add it as a Claim.
        /// This no-ops if the keys are not found or the value is empty.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="jsonKey">The top level key to look for in the json user data.</param>
        /// <param name="subKey">The second level key to look for in the json user data.</param>
        public static void MapJsonSubKey(this ClaimActionCollection collection, string claimType, string jsonKey, string subKey)
        {
            collection.MapJsonSubKey(claimType, jsonKey, subKey, ClaimValueTypes.String);
        }

        /// <summary>
        /// Select a second level value from the json user data with the given top level key name and second level sub key name and add it as a Claim.
        /// This no-ops if the keys are not found or the value is empty.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="jsonKey">The top level key to look for in the json user data.</param>
        /// <param name="subKey">The second level key to look for in the json user data.</param>
        /// <param name="valueType">The value to use for Claim.ValueType when creating a Claim.</param>
        public static void MapJsonSubKey(this ClaimActionCollection collection, string claimType, string jsonKey, string subKey, string valueType)
        {
            collection.Add(new JsonSubKeyClaimAction(claimType, valueType, jsonKey, subKey));
        }

        /// <summary>
        /// Run the given resolver to select a value from the json user data to add as a claim.
        /// This no-ops if the returned value is empty.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="resolver">The Func that will be called to select value from the given json user data.</param>
        public static void MapCustomJson(this ClaimActionCollection collection, string claimType, Func<JObject, string> resolver)
        {
            collection.MapCustomJson(claimType, ClaimValueTypes.String, resolver);
        }

        /// <summary>
        /// Run the given resolver to select a value from the json user data to add as a claim.
        /// This no-ops if the returned value is empty.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="valueType">The value to use for Claim.ValueType when creating a Claim.</param>
        /// <param name="resolver">The Func that will be called to select value from the given json user data.</param>
        public static void MapCustomJson(this ClaimActionCollection collection, string claimType, string valueType, Func<JObject, string> resolver)
        {
            collection.Add(new CustomJsonClaimAction(claimType, valueType, resolver));
        }

        /// <summary>
        /// Clears any current ClaimsActions and maps all values from the json user data as claims, excluding duplicates.
        /// </summary>
        /// <param name="collection"></param>
        public static void MapAll(this ClaimActionCollection collection)
        {
            collection.Clear();
            collection.Add(new MapAllClaimsAction());
        }

        /// <summary>
        /// Clears any current ClaimsActions and maps all values from the json user data as claims, excluding the specified types.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="exclusions"></param>
        public static void MapAllExcept(this ClaimActionCollection collection, params string[] exclusions)
        {
            collection.MapAll();
            collection.DeleteClaims(exclusions);
        }

        /// <summary>
        /// Delete all claims from the given ClaimsIdentity with the given ClaimType.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="claimType"></param>
        public static void DeleteClaim(this ClaimActionCollection collection, string claimType)
        {
            collection.Add(new DeleteClaimAction(claimType));
        }

        /// <summary>
        /// Delete all claims from the ClaimsIdentity with the given claimTypes.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="claimTypes"></param>
        public static void DeleteClaims(this ClaimActionCollection collection, params string[] claimTypes)
        {
            if (claimTypes == null)
            {
                throw new ArgumentNullException(nameof(claimTypes));
            }

            foreach (var claimType in claimTypes)
            {
                collection.Add(new DeleteClaimAction(claimType));
            }
        }
    }
}
