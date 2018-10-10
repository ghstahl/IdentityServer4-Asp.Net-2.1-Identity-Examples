using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PagesWebApp.Agent;
using ScopedHelpers;

namespace PagesWebApp.ClaimsFactory
{
    public class AppClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser, TRole>
        where TUser : class
        where TRole : class
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IScopedOperation _scopedOperation;
        private IAgentTracker _agentTracker;

        public AppClaimsPrincipalFactory(
            IScopedOperation scopedOperation,
            IAgentTracker agentTracker,
            IHttpContextAccessor httpContextAccessor,
            UserManager<TUser> userManager,
            RoleManager<TRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor
        )
            : base(userManager, roleManager, optionsAccessor)
        {
            _scopedOperation = scopedOperation;
            _agentTracker = agentTracker;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            if (!_scopedOperation.Dictionary.ContainsKey("agent:username"))
            {
                _scopedOperation.Dictionary.Add("agent:username", _agentTracker.UserName);
            }
            var principal = await base.CreateAsync(user);
            var items = _httpContextAccessor.HttpContext.Items;
            ((ClaimsIdentity) principal.Identity).AddClaim(new Claim("role", "SupportAgent"));
            ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("agent:username", _agentTracker.UserName));
            return principal;
        }
    }
}
