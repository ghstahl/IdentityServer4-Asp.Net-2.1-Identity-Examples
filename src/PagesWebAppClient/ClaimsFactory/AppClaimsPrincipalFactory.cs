using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PagesWebAppClient.Constants;
using PagesWebAppClient.Extensions;
using PagesWebAppClient.Models;

namespace PagesWebAppClient.ClaimsFactory
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
            OpenIdConnectSessionDetails oidc = items[Wellknown.OIDCSessionKey] as OpenIdConnectSessionDetails;
            bool addIdToken = false;
            if (oidc == null)
            {
                // maybe its in a cookie
                oidc = _httpContextAccessor.HttpContext.Request.Cookies.Get<OpenIdConnectSessionDetails>(
                    Wellknown.OIDCSessionKey);
                _httpContextAccessor.HttpContext.Response.Cookies.Remove(Wellknown.OIDCSessionKey);
                // This is a special case.  as this user just got created and the call to await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                // isn't helping us to store away the tokens.
                addIdToken = true;
            }

            if (oidc != null)
            {
                ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("login_provider", oidc.LoginProider));
                if (addIdToken)
                {
                    ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("id_token", oidc.OIDC["id_token"]));
                }
            }
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
