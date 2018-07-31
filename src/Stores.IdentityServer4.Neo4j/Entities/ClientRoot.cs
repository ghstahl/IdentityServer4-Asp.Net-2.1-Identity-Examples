using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4;
using IdentityServer4.Models;
using Newtonsoft.Json;

namespace Stores.IdentityServer4.Neo4j.Entities
{
    public class TestConverter<T> : JsonConverter
        where T : class, new()
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var val = JsonConvert.SerializeObject(value);
            serializer.Serialize(writer, val);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var dVal = serializer.Deserialize<T>(reader);
            var sVal = serializer.Deserialize<string>(reader);
            var val = JsonConvert.DeserializeObject<T>(sVal);

            return val;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }
    }

    public class ClientRoot
    {

        public bool Enabled { get; set; } = true;
        public string ClientId { get; set; }
        public string ProtocolType { get; set; } = IdentityServerConstants.ProtocolTypes.OpenIdConnect;

        public string ClientSecretsJson
        {
            get => JsonConvert.SerializeObject(ClientSecrets);
            set => ClientSecrets = JsonConvert.DeserializeObject<List<Secret>>(value);
        }
        [JsonIgnore]
        public List<Secret> ClientSecrets { get; set; }
        
        public bool RequireClientSecret { get; set; } = true;
        public string ClientName { get; set; }
        public string Description { get; set; }
        public string ClientUri { get; set; }
        public string LogoUri { get; set; }
        public bool RequireConsent { get; set; } = true;
        public bool AllowRememberConsent { get; set; } = true;
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        public string AllowedGrantTypesJson
        {
            get => JsonConvert.SerializeObject(AllowedGrantTypes);
            set => AllowedGrantTypes = JsonConvert.DeserializeObject<List<ClientGrantType>>(value);
        }
        [JsonIgnore]
        public List<ClientGrantType> AllowedGrantTypes { get; set; }
        public bool RequirePkce { get; set; }
        public bool AllowPlainTextPkce { get; set; }

        public bool AllowAccessTokensViaBrowser { get; set; }

        public string RedirectUrisJson
        {
            get => JsonConvert.SerializeObject(RedirectUris);
            set => RedirectUris = JsonConvert.DeserializeObject<List<ClientRedirectUri>>(value);
        }
        [JsonIgnore]
        public List<ClientRedirectUri> RedirectUris { get; set; }
        public string PostLogoutRedirectUrisJson
        {
            get => JsonConvert.SerializeObject(PostLogoutRedirectUris);
            set => PostLogoutRedirectUris = JsonConvert.DeserializeObject<List<ClientPostLogoutRedirectUri>>(value);
        }
        [JsonIgnore]
        public List<ClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; set; }
        public string FrontChannelLogoutUri { get; set; }
        public bool FrontChannelLogoutSessionRequired { get; set; } = true;
        public string BackChannelLogoutUri { get; set; }
        public bool BackChannelLogoutSessionRequired { get; set; } = true;

        public bool AllowOfflineAccess { get; set; }

        public string AllowedScopesJson
        {
            get => JsonConvert.SerializeObject(AllowedScopes);
            set => AllowedScopes = JsonConvert.DeserializeObject<List<ClientScope>>(value);
        }
        [JsonIgnore]
        public List<ClientScope> AllowedScopes { get; set; }
        public int IdentityTokenLifetime { get; set; } = 300;
        public int AccessTokenLifetime { get; set; } = 3600;
        public int AuthorizationCodeLifetime { get; set; } = 300;
        public int? ConsentLifetime { get; set; } = null;
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;
        public int RefreshTokenUsage { get; set; } = (int) TokenUsage.OneTimeOnly;
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
        public int RefreshTokenExpiration { get; set; } = (int) TokenExpiration.Absolute;
        public int AccessTokenType { get; set; } = (int) 0; // AccessTokenType.Jwt;

        public bool EnableLocalLogin { get; set; } = true;

        public string IdentityProviderRestrictionsJson
        {
            get => JsonConvert.SerializeObject(IdentityProviderRestrictions);
            set => IdentityProviderRestrictions = JsonConvert.DeserializeObject<List<ClientIDPRestriction>>(value);
        }
        [JsonIgnore]
        public List<ClientIDPRestriction> IdentityProviderRestrictions { get; set; }
        public bool IncludeJwtId { get; set; }

        public string ClaimsJson
        {
            get => JsonConvert.SerializeObject(Claims);
            set => Claims = JsonConvert.DeserializeObject<List<ClientClaim>>(value);
        }
        [JsonIgnore]
        public List<ClientClaim> Claims { get; set; }
        public bool AlwaysSendClientClaims { get; set; }
        public string ClientClaimsPrefix { get; set; } = "client_";

        public string PairWiseSubjectSalt { get; set; }

        public string AllowedCorsOriginsJson
        {
            get => JsonConvert.SerializeObject(AllowedCorsOrigins);
            set => AllowedCorsOrigins = JsonConvert.DeserializeObject<List<ClientCorsOrigin>>(value);
        }
        [JsonIgnore]
        public List<ClientCorsOrigin> AllowedCorsOrigins { get; set; }
        public string PropertiesJson
        {
            get => JsonConvert.SerializeObject(Properties);
            set => Properties = JsonConvert.DeserializeObject<List<ClientProperty>>(value);
        }
        [JsonIgnore]
        public List<ClientProperty> Properties { get; set; }
        public bool RequireRefreshClientSecret { get; set; }
        public bool AllowArbitraryLocalRedirectUris { get; set; }
    }
}
