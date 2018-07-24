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

namespace PagesWebAppClient.Pages
{
    public class IndexModel : PageModel
    {
        private IAuthenticationService _authenticationService;
        private IOptions<List<OAuth2SchemeRecord>> _oAuth2SchemeRecords;
        private TokenClientAccessor _tokenClientAccessor;

        public IndexModel(
            IOptions<List<OAuth2SchemeRecord>> oAuth2SchemeRecords,
            TokenClientAccessor tokenClientAccessor,
            IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _tokenClientAccessor = tokenClientAccessor;
            _oAuth2SchemeRecords = oAuth2SchemeRecords;
        }
        public List<Claim> Claims { get; set; }
        public async void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {

                var kk = await _authenticationService.GetTokenAsync(HttpContext, "access_token");
                  kk = await _authenticationService.GetTokenAsync(HttpContext, "refresh_token");
                string accessToken = await HttpContext.GetTokenAsync("access_token");
          


                string idToken = await HttpContext.GetTokenAsync("id_token");

                Claims = Request.HttpContext.User.Claims.ToList();
            }

        }
    }
}
