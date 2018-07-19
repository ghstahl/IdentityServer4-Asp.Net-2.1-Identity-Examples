using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4Extras.Configuration.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityServer4Extras
{
    public class IdentityServerAuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationService _inner;
        private readonly IAuthenticationSchemeProvider _schemes;
        private readonly ISystemClock _clock;
        private readonly ILogger<IdentityServerAuthenticationService> _logger;
        public IdentityServerAuthenticationService(
            Decorator<IAuthenticationService> decorator,
            IAuthenticationSchemeProvider schemes,
            ISystemClock clock,
            ILogger<IdentityServerAuthenticationService> logger)
        {
            _inner = decorator.Instance;

            _schemes = schemes;
            _clock = clock;
            _logger = logger;
        }
        private async Task<string> GetCookieAuthenticationSchemeAsync()
        {
            var scheme = await _schemes.GetDefaultAuthenticateSchemeAsync();
            if (scheme == null)
            {
                throw new InvalidOperationException("No DefaultAuthenticateScheme found.");
            }
            return scheme.Name;
        }
        public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme)
        {
            return _inner.AuthenticateAsync(context, scheme);
        }

        public Task ChallengeAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            return _inner.ChallengeAsync(context, scheme, properties);
        }

        public Task ForbidAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            return _inner.ForbidAsync(context, scheme, properties);
        }

        public async Task SignInAsync(HttpContext context, string scheme, ClaimsPrincipal principal, AuthenticationProperties properties)
        {
            var defaultScheme = await _schemes.GetDefaultSignInSchemeAsync();
            var cookieScheme = await GetCookieAuthenticationSchemeAsync();

            if ((scheme == null && defaultScheme?.Name == cookieScheme) || scheme == cookieScheme)
            {
                AugmentPrincipal(principal);
            }
            else
            {
                var claim = principal.FindFirst("http://schemas.microsoft.com/identity/claims/identityprovider");
                if (claim != null)
                {
                    context.Items.Add(JwtClaimTypes.IdentityProvider, claim.Value);
                }
            }
            await _inner.SignInAsync(context, scheme, principal, properties);
        }
        private void AugmentPrincipal(ClaimsPrincipal principal)
        {
            _logger.LogDebug("Augmenting SignInContext");

            AssertRequiredClaims(principal);
            AugmentMissingClaims(principal, _clock.UtcNow.UtcDateTime);
        }

        private void AugmentMissingClaims(ClaimsPrincipal principal, DateTime authTime)
        {
            var identity = principal.Identities.First();

            // ASP.NET Identity issues this claim type and uses the authentication middleware name
            // such as "Google" for the value. this code is trying to correct/convert that for
            // our scenario. IOW, we take their old AuthenticationMethod value of "Google"
            // and issue it as the idp claim. we then also issue a amr with "external"
            var amr = identity.FindFirst(ClaimTypes.AuthenticationMethod);
            if (amr != null &&
                identity.FindFirst(JwtClaimTypes.IdentityProvider) == null &&
                identity.FindFirst(JwtClaimTypes.AuthenticationMethod) == null)
            {
                _logger.LogDebug("Removing amr claim with value: {value}", amr.Value);
                identity.RemoveClaim(amr);

                _logger.LogDebug("Adding idp claim with value: {value}", amr.Value);
                identity.AddClaim(new Claim(JwtClaimTypes.IdentityProvider, amr.Value));

                _logger.LogDebug("Adding amr claim with value: {value}", Constants.ExternalAuthenticationMethod);
                identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationMethod, Constants.ExternalAuthenticationMethod));
            }

            if (identity.FindFirst(JwtClaimTypes.IdentityProvider) == null)
            {
                _logger.LogDebug("Adding idp claim with value: {value}", IdentityServerConstants.LocalIdentityProvider);
                identity.AddClaim(new Claim(JwtClaimTypes.IdentityProvider, IdentityServerConstants.LocalIdentityProvider));
            }

            if (identity.FindFirst(JwtClaimTypes.AuthenticationMethod) == null)
            {
                if (identity.FindFirst(JwtClaimTypes.IdentityProvider).Value == IdentityServerConstants.LocalIdentityProvider)
                {
                    _logger.LogDebug("Adding amr claim with value: {value}", OidcConstants.AuthenticationMethods.Password);
                    identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationMethod, OidcConstants.AuthenticationMethods.Password));
                }
                else
                {
                    _logger.LogDebug("Adding amr claim with value: {value}", Constants.ExternalAuthenticationMethod);
                    identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationMethod, Constants.ExternalAuthenticationMethod));
                }
            }

            if (identity.FindFirst(JwtClaimTypes.AuthenticationTime) == null)
            {
                var time = authTime.ToEpochTime().ToString();

                _logger.LogDebug("Adding auth_time claim with value: {value}", time);
                identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationTime, time, ClaimValueTypes.Integer));
            }
        }

        private void AssertRequiredClaims(ClaimsPrincipal principal)
        {
            // for now, we don't allow more than one identity in the principal/cookie
            if (principal.Identities.Count() != 1) throw new InvalidOperationException("only a single identity supported");
            if (principal.FindFirst(JwtClaimTypes.Subject) == null) throw new InvalidOperationException("sub claim is missing");
        }
        public async Task SignOutAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            await _inner.SignOutAsync(context, scheme, properties);
        }
    }
}
