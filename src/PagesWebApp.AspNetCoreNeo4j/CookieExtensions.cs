using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PagesWebApp
{
    public static class CookieExtensions
    {
        /// <summary>  
        /// set the cookie  
        /// </summary>  
        /// <param name="response">HttpResponse</param>
        /// <param name="key">key (unique indentifier)</param>  
        /// <param name="value">value to store in cookie object</param>  
        /// <param name="expireTime">expiration time</param>  
        public static void SetCookie(this HttpResponse response,string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
            {
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            }
            else
            {
                option.Expires = DateTime.Now.AddMilliseconds(10);
            }
            response.Cookies.Append(key, value, option);
        }

        /// <summary>  
        /// Delete the key  
        /// </summary>  
        /// <param name="response">HttpResponse</param>
        /// <param name="key">Key</param>  
        public static void RemoveCookie(this HttpResponse response, string key)
        {
            response.Cookies.Delete(key);
            CookieOptions option = new CookieOptions {Expires = DateTime.Now.AddYears(-1)};
            response.Cookies.Append(key, "", option);
        }
    }
}
