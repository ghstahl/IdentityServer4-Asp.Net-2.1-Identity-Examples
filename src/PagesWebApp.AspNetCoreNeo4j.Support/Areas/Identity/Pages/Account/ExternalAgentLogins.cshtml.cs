using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PagesWebApp.Agent;

namespace PagesWebApp.Areas.Identity.Pages.Account
{
    public class ExternalAgentLoginsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IAgentTracker _agentTracker;

        public ExternalAgentLoginsModel(
            IAgentTracker agentTracker,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _agentTracker = agentTracker;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationScheme> OtherLogins { get; set; }

        public bool ShowRemoveButton { get; set; }
        [BindProperty]
        public string ReturnUrl { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            Response.SetCookie("_eal_returnUrl", returnUrl, 360);

            OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .ToList();
            return Page();
        }
        public async Task<IActionResult> OnGetLogoutAsync()
        {
            _agentTracker.RemoveIdToken();
            return Redirect("/Identity/Account/Login");
        }

        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Page("./ExternalAgentLogins", pageHandler: "LinkLoginCallback");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }
 
        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            var returnUrl = Request.Cookies["_eal_returnUrl"];
            Response.RemoveCookie("_eal_returnUrl");
            var info = await _signInManager.GetExternalLoginInfoAsync();

            var jwtHandler = new JwtSecurityTokenHandler();
            var authTokens = info.AuthenticationTokens.ToList();
            var query = from item in authTokens
                where item.Name == "id_token"
                select item;
         
            var idToken = query.FirstOrDefault().Value;
            //Check if readable token (string is in a JWT format)
            var readableToken = jwtHandler.CanReadToken(idToken);
            if (readableToken)
            {
                _agentTracker.StoreIdToken(idToken);
            }
            else
            {
                _agentTracker.RemoveIdToken();
            }
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "The external login was added.";
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "/Identity/Account/Login";
            }
            return Redirect(returnUrl);
        }
    }
}
