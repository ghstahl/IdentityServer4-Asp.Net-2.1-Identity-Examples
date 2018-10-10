using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PagesWebApp.Agent;

namespace PagesWebApp.Areas.Identity.Pages.Account
{
    public class LoginStepTwoModel : PageModel
    {
        private SignInManager<ApplicationUser> _signInManager;
        private IMultiFactorUserStore<ApplicationUser, ApplicationFactor> _multiFactorUserStore;
        private ILogger<LoginModel> _logger;
        private UserManager<ApplicationUser> _userManager;
        private IIdentityServerInteractionService _interaction;
        private IChallengeQuestionsTracker _challengeQuestionsTracker;

        public class InputMultiFactorStepTwoModel
        {
            public class ApplicationFactorHandle
            {
                [Required] public string Challenge { get; set; }
                [Required] public string ChallengeResponse { get; set; }
            }
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public List<ApplicationFactorHandle> Factors { get; set; }
        }
        [BindProperty]
        public InputMultiFactorStepTwoModel Input { get; set; }

        public LoginStepTwoModel(
            IIdentityServerInteractionService interaction, 
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IMultiFactorUserStore<ApplicationUser, ApplicationFactor> multiFactorUserStore,
            IChallengeQuestionsTracker challengeQuestionsTracker,
            ILogger<LoginModel> logger)
        {
            _interaction = interaction;
            _signInManager = signInManager;
            _userManager = userManager;
            _multiFactorUserStore = multiFactorUserStore;
            _challengeQuestionsTracker = challengeQuestionsTracker;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string email, string returnUrl)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var factors = await _multiFactorUserStore.GetFactorsAsync(user, CancellationToken.None);
            ReturnUrl = returnUrl;
            var inputFactors = (from factor in factors
                let c = new InputMultiFactorStepTwoModel.ApplicationFactorHandle
                {
                    Challenge = factor.Challenge
                }
                select c).ToList();

            Input = new InputMultiFactorStepTwoModel()
            {
                Email = email,
                Factors = inputFactors
            };

            return Page();
        }

        [BindProperty]
        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnPostAsync(string button)
        {
            var returnUrlCookie = Request.Cookies[LoginWellKnown.LoginReturnUrlCookieName];
            var returnUrl = returnUrlCookie;

            if (button != "submit")
            {
                // the user clicked the "cancel" button
                var context = await _interaction.GetAuthorizationContextAsync(ReturnUrl);
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.GrantConsentAsync(context, ConsentResponse.Denied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    return Redirect(ReturnUrl);
                }
                else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return Redirect("~/");
                }
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                var factors = await _multiFactorUserStore.GetFactorsAsync(user, CancellationToken.None);
                var factorDictionary = new Dictionary<string, ApplicationFactor>();
                foreach (var factor in factors)
                {
                    factorDictionary.Add(factor.Challenge,factor);
                }

                bool challengeResponseValid = true;
                foreach (var inputFactor in Input.Factors)
                {
                    var factor = factorDictionary[inputFactor.Challenge];
                    challengeResponseValid = SecurePasswordHasher.Verify(inputFactor.ChallengeResponse, factor.ChallengeResponseHash);
                    
                    if (!challengeResponseValid)
                    {
                        ModelState.AddModelError(string.Empty, $"{inputFactor.Challenge}: Invalid Challenge Response.");
                    }
                }

                if (challengeResponseValid)
                {
                    // we can now signin.
                     await _signInManager.SignInAsync(user,  false, IdentityConstants.ApplicationScheme);
                    Response.RemoveCookie(LoginWellKnown.LoginReturnUrlCookieName);

                    foreach (var inputFactor in Input.Factors)
                    {
                        _challengeQuestionsTracker.ChallengeQuestions.Add(inputFactor.Challenge,true);
                    }
                    _challengeQuestionsTracker.Store();

                    return LocalRedirect(returnUrl);
                }
            }

          
            return Page();
        }
    }
}