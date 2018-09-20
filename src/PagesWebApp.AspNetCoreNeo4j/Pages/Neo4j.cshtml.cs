using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoresIdentityServer4.Neo4j;

namespace PagesWebApp.AspNetCoreNeo4j.Pages
{
    [Authorize]
    public class Neo4jModel : PageModel
    {
        private readonly INeo4jIdentityServer4Database _neo4JIdentityServer4Database;
        private readonly IIdentityServer4ModelsPopulation<ApplicationUser> _identityServer4ModelsPopulation;
        private readonly UserManager<ApplicationUser> _userManager;
        public Neo4jModel(
            UserManager<ApplicationUser> userManager,
            INeo4jIdentityServer4Database neo4JIdentityServer4Database,
            IIdentityServer4ModelsPopulation<ApplicationUser> identityServer4ModelsPopulation)
        {
            _neo4JIdentityServer4Database = neo4JIdentityServer4Database;
            _identityServer4ModelsPopulation = identityServer4ModelsPopulation;
            _userManager = userManager;

        }

        public async void OnGetAsync()
        {

        }

        public async Task<IActionResult> OnPostAsync(string button)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            switch (button)
            {
                case "dropDatabase":
                    await _neo4JIdentityServer4Database.DropDatabaseAsync();
                    break;
                case "populate":
                    var clients = Config.GetClients();
                    await _neo4JIdentityServer4Database.CreateConstraintsAsync();
                    var result = await _identityServer4ModelsPopulation.InsertClients(user, clients);

                    var apiResource = Config.GetApiResources();
                    result = await _identityServer4ModelsPopulation.InsertApiResources(apiResource);

                    var identityResources = Config.GetIdentityResources();
                    result = await _identityServer4ModelsPopulation.InsertIdentityResources(identityResources);
                    break;

            }

            return Page();
        }
    }
}