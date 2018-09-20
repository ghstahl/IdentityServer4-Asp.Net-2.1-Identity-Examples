using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras;
using Neo4jExtras.Extensions;
using Newtonsoft.Json;
using Stores.IdentityServer4Neo4j.Events;
using StoresIdentityServer4.Neo4j.Entities;
using StoresIdentityServer4.Neo4j.Mappers;
using ApiResource = IdentityServer4.Models.ApiResource;

namespace StoresIdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStore<TUser>
        where TUser : Neo4jIdentityUser
    {
        private static readonly string IdSrv4ApiResource;
        private static readonly string IdSrv4ApiResourceClaim;
        private static readonly string IdSrv4ApiSecret;
        private static readonly string IdSrv4ApiResourceRollup;
        private static readonly string IdSrv4ApiResourcesRollup;

        private async Task RaiseApiResourceChangeEventAsync(Neo4jIdentityServer4ApiResource apiResource)
        {
            await _eventService.RaiseAsync(new ApiResourceChangeEvent<Neo4jIdentityServer4ApiResource>(apiResource));
        }

        public async Task<IdentityResult> CreateApiResourceAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            try
            {
                var cypher = $@"CREATE (r:{IdSrv4ApiResource} $p0)";
                await Session.RunAsync(cypher, Params.Create(apiResource.ConvertToMap()));
                await RaiseApiResourceChangeEventAsync(apiResource);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> UpdateApiResourceAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            try
            {
                var cypher = $@"
                MATCH (r:{IdSrv4ApiResource}{{Name: $p0}})
                SET r = $p1";

                await Session.RunAsync(cypher, Params.Create(apiResource.Name, apiResource.ConvertToMap()));
                await RaiseApiResourceChangeEventAsync(apiResource);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteApiResourceAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            await DeleteApiScopesAsync(apiResource, cancellationToken);
            try
            {
                var cypher = $@"
                MATCH (r:{IdSrv4ApiResource}{{Name: $p0}})
                DETACH DELETE r";

                await Session.RunAsync(cypher, Params.Create(apiResource.Name));
                await RaiseApiResourceChangeEventAsync(apiResource);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteApiResourcesAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var apiResources = await GetApiResourcesAsync(cancellationToken);
            foreach (var apiResource in apiResources)
            {
                await DeleteApiResourceAsync(apiResource, cancellationToken);
            }

            return IdentityResult.Success;
        }

        public async Task<Neo4jIdentityServer4ApiResource> GetApiResourceAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));

            var cypher = $@"
                MATCH (r:{IdSrv4ApiResource}{{Name: $p0}})
                RETURN r {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(apiResource.Name));
            var grantTypeRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ApiResource>("r"));
            return grantTypeRecord;
        }

        public async Task<IList<Neo4jIdentityServer4ApiResource>> GetApiResourcesAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();


            var cypher = $@"
                MATCH 
                    (r:{IdSrv4ApiResource})
                RETURN 
                    r{{ .* }}";

            var result = await Session.RunAsync(cypher);

            var records = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ApiResource>("r"));
            return records;
        }

        public async Task<IdentityResult> AddApiResourceClaimAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            Neo4jIdentityServer4ApiResourceClaim apiResourceClaim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            apiResourceClaim.ThrowIfNull(nameof(apiResourceClaim));

            try
            {
                var cypher = $@"
                MATCH 
                    (r:{IdSrv4ApiResource}{{Name: $p0}}) 
                MERGE 
                    (l:{IdSrv4ApiResourceClaim} {"$p1".AsMapForNoNull<Neo4jIdentityServer4ApiResourceClaim>(apiResourceClaim)})
                MERGE 
                    (r)-[:{Neo4jConstants.Relationships.HasClaim}]->(l)";

                var result = await Session.RunAsync(cypher, Params.Create(apiResource.Name, apiResourceClaim));
                await RaiseApiResourceChangeEventAsync(apiResource);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<Neo4jIdentityServer4ApiResourceClaim> GetApiResourceClaimAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            Neo4jIdentityServer4ApiResourceClaim apiResourceClaim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            apiResourceClaim.ThrowIfNull(nameof(apiResourceClaim));

            var cypher = $@"
                MATCH 
                    (r:{IdSrv4ApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasClaim}]->
                    (c:{IdSrv4ApiResourceClaim}{{Type: $p1}}) 
                RETURN c {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(apiResource.Name, apiResourceClaim.Type));
            var record =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ApiResourceClaim>("c"));
            return record;
        }

        public async Task<IdentityResult> DeleteApiResourceClaimAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            Neo4jIdentityServer4ApiResourceClaim apiResourceClaim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            apiResourceClaim.ThrowIfNull(nameof(apiResourceClaim));

            try
            {
                var cypher = $@"
                MATCH 
                    (r:{IdSrv4ApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasClaim}]->
                    (c:{IdSrv4ApiResourceClaim}{{Type: $p1}}) 
                DETACH DELETE c";

                await Session.RunAsync(cypher,
                    Params.Create(
                        apiResource.Name,
                        apiResourceClaim.Type));
                await RaiseApiResourceChangeEventAsync(apiResource);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IList<Neo4jIdentityServer4ApiResourceClaim>> GetApiResourceClaimsAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));

            var cypher = $@"
                MATCH 
                    (:{IdSrv4ApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasClaim}]->
                    (s:{IdSrv4ApiResourceClaim})
                RETURN 
                    s{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(apiResource.Name));

            var records = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ApiResourceClaim>("s"));
            return records;
        }

        public async Task<IdentityResult> DeleteApiResourceClaimsAsync(Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));

            try
            {
                var cypher = $@"
                MATCH 
                   (:{IdSrv4ApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasClaim}]->
                    (s:{IdSrv4ApiResourceClaim})
                DETACH DELETE s";

                await Session.RunAsync(cypher,
                    Params.Create(apiResource.Name));
                await RaiseApiResourceChangeEventAsync(apiResource);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }


        public async Task<IdentityResult> AddApiSecretAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            Neo4jIdentityServer4ApiSecret apiSecret,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            apiSecret.ThrowIfNull(nameof(apiSecret));

            try
            {
                var cypher = $@"
                MATCH 
                    (r:{IdSrv4ApiResource}{{Name: $p0}}) 
                MERGE 
                    (s:{IdSrv4ApiSecret} {"$p1".AsMapForNoNull<Neo4jIdentityServer4ApiSecret>(apiSecret)})
                MERGE 
                    (r)-[:{Neo4jConstants.Relationships.HasSecret}]->(s)";

                var result = await Session.RunAsync(cypher, Params.Create(apiResource.Name, apiSecret));
                await RaiseApiResourceChangeEventAsync(apiResource);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<Neo4jIdentityServer4ApiSecret> GetApiSecretAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            Neo4jIdentityServer4ApiSecret apiSecret,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            apiSecret.ThrowIfNull(nameof(apiSecret));

            var cypher = $@"
                MATCH 
                    (:{IdSrv4ApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasSecret}]->
                    (s:{IdSrv4ApiSecret}{{Type: $p1,Value: $p2}}) 
                RETURN s {{ .* }}";

            var result = await Session.RunAsync(cypher,
                Params.Create(apiResource.Name, apiSecret.Type, apiSecret.Value));
            var record =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ApiSecret>("s"));
            return record;
        }

        public async Task<IdentityResult> DeleteApiSecretAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            Neo4jIdentityServer4ApiSecret apiSecret,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            apiSecret.ThrowIfNull(nameof(apiSecret));

            try
            {
                var cypher = $@"
                MATCH 
                    (r:{IdSrv4ApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasSecret}]->
                    (s:{IdSrv4ApiSecret}{{Type: $p1,Value: $p2}}) 
                DETACH DELETE s";

                await Session.RunAsync(cypher,
                    Params.Create(
                        apiResource.Name,
                        apiSecret.Type, apiSecret.Value));
                await RaiseApiResourceChangeEventAsync(apiResource);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteApiSecretsAsync(Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));

            try
            {
                var cypher = $@"
                MATCH 
                    (:{IdSrv4ApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasSecret}]->
                    (s:{IdSrv4ApiSecret}) 
                DETACH DELETE s";

                await Session.RunAsync(cypher,
                    Params.Create(
                        apiResource.Name));
                await RaiseApiResourceChangeEventAsync(apiResource);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IList<Neo4jIdentityServer4ApiSecret>> GetApiSecretsAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));

            var cypher = $@"
                MATCH 
                    (:{IdSrv4ApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasSecret}]->
                    (s:{IdSrv4ApiSecret})
                RETURN 
                    s{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(apiResource.Name));

            var records = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ApiSecret>("s"));
            return records;
        }

        public async Task<List<ApiResource>> RollupApiResourcesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var foundApiResources = await GetApiResourcesAsync(cancellationToken);
            List<IdentityServer4.Models.ApiResource> modelApiResources  = new List<ApiResource>();
            foreach (var item in foundApiResources)
            {
                var apiResource = await GetRollupAsync(item, cancellationToken);
                modelApiResources.Add(apiResource);
            }

            var json = JsonConvert.SerializeObject(modelApiResources);
            var neo4jObject = new Neo4jIdentityServer4ApiResourcesRollup()
            {
                Name = "OneAndOnly",
                ApiResourcesJson = json
            };
            try
            {
                var cypher = $@"CREATE (r:{IdSrv4ApiResourcesRollup} $p0)";
                await Session.RunAsync(cypher, Params.Create(neo4jObject.ConvertToMap()));
                return modelApiResources;
            }
            catch (ClientException ex)
            {
                return null;
            }
        }

        public async Task<List<ApiResource>> GetApiResoucesRollupAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
        

            var cypher = $@"
                MATCH 
                    (r:{IdSrv4ApiResourcesRollup}{{Name: 'OneAndOnly'}})
                RETURN r{{ .* }}";

            var result = await Session.RunAsync(cypher);
            List<ApiResource> model = null;
            var foundRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<ApiResourcesRollup>("r"));

            if (foundRecord == null)
            {
                model = await RollupApiResourcesAsync(cancellationToken);
            }
            else
            {
                model = JsonConvert.DeserializeObject<List<ApiResource>>(foundRecord.ApiResourcesJson);
            }

            return model;
        }

        public async Task<IdentityResult> DeleteApiResoucesRollupAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            
            try
            {
                var cypher = $@"
                MATCH 
                    (r:{IdSrv4ApiResourcesRollup}{{Name: 'OneAndOnly'}}) 
                DETACH DELETE r";

                await Session.RunAsync(cypher);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }
        public class ModelsScopeComparer : IEqualityComparer<IdentityServer4.Models.Scope>
        {
            public bool Equals(IdentityServer4.Models.Scope x, IdentityServer4.Models.Scope y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(IdentityServer4.Models.Scope obj)
            {
                return obj.Name.GetHashCode();
            }
        }
        public async Task<ApiResource> RollupAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            var foundApiResource = await GetApiResourceAsync(apiResource, cancellationToken);
            ApiResource model = null;
            if (foundApiResource != null)
            {
                model = foundApiResource.ToModel();
                var secrets = await GetApiSecretsAsync(apiResource, cancellationToken);
                foreach (var secret in secrets)
                {
                    model.ApiSecrets.Add(secret.ToModel());
                }

                var claims = await GetApiResourceClaimsAsync(apiResource, cancellationToken);
                foreach (var claim in claims)
                {
                    model.UserClaims.Add(claim.Type);
                }

                var apiScopes = await GetApiScopesAsync(apiResource, cancellationToken);
                foreach (var apiScope in apiScopes)
                {
                    var apiScopeModel = await GetRollupAsync(apiResource, apiScope, cancellationToken);
                    model.Scopes.Add(apiScopeModel);
                }

                var distinctList = model.Scopes.Distinct(new ModelsScopeComparer());
                model.Scopes = distinctList.ToList();
               
                var rollup = new ApiResourceRollup()
                {
                    ApiResourceJson = JsonConvert.SerializeObject(model),
                };
                await AddRollupAsync(apiResource, rollup);
            }


            return model;
        }

        private async Task<IdentityResult> AddRollupAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            ApiResourceRollup rollup,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            rollup.ThrowIfNull(nameof(rollup));

            try
            {
                var cypher = $@"
                    MATCH (c:{IdSrv4ApiResource}{{Name: $p0}})        
                    MERGE (rollup:{IdSrv4ApiResourceRollup} {"$p1".AsMapForNoNull<ApiResourceRollup>(rollup)})
                    MERGE (c)-[:{Neo4jConstants.Relationships.HasRollup}]->(rollup)";

                var result = await Session.RunAsync(cypher, Params.Create(apiResource.Name, rollup));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<ApiResource> GetRollupAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));

            var cypher = $@"
                MATCH 
                    (:{IdSrv4ApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasRollup}]->
                    (r:{IdSrv4ApiResourceRollup})
                RETURN r{{ .* }}";

            var result = await Session.RunAsync(cypher,
                Params.Create(apiResource.Name));

            IdentityServer4.Models.ApiResource model = null;
            var foundRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<ApiResourceRollup>("r"));
            if (foundRecord == null)
            {
                model = await RollupAsync(apiResource, cancellationToken);
            }
            else
            {
                model = JsonConvert.DeserializeObject<ApiResource>(foundRecord.ApiResourceJson);
            }

            return model;
        }

        public async Task<IdentityResult> DeleteRollupAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));

            try
            {
                var cypher = $@"
                MATCH 
                    (:{IdSrv4ApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasRollup}]->
                    (r:{IdSrv4ApiResourceRollup}) 
                DETACH DELETE r";

                await Session.RunAsync(cypher,
                    Params.Create(apiResource.Name));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }
    }
}
