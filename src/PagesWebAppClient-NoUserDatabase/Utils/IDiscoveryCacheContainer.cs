using IdentityModel.Client;

namespace PagesWebAppClient.Utils
{
    public interface IDiscoveryCacheContainer
    {
        DiscoveryCache DiscoveryCache { get; }
    }
}