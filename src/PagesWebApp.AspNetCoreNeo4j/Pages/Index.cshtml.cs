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
        private ITokenStore _tokenStore;

        public IndexModel(ITokenStore tokenStore)
        {
            _tokenStore = tokenStore;
        }
        public List<Claim> Claims { get; set; }
        public Dictionary<string,string> Tokens { get; set; }
        public async Task OnGet()
        {
            if (Request.HttpContext.User != null && User.IsAuthenticated())
            {
                Tokens = _tokenStore.Tokens;
                Claims = Request.HttpContext.User.Claims.ToList();
            }
        }
       
    }
}
