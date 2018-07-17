using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace PagesWebApp.Areas.Identity.Pages.Account.ManageFactors
{
    public class IndexModel : PageModel
    {
        private IMultiFactorUserStore<ApplicationUser, ApplicationFactor> _multiFactorUserStore;
        private ILogger<LoginModel> _logger;
        private UserManager<ApplicationUser> _userManager;

        public IList<ApplicationFactor> Factors { get; set; }

        public class InputModel
        {
            [Required] public string Challenge { get; set; }

            [Required]
            [Display(Name = "Challenge Response")]
            public string ChallengeResponse { get; set; }
        }

        [BindProperty] public InputModel Input { get; set; }

        [TempData] public string StatusMessage { get; set; }
        [BindProperty] public string Username { get; set; }

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            IMultiFactorUserStore<ApplicationUser, ApplicationFactor> multiFactorUserStore,
            ILogger<LoginModel> logger)
        {
            _multiFactorUserStore = multiFactorUserStore;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            Username = userName;
            Input = new InputModel();
            Factors = await _multiFactorUserStore.GetFactorsAsync(user, CancellationToken.None);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var userName = await _userManager.GetUserNameAsync(user);
            Username = userName;

            var identityResult =
                await _multiFactorUserStore.AddToFactorAsync(
                    user, 
                    new ApplicationFactor
                    {
                        FactorId = ApplicationFactor.UniqueFactorId(),
                        Challenge = Input.Challenge,
                        ChallengeResponseHash = ApplicationFactor.GenerateChallengeResponseHash(Input.ChallengeResponse)
                    }, CancellationToken.None);

            return RedirectToPage();
        }

    }
}