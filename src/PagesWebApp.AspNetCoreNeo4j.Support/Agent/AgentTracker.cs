using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using IdentityModel;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace PagesWebApp.Agent
{
    public class ChallengeQuestionsTracker : IChallengeQuestionsTracker
    {
        private const string Purpose = "ChallengeQuestions.ProtectedStore";
        private const string CookieName = "_cq_protected";
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IDataProtector _protector;
      
        public ChallengeQuestionsTracker(IDataProtectionProvider provider, IHttpContextAccessor httpContextAccessor)
        {
            _protector = provider.CreateProtector(Purpose);
            _httpContextAccessor = httpContextAccessor;
        }

        private Dictionary<string, bool> _challengeQuestions;
        public Dictionary<string, bool> ChallengeQuestions => _challengeQuestions ?? (_challengeQuestions = new Dictionary<string, bool>());

        public void Store()
        {
            var json = JsonConvert.SerializeObject(ChallengeQuestions);
            var bytes = Encoding.UTF8.GetBytes(json);
            bytes = _protector.Protect(bytes);
            var value = Base64Url.Encode(bytes);
            _httpContextAccessor.HttpContext.Response.SetCookie(CookieName, value, 1);
        }

        public void Retrieve()
        {
            string value;
            _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(CookieName, out value);
            if (!string.IsNullOrEmpty(value))
            {
                var bytes = Base64Url.Decode(value);
                bytes = _protector.Unprotect(bytes);
                var json = Encoding.UTF8.GetString(bytes);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, bool>>(json);
                _challengeQuestions = dict;
            }
        }

        public void Remove()
        {
            _httpContextAccessor.HttpContext.Response.RemoveCookie(CookieName);
        }
    }
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