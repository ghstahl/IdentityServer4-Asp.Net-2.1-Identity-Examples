using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace ScopedHelpers
{
    public class AuthenticatedInformation : IAuthenticatedInformation
    {
        private IHttpContextAccessor _httpContextAccessor;

        public AuthenticatedInformation(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        private AuthenticateResult _authenticateResult;

        public async Task<AuthenticateResult> GetAuthenticateResultAsync()
        {
            return _authenticateResult ??
                   (_authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync());
        }
    }
}