using IdentityModel.Client;

namespace IdentityModelExtras
{
    public interface IDiscoveryCacheContainer
    {
        DiscoveryCache DiscoveryCache { get; }
    }
}