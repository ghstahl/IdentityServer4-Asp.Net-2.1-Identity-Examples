using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace PagesWebApp.SupportAgent
{
    /// <summary>
    /// IProfileService to integrate with ASP.NET Identity.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <seealso cref="IdentityServer4.Services.IProfileService" />
    public class ProfileService<TUser> : IProfileService
        where TUser : class
    {
        private IdentityServer4.AspNetIdentity.ProfileService<TUser> _delegateProfileService;
        private readonly ILogger<IdentityServer4.AspNetIdentity.ProfileService<TUser>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityServer4.AspNetIdentity.ProfileService{TUser}"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="claimsFactory">The claims factory.</param>
        public ProfileService(UserManager<TUser> userManager,
            IUserClaimsPrincipalFactory<TUser> claimsFactory)
        {
            _delegateProfileService = new IdentityServer4.AspNetIdentity.ProfileService<TUser>(userManager, claimsFactory);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityServer4.AspNetIdentity.ProfileService{TUser}"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="claimsFactory">The claims factory.</param>
        /// <param name="logger">The logger.</param>
        public ProfileService(UserManager<TUser> userManager,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            ILogger<IdentityServer4.AspNetIdentity.ProfileService<TUser>> logger)
        {
            _delegateProfileService = new IdentityServer4.AspNetIdentity.ProfileService<TUser>(userManager, claimsFactory);
            _logger = logger;
        }

        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            await _delegateProfileService.GetProfileDataAsync(context);
            context.IssuedClaims.Add(new Claim("role","support_agent"));
        }

        /// <summary>
        /// This method gets called whenever identity server needs to determine if the user is valid or active (e.g. if the user's account has been deactivated since they logged in).
        /// (e.g. during token issuance or validation).
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual async Task IsActiveAsync(IsActiveContext context)
        {
            await _delegateProfileService.IsActiveAsync(context);
        }
    }
}