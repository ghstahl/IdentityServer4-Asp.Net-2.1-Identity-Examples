using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModelExtras;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PagesWebAppClient.Constants;
using PagesWebAppClient.Extensions;
using PagesWebAppClient.Models;

namespace PagesWebAppClient.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel()
        {
        }
        public List<Claim> Claims { get; set; }
        public OpenIdConnectSessionDetails OpenIdConnectSessionDetails { get; set; }
        public async void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {

                OpenIdConnectSessionDetails = HttpContext.Session.Get<OpenIdConnectSessionDetails>(Wellknown.OIDCSessionKey);

                Claims = Request.HttpContext.User.Claims.ToList();
            }

        }
    }
}
