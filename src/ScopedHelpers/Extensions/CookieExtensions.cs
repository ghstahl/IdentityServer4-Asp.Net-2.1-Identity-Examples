using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ScopedHelpers.Extensions
{
    public static class CookieExtensions
    {

        public static void SetCookie<T>(this IResponseCookies response, string key, T value, int? expireTime)
        {
            CookieOptions option = new CookieOptions
            {
                Expires = expireTime.HasValue
                    ? DateTime.Now.AddMinutes(expireTime.Value)
                    : DateTime.Now.AddMilliseconds(10)
            };
            var json = JsonConvert.SerializeObject(value);
            response.Append(key, json, option);
        }
        public static T Get<T>(this IRequestCookieCollection request, string key) where T : class
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
            var option = new CookieOptions { Expires = DateTime.Now.AddHours(-24) };
            response.Append(key, "", option);
        }

        /// <summary>  
        /// set the cookie  
        /// </summary>  
        /// <param name="response">HttpResponse</param>
        /// <param name="key">key (unique indentifier)</param>  
        /// <param name="value">value to store in cookie object</param>  
        /// <param name="expireTime">expiration time</param>  
        public static void SetCookie(this HttpResponse response, string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions
            {
                Expires = expireTime.HasValue
                    ? DateTime.Now.AddMinutes(expireTime.Value)
                    : DateTime.Now.AddSeconds(5)
            };

            response.Cookies.Append(key, value, option);
        }
        public static void SetCookie<T>(this HttpResponse response, string key, T value, int? expireTime)
        {
            CookieOptions option = new CookieOptions
            {
                Expires = expireTime.HasValue
                    ? DateTime.Now.AddMinutes(expireTime.Value)
                    : DateTime.Now.AddMilliseconds(10)
            };
            var json = JsonConvert.SerializeObject(value);
            response.Cookies.Append(key, json, option);
        }
        /// <summary>  
        /// Delete the key  
        /// </summary>  
        /// <param name="response">HttpResponse</param>
        /// <param name="key">Key</param>  
        public static void RemoveCookie(this HttpResponse response, string key)
        {
            response.Cookies.Delete(key);
            CookieOptions option = new CookieOptions { Expires = DateTime.Now.AddYears(-1) };
            response.Cookies.Append(key, "", option);
        }
    }
}