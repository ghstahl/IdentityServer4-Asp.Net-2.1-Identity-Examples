using System.Collections.Generic;
using System.Linq;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;

namespace PagesWebAppClient.Utils
{
    public class ConfiguredDiscoverCacheContainer : IDiscoveryCacheContainer
    {
        private IConfiguration _configuration;
        private DiscoveryCache _discoveryCache { get; set; }
        private string Scheme { get; set; }
        private List<OAuth2SchemeRecord> OAuth2SchemeRecords { get; set; }
        public ConfiguredDiscoverCacheContainer(IConfiguration configuration, string scheme)
        {
            _configuration = configuration;
            var section = configuration.GetSection("oauth2");
            OAuth2SchemeRecords = new List<OAuth2SchemeRecord>();
            section.Bind(OAuth2SchemeRecords);
            Scheme = scheme;
        }
        public DiscoveryCache DiscoveryCache
        {
            get
            {
                if (_discoveryCache == null)
                {
                    var query = from item in OAuth2SchemeRecords
                        where item.Scheme == Scheme
                        select item;
                    var record = query.FirstOrDefault();

                    var discoveryClient = new DiscoveryClient(record.Authority) { Policy = { ValidateEndpoints = false } };
                    if (record.AdditionalEndpointBaseAddresses != null)
                    {
                        foreach (var additionalEndpointBaseAddress in record.AdditionalEndpointBaseAddresses)
                        {
                            discoveryClient.Policy.AdditionalEndpointBaseAddresses.Add(additionalEndpointBaseAddress);
                        }
                    }
                    _discoveryCache = new DiscoveryCache(discoveryClient);
                }
                return _discoveryCache;
            }
        }
    }
}