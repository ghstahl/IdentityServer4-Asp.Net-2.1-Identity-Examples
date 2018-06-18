using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace PagesWebApp.Areas.Identity.Pages.Consent
{
    public class KV
    {
        public string Name { get; set; }
        public bool Check { get; set; }
    }

    public class IndexModel : PageModel
    {
        private readonly IIdentityServerInteractionService _interaction;
        private ILogger<IndexModel> _logger;
        private IClientStore _clientStore;
        private IResourceStore _resourceStore;

        [BindProperty] public ConsentViewModel ConsentViewModel { get; set; }
        [BindProperty] public List<ScopeViewModel> IdentityScopes { get; set; }
        [BindProperty] public bool TrueRequiredPlaceHolder { get; set; } = true;
        public IndexModel(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IResourceStore resourceStore,
            ILogger<IndexModel> logger)
        {
            _interaction = interaction;
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            ConsentViewModel = await BuildViewModelAsync(returnUrl);
            IdentityScopes = ConsentViewModel.IdentityScopes;
            if (ConsentViewModel != null)
            {
                
                return Page();
            }

            throw new Exception("Consent Error");
        }

        public async Task<IActionResult> OnPostAsync(string button)
        {
            ConsentViewModel.Button = button;
            ConsentViewModel.ScopesConsented = new List<string>();
            var query = from item in ConsentViewModel.IdentityScopes
                where item.Checked == true
                let c = item.Name
                select c;
            ConsentViewModel.ScopesConsented.AddRange(query);
            if (ConsentViewModel.ResourceScopes != null)
            {
                query = from item in ConsentViewModel.ResourceScopes
                    where item.Checked == true
                    let c = item.Name
                    select c;
                ConsentViewModel.ScopesConsented.AddRange(query);
            }

            var result = await ProcessConsent(ConsentViewModel);
            if (result.IsRedirect)
            {
                return Redirect(result.RedirectUri);
            }

            if (result.HasValidationError)
            {
                ModelState.AddModelError("", result.ValidationError);
            }

            if (result.ShowView)
            {
                return Page();
            }

            throw new Exception("Consent Error");
        }

        /*****************************************/
        /* helper APIs for the ConsentController */
        /*****************************************/
        private async Task<ProcessConsentResult> ProcessConsent(ConsentInputModel model)
        {
            var result = new ProcessConsentResult();

            ConsentResponse grantedConsent = null;

            // user clicked 'no' - send back the standard 'access_denied' response
            if (model.Button == "no")
            {
                grantedConsent = ConsentResponse.Denied;
            }
            // user clicked 'yes' - validate the data
            else if (model.Button == "yes" && model != null)
            {
               
                // if the user consented to some scope, build the response model
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    var scopes = model.ScopesConsented;
                    if (ConsentOptions.EnableOfflineAccess == false)
                    {
                        scopes = scopes.Where(x =>
                            x != IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess).ToList();
                    }

                    grantedConsent = new ConsentResponse
                    {
                        RememberConsent = model.RememberConsent,
                        ScopesConsented = scopes.ToArray()
                    };
                }
                else
                {
                    result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
                }
            }
            else
            {
                result.ValidationError = ConsentOptions.InvalidSelectionErrorMessage;
            }

            if (grantedConsent != null)
            {
                // validate return url is still valid
                var request = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
                if (request == null) return result;

                // communicate outcome of consent back to identityserver
                await _interaction.GrantConsentAsync(request, grantedConsent);

                // indicate that's it ok to redirect back to authorization endpoint
                result.RedirectUri = model.ReturnUrl;
            }
            else
            {
                // we need to redisplay the consent UI
                result.ViewModel = await BuildViewModelAsync(model.ReturnUrl, model);
            }

            return result;
        }

        private async Task<ConsentViewModel> BuildViewModelAsync(string returnUrl, ConsentInputModel model = null)
        {
            var request = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (request != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(request.ClientId);
                if (client != null)
                {
                    var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
                    if (resources != null && (resources.IdentityResources.Any() || resources.ApiResources.Any()))
                    {
                        return CreateConsentViewModel(model, returnUrl, request, client, resources);
                    }
                    else
                    {
                        _logger.LogError("No scopes matching: {0}",
                            request.ScopesRequested.Aggregate((x, y) => x + ", " + y));
                    }
                }
                else
                {
                    _logger.LogError("Invalid client id: {0}", request.ClientId);
                }
            }
            else
            {
                _logger.LogError("No consent request matching request: {0}", returnUrl);
            }

            return null;
        }

        private ConsentViewModel CreateConsentViewModel(
            ConsentInputModel model, string returnUrl,
            AuthorizationRequest request,
            Client client, Resources resources)
        {
            var vm = new ConsentViewModel();
            vm.RememberConsent = model?.RememberConsent ?? true;
            vm.ScopesConsented = (model?.ScopesConsented ?? Enumerable.Empty<string>()).ToList();

            vm.ReturnUrl = returnUrl;

            vm.ClientName = client.ClientName ?? client.ClientId;
            vm.ClientUrl = client.ClientUri;
            vm.ClientLogoUrl = client.LogoUri;
            vm.AllowRememberConsent = client.AllowRememberConsent;

            vm.IdentityScopes = (resources.IdentityResources
                    .Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null))
                    .ToArray())
                .ToList();
            vm.ResourceScopes = (resources.ApiResources.SelectMany(x => x.Scopes).Select(x =>
                CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null)).ToArray()).ToList();
            if (ConsentOptions.EnableOfflineAccess && resources.OfflineAccess)
            {
                vm.ResourceScopes = (vm.ResourceScopes.Union(new ScopeViewModel[]
                {
                    GetOfflineAccessScope(
                        vm.ScopesConsented.Contains(
                            IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess) || model == null)
                })).ToList();
            }

            return vm;
        }

        private ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
        {
            return new ScopeViewModel
            {
                Name = identity.Name,
                DisplayName = identity.DisplayName,
                Description = identity.Description,
                Emphasize = identity.Emphasize,
                Required = identity.Required,
                Checked = check || identity.Required
            };
        }

        public ScopeViewModel CreateScopeViewModel(Scope scope, bool check)
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Emphasize = scope.Emphasize,
                Required = scope.Required,
                Checked = check || scope.Required
            };
        }

        private ScopeViewModel GetOfflineAccessScope(bool check)
        {
            return new ScopeViewModel
            {
                Name = IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = ConsentOptions.OfflineAccessDisplayName,
                Description = ConsentOptions.OfflineAccessDescription,
                Emphasize = true,
                Checked = check
            };
        }
    }
}