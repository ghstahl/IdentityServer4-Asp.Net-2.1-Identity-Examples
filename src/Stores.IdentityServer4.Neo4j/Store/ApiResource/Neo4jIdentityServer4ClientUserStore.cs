﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras;
using Neo4jExtras.Extensions;
using Stores.IdentityServer4Neo4j.Events;

namespace StoresIdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStore<TUser>
        where TUser : Neo4jIdentityUser
    {
        private static readonly string IdSrv4ClientApiResource;
        private static readonly string IdSrv4ClientApiResourceClaim;
        private static readonly string IdSrv4ClientApiSecret;

        private Task RaiseApiResourceChangeEventAsync(Neo4jIdentityServer4ApiResource apiResource)
        {
            return _eventService.RaiseAsync(new ApiResourceChangeEvent<Neo4jIdentityServer4ApiResource>(apiResource));
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
                var cypher = $@"CREATE (r:{IdSrv4ClientApiResource} $p0)";
                await Session.RunAsync(cypher, Params.Create(apiResource.ConvertToMap()));
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
                MATCH (r:{IdSrv4ClientApiResource}{{Name: $p0}})
                SET r = $p1";

                await Session.RunAsync(cypher, Params.Create(apiResource.Name, apiResource.ConvertToMap()));
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
                MATCH (r:{IdSrv4ClientApiResource}{{Name: $p0}})
                DETACH DELETE r";

                await Session.RunAsync(cypher, Params.Create(apiResource.Name));
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
                MATCH (r:{IdSrv4ClientApiResource}{{Name: $p0}})
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
                    (r:{IdSrv4ClientApiResource})
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
                    (r:{IdSrv4ClientApiResource}{{Name: $p0}}) 
                MERGE 
                    (l:{IdSrv4ClientApiResourceClaim} {"$p1".AsMapFor<Neo4jIdentityServer4ApiResourceClaim>()})
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
                    (r:{IdSrv4ClientApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasClaim}]->
                    (c:{IdSrv4ClientApiResourceClaim}{{Type: $p1}}) 
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
                    (r:{IdSrv4ClientApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasClaim}]->
                    (c:{IdSrv4ClientApiResourceClaim}{{Type: $p1}}) 
                DETACH DELETE c";

                await Session.RunAsync(cypher,
                    Params.Create(
                        apiResource.Name,
                        apiResourceClaim.Type ));
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
                    (:{IdSrv4ClientApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasClaim}]->
                    (s:{IdSrv4ClientApiResourceClaim})
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
                   (:{IdSrv4ClientApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasClaim}]->
                    (s:{IdSrv4ClientApiResourceClaim})
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
                    (r:{IdSrv4ClientApiResource}{{Name: $p0}}) 
                MERGE 
                    (s:{IdSrv4ClientApiSecret} {"$p1".AsMapFor<Neo4jIdentityServer4ApiSecret>()})
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
                    (:{IdSrv4ClientApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasSecret}]->
                    (s:{IdSrv4ClientApiSecret}{{Type: $p1,Value: $p2}}) 
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
                    (r:{IdSrv4ClientApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasSecret}]->
                    (s:{IdSrv4ClientApiSecret}{{Type: $p1,Value: $p2}}) 
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
                    (:{IdSrv4ClientApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasSecret}]->
                    (s:{IdSrv4ClientApiSecret}) 
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
                    (:{IdSrv4ClientApiResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasSecret}]->
                    (s:{IdSrv4ClientApiSecret})
                RETURN 
                    s{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(apiResource.Name));

            var records = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ApiSecret>("s"));
            return records;
        }

        public async Task<ApiResource> RollupAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResource> GetRollupAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> DeleteRollupAsync(
            Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
