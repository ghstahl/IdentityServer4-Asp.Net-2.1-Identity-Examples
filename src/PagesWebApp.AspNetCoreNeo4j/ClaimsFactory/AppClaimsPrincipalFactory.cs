using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;


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
            string json;
            _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("_tempfilteredClaims",out json);
            if (!string.IsNullOrEmpty(json))
            {
                var filteredClaims = JsonConvert.DeserializeObject<List<ClaimHandle>>(json);
                foreach (var item in filteredClaims)
                {
                    ((ClaimsIdentity)principal.Identity).AddClaim(new Claim(item.Type,item.Value));
                }
            }

            return principal;
        }
    }
}
