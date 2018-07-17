using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace PagesWebApp.Areas.Identity.Pages.Account
{
    public class LoginStepOneModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private IIdentityServerInteractionService _interaction;
        private IMultiFactorUserStore<ApplicationUser, ApplicationFactor> _multiFactorUserStore;
        private UserManager<ApplicationUser> _userManager;

        public LoginStepOneModel(
            IIdentityServerInteractionService interaction,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IMultiFactorUserStore<ApplicationUser, ApplicationFactor> multiFactorUserStore,
            ILogger<LoginModel> logger)
        {
            _interaction = interaction;
            _signInManager = signInManager;
            _userManager = userManager;
            _multiFactorUserStore = multiFactorUserStore;
            _logger = logger;
        }


        [BindProperty]
        public InputMultiFactorStepOneModel InputStepOne { get; set; }


        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }


        public class InputMultiFactorStepOneModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }
        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string button, string returnUrl = null)
        {
            if (button != "submit")
            {
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
                var user = await _userManager.FindByEmailAsync(InputStepOne.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
                return RedirectToPage("./LoginStepTwo", routeValues: new
                {
                    email = InputStepOne.Email,
                    returnUrl = returnUrl

                });

            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
