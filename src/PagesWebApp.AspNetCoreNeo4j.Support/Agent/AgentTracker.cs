using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace PagesWebApp.Agent
{
    public class AgentTracker : IAgentTracker
    {
        private IHttpContextAccessor _httpContextAccessor;

        public bool IsLoggedIn
        {
            get
            {
                if (JwtSecurityToken == null)
                    return false;
                return true;
            }
        }

        public string UserName
        {
            get
            {
                if (IsLoggedIn)
                {
                    return Claims["name"];

                }

                return "";
            }
        }

        public string UserId
        {
            get
            {
                if (IsLoggedIn)
                {
                    return Claims["sub"];

                }
                return "";
            }
        }


        private Dictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                if (JwtSecurityToken != null)
                {
                    foreach (var h in JwtSecurityToken.Header)
                    {
                        headers.Add(h.Key, h.Value as string);
                    }
                }
                return headers;
            }
        }

        private Dictionary<string, string> Claims
        {
            get
            {
                var claims = new Dictionary<string, string>();
                if (JwtSecurityToken != null)
                {
                    foreach (var h in JwtSecurityToken.Claims)
                    {
                        claims.Add(h.Type, h.Value as string);
                    }

                }
                return claims;
            }
        }

        public void StoreIdToken(string idToken)
        {
            _httpContextAccessor.HttpContext.Response.SetCookie("_agentIdToken", idToken, 10);
        }

        public void RemoveIdToken()
        {
            _httpContextAccessor.HttpContext.Response.RemoveCookie("_agentIdToken");
        }

        public AgentTracker(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }



        private JwtSecurityToken JwtSecurityToken
        {
            get
            {

                string jwtInput = _httpContextAccessor.HttpContext.Request.Cookies["_agentIdToken"];
                if (string.IsNullOrEmpty(jwtInput))
                {
                    return null;
                }

                var jwtHandler = new JwtSecurityTokenHandler();
                var readableToken = jwtHandler.CanReadToken(jwtInput);

                if (readableToken != true)
                {
                    return null;
                }

                var jwtSecurityToken = jwtHandler.ReadJwtToken(jwtInput);
                return jwtSecurityToken;
            }
        }
    }
}