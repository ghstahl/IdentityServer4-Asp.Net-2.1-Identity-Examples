using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using PagesWebAppClient.Constants;
using PagesWebAppClient.Extensions;
using PagesWebAppClient.Models;

namespace PagesWebAppClient.Controllers
{
    [Produces("application/json")]
    [Route("refresh-tokens")]
    [ApiController]
    public class RefreshController : ControllerBase
    {
        private TokenClientAccessor _tokenClientAccessor;

        public RefreshController(TokenClientAccessor tokenClientAccessor)
        {
            _tokenClientAccessor = tokenClientAccessor;
        }
        // GET: api/KeepAlive
        [HttpGet]
        public async Task<string> GetAsync()
        {
            /*
              HttpContext.Session.Set(
               Wellknown.OIDCSessionKey,
               new OpenIdConnectSessionDetails
               {
               Authority = oAuth2SchemeRecord.Authority,
               LoginProider = info.LoginProvider,
               OIDC = oidc
               });
             */
            var openIdConnectSessionDetails = HttpContext.Session.Get<OpenIdConnectSessionDetails>(Wellknown.OIDCSessionKey);
            if (openIdConnectSessionDetails != null)
            {
                var tokenClient = await _tokenClientAccessor.TokenClientByAuthorityAsync(openIdConnectSessionDetails.Authority);
                string refreshToken;
                openIdConnectSessionDetails.OIDC.TryGetValue("refresh_token", out refreshToken);

                var tokenResponse = await tokenClient.RequestRefreshTokenAsync(refreshToken);
                if (tokenResponse != null)
                {
                    var expiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn).ToString("s");
                    var oidc = new Dictionary<string, string>
                    {
                        {"access_token", tokenResponse.AccessToken},
                        {"id_token", tokenResponse.IdentityToken},
                        {"refresh_token", tokenResponse.RefreshToken},
                        {"token_type", tokenResponse.TokenType},
                        {"expires_at", expiresAt}
                    };

                    openIdConnectSessionDetails.OIDC = oidc;
                    HttpContext.Session.Set(
                        Wellknown.OIDCSessionKey,
                        openIdConnectSessionDetails);
                    return expiresAt;
                }
            }
            return $"Ok";
        }
    }
}