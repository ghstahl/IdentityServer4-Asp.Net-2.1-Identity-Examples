using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace ScopedHelpers
{
    public class TokenStore : ITokenStore
    {
        private const string Purpose = "TokenStore.ProtectedStore";
        private const string CookieName = "_tokens_Protected";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProtectedCookieStore _protectedCookieStore;

        public TokenStore(
            IHttpContextAccessor httpContextAccessor,
            IProtectedCookieStore protectedCookieStore)
        {
            _httpContextAccessor = httpContextAccessor;
            _protectedCookieStore = protectedCookieStore;
        }


        private async Task<Dictionary<string, string>> HarvestOidcDataAsync()
        {
            var at = await _httpContextAccessor.HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "access_token");
            var idt = await _httpContextAccessor.HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "id_token");
            var rt = await _httpContextAccessor.HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "refresh_token");
            var tt = await _httpContextAccessor.HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "token_type");
            var ea = await _httpContextAccessor.HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "expires_at");

            var oidc = new Dictionary<string, string>
            {
                {"access_token", at},
                {"id_token", idt},
                {"refresh_token", rt},
                {"token_type", tt},
                {"expires_at", ea}
            };
            return oidc;
        }

        public void HarvestAndStore()
        {
            var tokens = HarvestOidcDataAsync().GetAwaiter().GetResult();
            var json = JsonConvert.SerializeObject(tokens);
            _protectedCookieStore.Store(CookieName, json, 30);
        }

        void InternalRead()
        {
            if (_tokens == null)
            {
                string json;
                if (_protectedCookieStore.TryRead(CookieName, out json))
                {
                    _tokens = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                }
            }
        }

        private Dictionary<string, string> _tokens;
        public Dictionary<string, string> Tokens
        {
            get
            {
                InternalRead();
                return _tokens;
            }
        }
    }
}