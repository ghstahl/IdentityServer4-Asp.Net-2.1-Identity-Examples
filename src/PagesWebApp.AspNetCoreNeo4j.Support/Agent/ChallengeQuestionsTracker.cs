using System.Collections.Generic;
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
}