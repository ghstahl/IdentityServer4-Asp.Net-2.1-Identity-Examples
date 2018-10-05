using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace PagesWebApp.ClaimsFactory
{
    public class AppClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser, TRole>
        where TUser : class
        where TRole : class
    {
        private IHttpContextAccessor _httpContextAccessor;

        public AppClaimsPrincipalFactory(
            IHttpContextAccessor httpContextAccessor,
            UserManager<TUser> userManager,
             RoleManager<TRole> roleManager,
             IOptions<IdentityOptions> optionsAccessor
        )
            : base(userManager, roleManager, optionsAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            var principal = await base.CreateAsync(user);
            var items = _httpContextAccessor.HttpContext.Items;
           ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("role", "SupportAgent"));
            return principal;
        }
    }
}
