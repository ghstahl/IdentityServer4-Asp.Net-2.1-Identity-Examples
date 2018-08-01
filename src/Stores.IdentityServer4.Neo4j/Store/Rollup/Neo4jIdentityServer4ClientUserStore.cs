using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras;
using Neo4jExtras.Extensions;
using Newtonsoft.Json;
using StoresIdentityServer4.Neo4j;
using StoresIdentityServer4.Neo4j.Entities;
using StoresIdentityServer4.Neo4j.Mappers;
using Client = IdentityServer4.Models.Client;

namespace StoresIdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStore<TUser> where TUser : Neo4jIdentityUser
    {
        private static readonly string IdSrv4ClientRollup;
      

        public async Task<IdentityServer4.Models.Client> RollupAsync(Neo4jIdentityServer4Client client, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            await RaiseClientChangeEventAsync(client);
            var finalResult = new Client();
            var clientFound = await FindClientByClientIdAsync(client.ClientId, cancellationToken);
            var model = clientFound.ToModel();
            var secrets = await GetSecretsAsync(client, cancellationToken);
            if (secrets != null)
            {
                foreach (var item in secrets)
                {
                    model.ClientSecrets.Add(item);
                }
            }

            var allowedGrants = await GetAllowedGrantTypesAsync(client, cancellationToken);
            if (allowedGrants != null)
            {
                foreach (var item in allowedGrants)
                {
                    model.AllowedGrantTypes.Add(item.GrantType);
                }
            }

           
            var corsOrigins = await GetCorsOriginsAsync(client, cancellationToken);
            if (corsOrigins != null)
            {
                foreach (var item in corsOrigins)
                {
                    model.AllowedCorsOrigins.Add(item.Origin);
                }
            }
            var idpRestrictions = await GetIdPRestrictionsAsync(client, cancellationToken);
            if (idpRestrictions != null)
            {
                foreach (var item in idpRestrictions)
                {
                    model.IdentityProviderRestrictions.Add(item.Provider);
                }
            }
            var postLogoutRedirectUris = await GetPostLogoutRedirectUrisAsync(client, cancellationToken);
            if (postLogoutRedirectUris != null)
            {
                foreach (var item in postLogoutRedirectUris)
                {
                    model.PostLogoutRedirectUris.Add(item.PostLogoutRedirectUri);
                }
            }
            var properties = await GetPropertiesAsync(client, cancellationToken);
            if (properties != null)
            {
                foreach (var item in properties)
                {
                    model.Properties.Add(item.Key,item.Value );
                }
            }
            var redirectUris = await GetRedirectUrisAsync(client, cancellationToken);
            if (redirectUris != null)
            {
                foreach (var item in redirectUris)
                {
                    model.RedirectUris.Add(item.RedirectUri);
                }
            }
            var scopes = await GetScopesAsync(client, cancellationToken);
            if (redirectUris != null)
            {
                foreach (var item in scopes)
                {
                    model.AllowedScopes.Add(item.Scope);
                }
            }
            var claims = await GetClaimsAsync(client, cancellationToken);
            string claimsJson = null;
            if (claims != null)
            {
                claimsJson = JsonConvert.SerializeObject(claims);
            }
            var rollup = new ClientRollup()
            {
                ClientJson = JsonConvert.SerializeObject(model),
                ClaimsJson = claimsJson
            };
            var result = await AddRollupToClientAsync(client, rollup, cancellationToken);

            if (claims != null)
            {
                foreach (var item in claims)
                {
                    model.Claims.Add(new Claim(item.Type, item.Value));
                }
            }
            return model;
        }

        public async Task<IdentityServer4.Models.Client> GetRollupAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                MATCH (c:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasRollup}]->(rollup:{IdSrv4ClientRollup})
                WHERE c.ClientId = $p0
                RETURN rollup{{ .* }}";

            var result = await Session.RunAsync(cypher,
                Params.Create(
                    client.ClientId));

            IdentityServer4.Models.Client model = null;
            var foundRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<ClientRollup>("rollup"));
            if (foundRecord == null)
            {
                model = await RollupAsync(client, cancellationToken);
            }
            else
            {
                model = JsonConvert.DeserializeObject<Client>(foundRecord.ClientJson);
                if (!string.IsNullOrEmpty(foundRecord.ClaimsJson))
                {
                    var claims = JsonConvert.DeserializeObject<List<Neo4jIdentityServer4ClientClaim>>(foundRecord.ClaimsJson);
                    foreach (var item in claims)
                    {
                        model.Claims.Add(new Claim(item.Type, item.Value));
                    }
                }
            }
            return model;
        }

        public async Task<IdentityResult> DeleteRollupAsync(Neo4jIdentityServer4Client client, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
          
            try
            {
                var cypher = $@"
                MATCH (c:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasRollup}]->(rollup:{IdSrv4ClientRollup})
                WHERE c.ClientId = $p0 
                DETACH DELETE rollup";

                await Session.RunAsync(cypher,
                    Params.Create(client.ClientId));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> AddRollupToClientAsync(
            Neo4jIdentityServer4Client client,
            ClientRollup rollup,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            rollup.ThrowIfNull(nameof(rollup));
            try
            {
                var cypher = $@"
                MATCH (c:{IdSrv4Client} {{ClientId: $p0}})
                MERGE (rollup:{IdSrv4ClientRollup} {"$p1".AsMapFor<Neo4jIdentityServer4ClientRollup>()})
                MERGE (c)-[:{Neo4jConstants.Relationships.HasRollup}]->(rollup)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, rollup));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }
    }
}