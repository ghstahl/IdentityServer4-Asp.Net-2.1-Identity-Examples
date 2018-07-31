using System.Collections.Generic;
using Neo4j.Driver.V1;
using Newtonsoft.Json.Linq;

namespace Neo4jExtras.Extensions
{
    public static class RecordMapperExtensions
    {
        public static T MapTo<T>(this IRecord record, string alias)
        {
            var aliasObj = record[alias];
            var jObj = JObject.FromObject(aliasObj);
            var obj = jObj.ToObject<T>();
            return obj;
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
