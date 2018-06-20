using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PagesWebAppClient.Constants;
using PagesWebAppClient.Extensions;
using PagesWebAppClient.Models;
using PagesWebAppClient.Utils;

namespace PagesWebAppClient.Areas.Identity.Pages.Account
{
    public class FrontChannelLogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private ConfiguredDiscoverCacheContainerFactory _configuredDiscoverCacheContainerFactory;
        private readonly ILogger _logger;
        public string EndSessionUrl { get; set; }

        public FrontChannelLogoutModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ConfiguredDiscoverCacheContainerFactory configuredDiscoverCacheContainerFactory,
            ILogger<FrontChannelLogoutModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuredDiscoverCacheContainerFactory = configuredDiscoverCacheContainerFactory;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get id_token first to seed the iframe logout
            var user = await _signInManager.UserManager.GetUserAsync(User);
            if (user != null)
            {
                // this is the session alternative to storing tokens
                //var openIdConnectSessionDetails = HttpContext.Session.Get<OpenIdConnectSessionDetails>(Wellknown.OIDCSessionKey);

                var queryLoginProviderClaim = (from claim in User.Claims
                    where claim.Type == "login_provider"
                    select claim).FirstOrDefault();
                var queryIdTokenClaim = (from claim in User.Claims
                    where claim.Type == "id_token"
                    select claim).FirstOrDefault();
                string loginProvider = null;
                string idToken = null;

                if (queryLoginProviderClaim != null)
                {
                    loginProvider = queryLoginProviderClaim.Value;
                    if (queryIdTokenClaim != null)
                    {
                        idToken = queryIdTokenClaim.Value;
                    }
                    else
                    {
                        idToken = await _userManager.GetAuthenticationTokenAsync(user, loginProvider, "id_token");
                    }
                }

                // no matter what, we are logging out our own app.
                // Do Not trust the provider to keep its end of the bargain to frontchannel sign us out.
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out.");

                HttpContext.Session.Clear();

                if (!string.IsNullOrEmpty(loginProvider) && !string.IsNullOrEmpty(idToken))
                {
                    // we have an external OIDC provider here.
                    var clientSignoutCallback =
                        $"{Request.Scheme}://{Request.Host}/Identity/Account/SignoutCallbackOidc";
                    var discoverCacheContainer = _configuredDiscoverCacheContainerFactory.Get(loginProvider);
                    var discoveryCache = await discoverCacheContainer.DiscoveryCache.GetAsync();
                    var endSession = discoveryCache.EndSessionEndpoint;
                    EndSessionUrl =
                        $"{endSession}?id_token_hint={idToken}&post_logout_redirect_uri={clientSignoutCallback}";
                    // this redirect is to the provider to log everyone else out.  
                    // We will get a double hit here, as our $"{Request.Scheme}://{Request.Host}/Account/SignoutFrontChannel";
                    // will get hit as well.  
                    return new RedirectResult(EndSessionUrl);
                }
            }

            return new RedirectResult("/");
        }
    }
}