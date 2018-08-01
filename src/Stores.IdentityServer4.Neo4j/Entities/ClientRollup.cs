using Newtonsoft.Json;

namespace StoresIdentityServer4.Neo4j.Entities
{
    public class ClientRollup
    {
        private string _clientJson;

        public string ClientJson
        {
            get
            {
                if (
                    string.IsNullOrEmpty(_clientJson) && 
                    _client != null)
                {
                    _clientJson = JsonConvert.SerializeObject(_client);
                }
                return _clientJson;
            }
            set => _clientJson = value;
        }

        private IdentityServer4.Models.Client _client;
        [JsonIgnore]
        public IdentityServer4.Models.Client Client {
            get
            {
                if (_client == null)
                {
                    _client = JsonConvert.DeserializeObject<IdentityServer4.Models.Client>(ClientJson);
                }

                return _client;
            }
            set => _client = value;
        }
    }
}