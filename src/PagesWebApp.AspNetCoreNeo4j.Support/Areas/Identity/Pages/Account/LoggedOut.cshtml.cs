using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PagesWebApp.Areas.Identity.Pages.Account
{
    public class LoggedOutModel : PageModel
    {
        public LoggedOutViewModel LoggedOutViewModel { get; private set; }

        public async void OnGetAsync(string logoutId)
        {
            try
            {
                LoggedOutViewModel = JsonConvert.DeserializeObject<LoggedOutViewModel>(logoutId);
            }
            catch (Exception e)
            {
                // eatit
            }
        }
    }
}