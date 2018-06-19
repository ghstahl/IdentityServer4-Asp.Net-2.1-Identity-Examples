using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PagesWebAppClient.Utils
{
    public class ConfiguredDiscoverCacheContainerFactory
    {
        private IConfiguration _configuration;
        private Dictionary<string, ConfiguredDiscoverCacheContainer> OIDCDiscoverCacheContainers { get; set; }
        private List<OAuth2SchemeRecord> OAuth2SchemeRecords { get; set; }

        public ConfiguredDiscoverCacheContainerFactory(IConfiguration configuration)
        {
            _configuration = configuration;
            var section = configuration.GetSection("oauth2");
            OAuth2SchemeRecords = new List<OAuth2SchemeRecord>();
            section.Bind(OAuth2SchemeRecords);
            OIDCDiscoverCacheContainers = new Dictionary<string, ConfiguredDiscoverCacheContainer>();
            foreach (var record in OAuth2SchemeRecords)
            {
                OIDCDiscoverCacheContainers.Add(record.Scheme, new ConfiguredDiscoverCacheContainer(_configuration, record.Scheme));
            }
        }



        public ConfiguredDiscoverCacheContainer Get(string scheme)
        {
            if (OIDCDiscoverCacheContainers.ContainsKey(scheme))
            {
                return OIDCDiscoverCacheContainers[scheme];
            }
            return null;
        }
    }
}
