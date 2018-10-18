using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScopedHelpers;

namespace PagesWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private IAuthenticatedInformation _authenticatedInformation;

        public IndexModel(IAuthenticatedInformation authenticatedInformation)
        {
            _authenticatedInformation = authenticatedInformation;
        }
        public List<Claim> Claims { get; set; }
        public AuthenticateResult AuthenticateResult { get; private set; }
        public Dictionary<string,string> Tokens { get; set; }
        public async Task OnGetAsync()
        {
            if (Request.HttpContext.User != null && User.IsAuthenticated())
            {
                AuthenticateResult = await _authenticatedInformation.GetAuthenticateResultAsync();
                Tokens = new Dictionary<string, string>();
                Claims = Request.HttpContext.User.Claims.ToList();
            }
        }
       
    }
}
