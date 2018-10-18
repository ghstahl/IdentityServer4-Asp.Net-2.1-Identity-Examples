using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace IdentityServer4Extras.Services
{
    /// <summary>
    /// IProfileService to integrate with ASP.NET Identity.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <seealso cref="IdentityServer4.Services.IProfileService" />
    public class ProfileServiceAggregator<TUser> : IProfileService
        where TUser : class
    {
        private ProfileService<TUser> _delegate;
        private ILogger _logger;
        private IEnumerable<IProfileServicePlugin> _profileServiceDelegates;

        public ProfileServiceAggregator(
            IdentityServer4.AspNetIdentity.ProfileService<TUser> theDelegate,
            IEnumerable<IProfileServicePlugin> profileServiceDelegates,
            ILogger<ProfileServiceAggregator<TUser>> logger)
        {
            _delegate = theDelegate;
            _profileServiceDelegates = profileServiceDelegates;
            _logger = logger;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            await _delegate.GetProfileDataAsync(context);
            foreach (var plugin in _profileServiceDelegates)
            {
                await plugin.GetProfileDataAsync(context);
            }
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return _delegate.IsActiveAsync(context);
        }
    }
}
