using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace PagesWebAppClient.InMemory
{
    public class AppClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser, TRole>
        where TUser : class
        where TRole : class
    {


        public AppClaimsPrincipalFactory(
            UserManager<TUser> userManager
            , RoleManager<TRole> roleManager
            , IOptions<IdentityOptions> optionsAccessor
        )
            : base(userManager, roleManager, optionsAccessor)
        {

        }

        public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            var principal = await base.CreateAsync(user);
            /*
             * get more claims.
             * */
            /*
            var claims = await _postAuthClaimsProvider.FetchClaims(principal);
            if (claims != null)
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(claims);
            }
            */
            return principal;
        }
    }
}