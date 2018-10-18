using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IdentityServer4Extras.Services
{
    /// <summary>
    /// IProfileService to integrate with ASP.NET Identity.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <seealso cref="IdentityServer4.Services.IProfileService" />
    public class EndUserKBAProfileService : IProfileServicePlugin
    {
        private ILogger _logger;

        public EndUserKBAProfileService(ILogger<EndUserKBAProfileService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var query = from item in context.Subject.Claims
                where item.Type.StartsWith("agent:", StringComparison.InvariantCultureIgnoreCase)
                select item;
            var claims = query.ToList();
            context.IssuedClaims.AddRange(claims);
            if (claims.Any())
            {
                context.IssuedClaims.Add(new Claim("role", "agent_proxy"));
            }
        }
    }
}