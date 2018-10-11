using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PagesWebApp.Areas.Identity.Pages.Account;


namespace PagesWebApp.ClaimsFactory
{
    public class AppClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser, TRole>
        where TUser : class
        where TRole : class
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IExternalLoginProvider _externalLoginProvider;

        public AppClaimsPrincipalFactory(
            IHttpContextAccessor httpContextAccessor,
            IExternalLoginProvider externalLoginProvider,
            UserManager<TUser> userManager,
            RoleManager<TRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor
        )
            : base(userManager, roleManager, optionsAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _externalLoginProvider = externalLoginProvider;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            var principal = await base.CreateAsync(user);
            if (_externalLoginProvider.ExternalLoginInfo != null)
            {
                if (_externalLoginProvider.ExternalLoginInfo.LoginProvider == "EndUserKBAIDP")
                {
                    var query = from item in _externalLoginProvider.ExternalLoginInfo.Principal.Claims
                        where item.Type.StartsWith("agent:")
                        select item;
                    ((ClaimsIdentity)principal.Identity).AddClaims(query);
                }
            }
            /*
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
*/
            return principal;
        }
    }
}
