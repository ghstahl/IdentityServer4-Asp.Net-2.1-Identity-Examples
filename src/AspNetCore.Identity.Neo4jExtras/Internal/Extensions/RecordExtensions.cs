using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Identity.Neo4j.Internal.Extensions
{
    internal static class RecordMapperExtensions
    {
        public static T MapTo<T>(this IRecord record, string alias)
        {
            return JObject.FromObject(record[alias]).ToObject<T>();
        }

        /// <summary>
        /// This method allow to convert object to map for Neo4j
        /// User can remove property from map by adding [JsonIgnore] attribute on the propery
        /// 
        /// Note: Neo4j driver has its own logic to convert object to map
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ConvertToMap<T>(this T source) where T : class
        {
            return JObject.FromObject(source).ToObject<Dictionary<string, object>>();
        }
    }
}
