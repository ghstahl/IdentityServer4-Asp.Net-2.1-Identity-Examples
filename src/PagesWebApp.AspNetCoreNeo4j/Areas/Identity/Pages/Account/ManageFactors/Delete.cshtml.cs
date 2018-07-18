using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace PagesWebApp.Areas.Identity.Pages.Account.ManageFactors
{
    public class DeleteModel : PageModel
    {
        private IMultiFactorUserStore<ApplicationUser, ApplicationFactor> _multiFactorUserStore;
        private UserManager<ApplicationUser> _userManager;
        private ILogger<LoginModel> _logger;
        [TempData] public string StatusMessage { get; set; }
        [BindProperty] public string FactorId { get; set; }

        public ApplicationFactor Factor { get; set; }

        public DeleteModel(
            UserManager<ApplicationUser> userManager,
            IMultiFactorUserStore<ApplicationUser, ApplicationFactor> multiFactorUserStore,
            ILogger<LoginModel> logger)
        {
            _multiFactorUserStore = multiFactorUserStore;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task OnGetAsync(string factorId)
        {
            FactorId = factorId;
            Factor = await _multiFactorUserStore.FindByIdAsync(factorId, CancellationToken.None);
        }

        public async Task<IActionResult> OnPostAsync(string button)
        {
            if (button == "delete")
            {
                var factor = await _multiFactorUserStore.FindByIdAsync(FactorId, CancellationToken.None);
                var deleteResult = await _multiFactorUserStore.DeleteAsync(factor, CancellationToken.None);
            }

            return RedirectToPage("./Index");
        }
    }
}