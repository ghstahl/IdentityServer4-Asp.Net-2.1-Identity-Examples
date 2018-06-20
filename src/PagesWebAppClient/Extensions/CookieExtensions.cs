using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace PagesWebAppClient.Extensions
{
    public static class CookieExtensions
    {
        public static void Set<T>(this IResponseCookies response, string key, T value, int? expireTime)
        {
            CookieOptions option = new CookieOptions
            {
                Expires = expireTime.HasValue
                    ? DateTime.Now.AddMinutes(expireTime.Value)
                    : DateTime.Now.AddMilliseconds(10)
            };

            response.Append(key, JsonConvert.SerializeObject(value), option);
        }
        public static T Get<T>(this IRequestCookieCollection request, string key) where T:class
        {
            T result = null;
            if (request.TryGetValue(key, out var obj))
            {
                result = JsonConvert.DeserializeObject<T>(obj);
            }
            return result;
        }
        public static void Remove(this IResponseCookies response, string key)
        {
            var option = new CookieOptions {Expires = DateTime.Now.AddHours(-24)};
            response.Append(key, "", option);
        }
    }
}