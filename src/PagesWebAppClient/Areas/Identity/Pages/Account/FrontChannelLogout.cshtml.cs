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
        private ConfiguredDiscoverCacheContainerFactory _configuredDiscoverCacheContainerFactory;
        private readonly ILogger _logger;
        public string EndSessionUrl { get; set; }
        public FrontChannelLogoutModel(
            SignInManager<IdentityUser> signInManager,
            ConfiguredDiscoverCacheContainerFactory configuredDiscoverCacheContainerFactory,
            ILogger<FrontChannelLogoutModel> logger)
        {
            _signInManager = signInManager;
            _configuredDiscoverCacheContainerFactory = configuredDiscoverCacheContainerFactory;
            _logger = logger;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            // Get id_token first to seed the iframe logout
            var user = await _signInManager.UserManager.GetUserAsync(User);
            if (user != null)
            {
                var openIdConnectSessionDetails = HttpContext.Session.Get<OpenIdConnectSessionDetails>(Wellknown.OIDCSessionKey);
                // no matter what, we are logging out our own app.
                // Do Not trust the provider to keep its end of the bargain to frontchannel sign us out.
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out.");

                HttpContext.Session.Clear();
        
                if (openIdConnectSessionDetails != null)
                {
                    // we have an external OIDC provider here.
                    var clientSignoutCallback = $"{Request.Scheme}://{Request.Host}/Identity/Account/SignoutCallbackOidc";
                    var discoverCacheContainer = _configuredDiscoverCacheContainerFactory.Get(openIdConnectSessionDetails.LoginProider);
                    var discoveryCache = await discoverCacheContainer.DiscoveryCache.GetAsync();
                    var endSession = discoveryCache.EndSessionEndpoint;
                    EndSessionUrl = $"{endSession}?id_token_hint={openIdConnectSessionDetails.OIDC["id_token"]}&post_logout_redirect_uri={clientSignoutCallback}";
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