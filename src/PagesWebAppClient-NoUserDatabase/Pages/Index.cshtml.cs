using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PagesWebAppClient.Pages
{
    public class IndexModel : PageModel
    {
        public List<Claim> Claims { get; set; }
        public void OnGet()
        {
            Claims = Request.HttpContext.User.Claims.ToList();
        }
    }
}
