using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace PagesWebAppClient.Areas.Identity.Pages.Account
{
    public class SignoutFrontChannelModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger _logger;
        public SignoutFrontChannelModel(SignInManager<IdentityUser> signInManager, ILogger<SignoutFrontChannelModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out via signout-frontchannel.");
        }
    }
}