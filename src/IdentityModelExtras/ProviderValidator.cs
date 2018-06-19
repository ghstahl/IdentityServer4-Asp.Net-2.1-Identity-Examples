using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace IdentityModelExtras
{
    public class ProviderValidator
    {
        private MemoryCache _certCache;
        private List<RsaSecurityKey> _cachedCerts;
        private IDiscoveryCacheContainer _discoverCacheContainer;
        private string _audience;
        public DiscoveryResponse _discoveryResponse;
        private IMemoryCache _cache;
        public ProviderValidator(IDiscoveryCacheContainer discoverCacheContainer, 
            IMemoryCache cache,string audience = null)
        {
            _discoverCacheContainer = discoverCacheContainer;
            _audience = audience;
            _cache = cache;
        }

        async Task<DiscoveryResponse> GetDiscoveryResponseAsync()
        {
            if (_discoveryResponse == null)
            {
                _discoveryResponse = await _discoverCacheContainer.DiscoveryCache.GetAsync();
            }
            return _discoveryResponse;
        }

        public async Task<List<RsaSecurityKey>> FetchCertificates()
        {
            List<RsaSecurityKey> cacheEntry;
            if (!_cache.TryGetValue("certs", out cacheEntry))
            {
                // Key not in cache, so get data.
                var doc = await GetDiscoveryResponseAsync();
                var tokenEndpoint = doc.TokenEndpoint;
                var keys = doc.KeySet.Keys;

                cacheEntry = new List<RsaSecurityKey>();
                foreach (var webKey in keys)
                {
                    var e = Base64Url.Decode(webKey.E);
                    var n = Base64Url.Decode(webKey.N);

                    var key = new RsaSecurityKey(new RSAParameters {Exponent = e, Modulus = n})
                    {
                        KeyId = webKey.Kid
                    };

                    cacheEntry.Add(key);
                }
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(_discoverCacheContainer.DiscoveryCache.CacheDuration);
                // Save data in cache.
                _cache.Set("certs", cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }

        public async Task<ClaimsPrincipal> ValidateToken(string idToken, TokenValidationParameters tvp)
        {
            var doc = await GetDiscoveryResponseAsync();
            var certificates = await this.FetchCertificates();
            tvp.IssuerSigningKeys = certificates;
            tvp.ValidIssuers = new List<string> {doc.Issuer};
            if (!string.IsNullOrEmpty(_audience))
            {
                tvp.ValidateAudience = true;
                tvp.ValidAudience = _audience;
            }

            JwtSecurityTokenHandler jsth = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            ClaimsPrincipal cp = jsth.ValidateToken(idToken, tvp, out validatedToken);

            return cp;
        }

        public async Task<ClaimsPrincipal> ValidateToken(string idToken)
        {
            TokenValidationParameters tvp = new TokenValidationParameters()
            {
                ValidateActor = false, // check the profile ID

                ValidateAudience = false, // check the client ID
                ValidAudience = null,

                ValidateIssuer = true, // check token came from ?
                ValidateIssuerSigningKey = true,
                RequireSignedTokens = true,

                ValidateLifetime = true,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.FromHours(13)
            };
            return await ValidateToken(idToken, tvp);
        }
    }
}