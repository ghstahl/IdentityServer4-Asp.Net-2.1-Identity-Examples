using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PagesWebApp.Agent;

namespace PagesWebApp.Areas.Identity.Pages.Account
{
    public static class LoginWellKnown
    {
        public static string LoginReturnUrlCookieName = "_login_returnUrl";
    }
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
      
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private IIdentityServerInteractionService _interaction;
        private IAgentTracker _agentTracker;

        public LoginModel(
            IAgentTracker agentTracker,
            IIdentityServerInteractionService interaction, 
            SignInManager<ApplicationUser> signInManager, 
            ILogger<LoginModel> logger)
        {
            _agentTracker = agentTracker;
            _interaction = interaction;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

      
        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? "/Identity/Account/Login";

   
            Response.SetCookie(LoginWellKnown.LoginReturnUrlCookieName, returnUrl, 360);
            if (!_agentTracker.IsLoggedIn)
            {
                var dd = $"/Identity/Account/Login?returnUrl={WebUtility.UrlEncode(returnUrl)}";
                return Redirect($"/Identity/Account/ExternalAgentLogins?returnUrl={WebUtility.UrlEncode(dd)}");
            }

          

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context != null)
            {
                var idpProvider = (from item in context.AcrValues
                    where item.StartsWith("idp=")
                    select item.Substring(4)).FirstOrDefault();
                if (!string.IsNullOrEmpty(idpProvider))
                {

                    var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
                    var properties = _signInManager.ConfigureExternalAuthenticationProperties(
                        idpProvider, redirectUrl);
                    return new ChallengeResult(idpProvider, properties);

                }
            }

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string button, string returnUrl = null)
        {
           
            var returnUrlCookie = Request.Cookies[LoginWellKnown.LoginReturnUrlCookieName];
            returnUrl = returnUrlCookie;

            if (button == "stepOne")
            {
                return RedirectToPage("./LoginStepOne", routeValues: new
                {
                    returnUrl = returnUrl
                });
            }
            if (button != "login")
            {
                Response.RemoveCookie(LoginWellKnown.LoginReturnUrlCookieName);
                // the user clicked the "cancel" button
                var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.GrantConsentAsync(context, ConsentResponse.Denied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                 
                    return Redirect(returnUrl);
                }
                else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return Redirect("~/");
                }
            }


            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    Response.RemoveCookie(LoginWellKnown.LoginReturnUrlCookieName);
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    Response.RemoveCookie(LoginWellKnown.LoginReturnUrlCookieName);
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
