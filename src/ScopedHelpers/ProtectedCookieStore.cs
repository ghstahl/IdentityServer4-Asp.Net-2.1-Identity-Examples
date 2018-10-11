using System.Text;
using IdentityModel;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using ScopedHelpers.Extensions;

namespace ScopedHelpers
{
    public class ProtectedCookieStore : IProtectedCookieStore
    {
        private const string Purpose = "ProtectedCookieStore.Generic";
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IDataProtector _protector;

        public ProtectedCookieStore(
            IDataProtectionProvider provider, 
            IHttpContextAccessor httpContextAccessor)
        {
            _protector = provider.CreateProtector(Purpose);
            _httpContextAccessor = httpContextAccessor;
        }

        public void Store(string cookieName, string data, int minutes)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            bytes = _protector.Protect(bytes);
            var value = Base64Url.Encode(bytes);
            _httpContextAccessor.HttpContext.Response.SetCookie(cookieName, value, minutes);
        }

        public bool TryRead(string cookieName,out string value)
        {
            string storedValue;
            _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(cookieName, out storedValue);
            if (!string.IsNullOrEmpty(storedValue))
            {
                var bytes = Base64Url.Decode(storedValue);
                bytes = _protector.Unprotect(bytes);
                value = Encoding.UTF8.GetString(bytes);
                return true;
            }
            value = null;
            return false;
        }

        public void Remove(string cookieName)
        {
            _httpContextAccessor.HttpContext.Response.RemoveCookie(cookieName);
        }
    }
}