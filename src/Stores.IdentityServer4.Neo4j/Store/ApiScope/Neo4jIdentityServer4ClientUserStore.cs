using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras;
using Neo4jExtras.Extensions;
using Newtonsoft.Json;
using Stores.IdentityServer4Neo4j.Events;
using StoresIdentityServer4.Neo4j.Entities;
using StoresIdentityServer4.Neo4j.Mappers;
using Client = IdentityServer4.Models.Client;

namespace StoresIdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStore<TUser> 
        where TUser : Neo4jIdentityUser
    {
        private Task RaiseApiScopeChangeEventAsync(Neo4jIdentityServer4ApiResource apiResource,Neo4jIdentityServer4ApiScope apiScope)
        {
            return _eventService.RaiseAsync(new ApiScopeChangeEvent<Neo4jIdentityServer4ApiResource,Neo4jIdentityServer4ApiScope>(apiResource,apiScope));
        }

        private static readonly string IdSrv4ClientApiScope;
        private static readonly string IdSrv4ClientApiScopeClaim;
        private static readonly string IdSrv4ApiScopeRollup;
        private static readonly string IdSrv4ApiResourceRollup;

        public async Task<IdentityResult> AddApiScopeAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            Neo4jIdentityServer4ApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            apiScope.ThrowIfNull(nameof(apiScope));
            try
            {
                var cypher = $@"
                MATCH 
                    (r:{IdSrv4ClientApiResource}{{Name: $p0}})
                MERGE 
                    (l:{IdSrv4ClientApiScope} {"$p1".AsMapFor<Neo4jIdentityServer4ApiScope>()})
                MERGE 
                    (r)-[:{Neo4jConstants.Relationships.HasScope}]->(l)";

                var result = await Session.RunAsync(cypher, Params.Create(apiResource.Name, apiScope));
                await RaiseApiResourceChangeEventAsync(apiResource);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IList<Neo4jIdentityServer4ApiScope>> GetApiScopesAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));

            var cypher = $@"
                 MATCH 
                        (:{IdSrv4ClientApiResource}{{Name: $p0}})
                        -[:{Neo4jConstants.Relationships.HasScope}]->
                        (s:{IdSrv4ClientApiScope})
                RETURN s{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(apiResource.Name));

            var records = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ApiScope>("s"));
            return records;
        }
        public async Task<Neo4jIdentityServer4ApiScope> GetApiScopeAsync(
            Neo4jIdentityServer4ApiResource apiResource, 
            Neo4jIdentityServer4ApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            apiScope.ThrowIfNull(nameof(apiScope));

            var cypher = $@"
                MATCH (apiResource:{IdSrv4ClientApiResource})-[:{Neo4jConstants.Relationships.HasScope}]->(apiScope:{IdSrv4ClientApiScope})
                WHERE apiResource.Name = $p0 AND apiScope.Name = $p1
                RETURN apiScope {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(apiResource.Name, apiScope.Name));
            var record =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ApiScope>("apiScope"));
            return record;
        }

        public async Task<IdentityResult> UpdateApiScopeAsync(
            Neo4jIdentityServer4ApiResource apiResource, 
            Neo4jIdentityServer4ApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            apiScope.ThrowIfNull(nameof(apiScope));

            try
            {
                var cypher = $@"
                    MATCH 
                        (:{IdSrv4ClientApiResource}{{Name: $p0}})
                        -[:{Neo4jConstants.Relationships.HasScope}]->
                        (apiScope:{IdSrv4ClientApiScope}{{Name: $p1}})
                    SET apiScope = $p1";

                await Session.RunAsync(cypher, Params.Create(apiResource.Name, apiScope.Name, apiScope.ConvertToMap()));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteApiScopeAsync(
            Neo4jIdentityServer4ApiResource apiResource, 
            Neo4jIdentityServer4ApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            apiScope.ThrowIfNull(nameof(apiScope));
        
            await DeleteApiScopeClaimsFromApiScopeAsync(apiResource,apiScope, cancellationToken);
            try
            {
                var cypher = $@"
                    MATCH 
                        (:{IdSrv4ClientApiResource}{{Name: $p0}})
                        -[:{Neo4jConstants.Relationships.HasScope}]->
                        (apiScope:{IdSrv4ClientApiScope}{{Name: $p1}})
                    DETACH DELETE apiScope";

                await Session.RunAsync(cypher, Params.Create(apiResource.Name, apiScope.Name));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteApiScopesAsync(
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
                        (:{IdSrv4ClientApiResource}{{Name: $p0}})
                        -[:{Neo4jConstants.Relationships.HasScope}]->
                        (apiScope:{IdSrv4ClientApiScope})
                DETACH DELETE apiScope";

                await Session.RunAsync(cypher, Params.Create(apiResource.Name));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> AddApiScopeClaimToApiScopeAsync(
            Neo4jIdentityServer4ApiResource apiResource, 
            Neo4jIdentityServer4ApiScope apiScope,
            Neo4jIdentityServer4ApiScopeClaim apiScopeClaim, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            apiScope.ThrowIfNull(nameof(apiScope));
            apiScopeClaim.ThrowIfNull(nameof(apiScopeClaim));
            try
            {
                var cypher = $@"
                MATCH 
                    (apiResource:{IdSrv4ClientApiResource}{{Name: $p0}})
                    -[:{Neo4jConstants.Relationships.HasScope}]->
                    (apiScope:{IdSrv4ClientApiScope}{{Name: $p1}})
                MERGE 
                    (l:{IdSrv4ClientApiScopeClaim} {"$p2".AsMapFor<Neo4jIdentityServer4ApiScopeClaim>()})
                MERGE 
                    (apiScope)-[:{Neo4jConstants.Relationships.HasClaim}]->(l)";

                var result = await Session.RunAsync(cypher, Params.Create(apiResource.Name, apiScope.Name, apiScopeClaim));
                await RaiseApiScopeChangeEventAsync(apiResource,apiScope);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteApiScopeClaimFromApiScopeAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            Neo4jIdentityServer4ApiScope apiScope, 
            Neo4jIdentityServer4ApiScopeClaim apiScopeClaim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiScope.ThrowIfNull(nameof(apiResource));
            apiScope.ThrowIfNull(nameof(apiScope));
            apiScopeClaim.ThrowIfNull(nameof(apiScopeClaim));
            try
            {
                var cypher = $@"
                MATCH 
                    (apiResource:{IdSrv4ClientApiResource}{{Name: $p0}})
                    -[:{Neo4jConstants.Relationships.HasScope}]->
                    (apiScope:{IdSrv4ClientApiScope}{{Name: $p1}})
                    -[:{Neo4jConstants.Relationships.HasClaim}]->
                    (s:{IdSrv4ClientApiScopeClaim}{{Type: $p2}})
                DETACH DELETE s";

                await Session.RunAsync(cypher,
                    Params.Create(
                        apiResource.Name,
                        apiScope.Name,
                        apiScopeClaim.Type));
                await RaiseApiScopeChangeEventAsync(apiResource,apiScope);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteApiScopeClaimsFromApiScopeAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            Neo4jIdentityServer4ApiScope apiScope, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            apiScope.ThrowIfNull(nameof(apiScope));

            try
            {
                var cypher = $@"
                    MATCH 
                        (apiResource:{IdSrv4ClientApiResource}{{Name: $p0}})
                        -[:{Neo4jConstants.Relationships.HasScope}]->
                        (apiScope:{IdSrv4ClientApiScope}{{Name: $p1}})
                        -[:{Neo4jConstants.Relationships.HasClaim}]->
                        (s:{IdSrv4ClientApiScopeClaim})
                DETACH DELETE s";

                await Session.RunAsync(cypher,
                    Params.Create(
                        apiResource.Name,
                        apiScope.Name));
                await RaiseApiScopeChangeEventAsync(apiResource, apiScope);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IList<Neo4jIdentityServer4ApiScopeClaim>> GetApiScopeClaimsAsync(
            Neo4jIdentityServer4ApiResource apiResource, 
            Neo4jIdentityServer4ApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiScope.ThrowIfNull(nameof(apiScope));

            var cypher = $@"
                 MATCH 
                        (:{IdSrv4ClientApiResource}{{Name: $p0}})
                        -[:{Neo4jConstants.Relationships.HasScope}]->
                        (:{IdSrv4ClientApiScope}{{Name: $p1}})
                        -[:{Neo4jConstants.Relationships.HasClaim}]->
                        (s:{IdSrv4ClientApiScopeClaim})
                RETURN s{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(apiResource.Name,apiScope.Name));

            var records = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ApiScopeClaim>("s"));
            return records;
        }

        public async Task<Scope> RollupAsync(
            Neo4jIdentityServer4ApiResource apiResource, 
            Neo4jIdentityServer4ApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            apiScope.ThrowIfNull(nameof(apiScope));

            await RaiseApiScopeChangeEventAsync(apiResource,apiScope);
            var finalResult = new Client();
            var apiScopeFound = await GetApiScopeAsync(apiResource,apiScope, cancellationToken);
            var model = apiScopeFound.ToModel();
            var apiScopeClaims = await GetApiScopeClaimsAsync(apiResource,apiScopeFound, cancellationToken);
            if (apiScopeClaims != null)
            {
                foreach (var item in apiScopeClaims)
                {
                    model.UserClaims.Add(item.Type);
                }
            }
            var rollup = new ApiScopeRollup()
            {
                ApiScopeJson = JsonConvert.SerializeObject(model),
            };
            var result = await AddRollupAsync(apiResource,apiScopeFound, rollup, cancellationToken);
            return model;
        }
        private async Task<IdentityResult> AddRollupAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            Neo4jIdentityServer4ApiScope apiScope,
            ApiScopeRollup rollup,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            apiScope.ThrowIfNull(nameof(apiScope));
            rollup.ThrowIfNull(nameof(rollup));

            try
            {
                var cypher = $@"
                 MATCH 
                        (:{IdSrv4ClientApiResource}{{Name: $p0}})
                        -[:{Neo4jConstants.Relationships.HasScope}]->
                        (c:{IdSrv4ClientApiScope}{{Name: $p1}})
                MERGE (rollup:{IdSrv4ApiScopeRollup} {"$p2".AsMapFor<Neo4jIdentityServer4ApiScopeRollup>()})
                MERGE (c)-[:{Neo4jConstants.Relationships.HasRollup}]->(rollup)";

                var result = await Session.RunAsync(cypher, Params.Create(apiResource.Name, apiScope.Name, rollup));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }
        public async Task<Scope> GetRollupAsync(
            Neo4jIdentityServer4ApiResource apiResource, 
            Neo4jIdentityServer4ApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            apiScope.ThrowIfNull(nameof(apiScope));

            var cypher = $@"
                MATCH 
                    (:{IdSrv4ClientApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasScope}]->
                    (:{IdSrv4ClientApiScope}{{Name: $p1}})-[:{Neo4jConstants.Relationships.HasRollup}]->
                    (r:{IdSrv4ApiScopeRollup})
                RETURN r{{ .* }}";

            var result = await Session.RunAsync(cypher,
                Params.Create(apiResource.Name,apiScope.Name));

            IdentityServer4.Models.Scope model = null;
            var foundRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<ApiScopeRollup>("r"));
            if (foundRecord == null)
            {
                model = await RollupAsync(apiResource,apiScope, cancellationToken);
            }
            else
            {
                model = JsonConvert.DeserializeObject<Scope>(foundRecord.ApiScopeJson);
            }
            return model;
        }

        public async Task<IdentityResult> DeleteRollupAsync(
            Neo4jIdentityServer4ApiResource apiResource, 
            Neo4jIdentityServer4ApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            apiResource.ThrowIfNull(nameof(apiResource));
            apiScope.ThrowIfNull(nameof(apiScope));

            try
            {
                var cypher = $@"
                MATCH 
                    (:{IdSrv4ClientApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasScope}]->
                    (:{IdSrv4ClientApiScope}{{Name: $p1}})-[:{Neo4jConstants.Relationships.HasRollup}]->(r:{IdSrv4ApiScopeRollup})
             
                DETACH DELETE r";

                await Session.RunAsync(cypher,
                    Params.Create(apiResource.Name, apiScope.Name));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }
    }
}
