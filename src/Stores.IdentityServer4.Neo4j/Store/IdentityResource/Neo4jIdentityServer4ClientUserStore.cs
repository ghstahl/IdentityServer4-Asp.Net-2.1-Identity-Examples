using System;
using System.Collections.Generic;
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
using IdentityResource = IdentityServer4.Models.IdentityResource;

namespace StoresIdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStore<TUser>
        where TUser : Neo4jIdentityUser
    {
 
        private static readonly string IdSrv4IdentityResource;
        private static readonly string IdSrv4IdentityClaim;
        private static readonly string IdSrv4IdentityResourceRollup;
        private static readonly string IdSrv4IdentityResourcesRollup;
        

        private async Task RaiseIdentityResourceChangeEventAsync(Neo4jIdentityServer4IdentityResource IdentityResource)
        {
            await _eventService.RaiseAsync(new IdentityResourceChangeEvent<Neo4jIdentityServer4IdentityResource>(IdentityResource));
        }

        public async Task<List<IdentityResource>> RollupIdentityResourcesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var foundRecords = await GetIdentityResourcesAsync(cancellationToken);
            List<IdentityServer4.Models.IdentityResource> model = new List<IdentityServer4.Models.IdentityResource>();
            foreach (var item in foundRecords)
            {
                var rollup = await GetRollupAsync(item, cancellationToken);
                model.Add(rollup);
            }

            var json = JsonConvert.SerializeObject(model);
            var neo4jObject = new Neo4jIdentityServer4IdentityResourcesRollup()
            {
                Name = "OneAndOnly",
                IdentityResourcesJson = json
            };
            try
            {
                var cypher = $@"CREATE (r:{IdSrv4IdentityResourcesRollup} $p0)";
                await Session.RunAsync(cypher, Params.Create(neo4jObject.ConvertToMap()));
                return model;
            }
            catch (ClientException ex)
            {
                return null;
            }
        }

        public async Task<List<IdentityResource>> GetIdentityResoucesRollupAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();


            var cypher = $@"
                MATCH 
                    (r:{IdSrv4IdentityResourcesRollup}{{Name: 'OneAndOnly'}})
                RETURN r{{ .* }}";

            var result = await Session.RunAsync(cypher);
            List<IdentityServer4.Models.IdentityResource> model = null;
            var foundRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<IdentityResourcesRollup>("r"));

            if (foundRecord == null)
            {
                model = await RollupIdentityResourcesAsync(cancellationToken);
            }
            else
            {
                model = JsonConvert.DeserializeObject<List<IdentityResource>>(foundRecord.IdentityResourcesJson);
            }

            return model;
        }

        public async Task<IdentityResult> DeleteIdentityResoucesRollupAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            try
            {
                var cypher = $@"
                MATCH 
                    (r:{IdSrv4IdentityResourcesRollup}{{Name: 'OneAndOnly'}}) 
                DETACH DELETE r";

                await Session.RunAsync(cypher);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> CreateIdentityResourceAsync(
            Neo4jIdentityServer4IdentityResource identityResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            identityResource.ThrowIfNull(nameof(identityResource));
            try
            {
                var cypher = $@"CREATE (r:{IdSrv4IdentityResource} $p0)";
                await Session.RunAsync(cypher, Params.Create(identityResource.ConvertToMap()));
                await RaiseIdentityResourceChangeEventAsync(identityResource);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> UpdateIdentityResourceAsync(
            Neo4jIdentityServer4IdentityResource identityResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            identityResource.ThrowIfNull(nameof(identityResource));
            try
            {
                var cypher = $@"
                MATCH (r:{IdSrv4IdentityResource}{{Name: $p0}})
                SET r = $p1";

                await Session.RunAsync(cypher, Params.Create(identityResource.Name, identityResource.ConvertToMap()));
                await RaiseIdentityResourceChangeEventAsync(identityResource);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteIdentityResourceAsync(
            Neo4jIdentityServer4IdentityResource identityResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            identityResource.ThrowIfNull(nameof(identityResource));
            await DeleteIdentityClaimsAsync(identityResource, cancellationToken);
            try
            {
                var cypher = $@"
                MATCH (r:{IdSrv4IdentityResource}{{Name: $p0}})
                DETACH DELETE r";

                await Session.RunAsync(cypher, Params.Create(identityResource.Name));
                await RaiseIdentityResourceChangeEventAsync(identityResource);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteIdentityResourcesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var identityResources = await GetIdentityResourcesAsync(cancellationToken);
            foreach (var identityResource in identityResources)
            {
                await DeleteIdentityResourceAsync(identityResource, cancellationToken);
            }

            return IdentityResult.Success;
        }

        public async Task<Neo4jIdentityServer4IdentityResource> GetIdentityResourceAsync(
            Neo4jIdentityServer4IdentityResource identityResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            identityResource.ThrowIfNull(nameof(identityResource));

            var cypher = $@"
                MATCH (r:{IdSrv4IdentityResource}{{Name: $p0}})
                RETURN r {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(identityResource.Name));
            var record =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4IdentityResource>("r"));
            return record;
        }

        public async Task<IList<Neo4jIdentityServer4IdentityResource>> GetIdentityResourcesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var cypher = $@"
                MATCH 
                    (r:{IdSrv4IdentityResource})
                RETURN 
                    r{{ .* }}";

            var result = await Session.RunAsync(cypher);

            var records = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4IdentityResource>("r"));
            return records;
        }

        public async Task<IdentityResult> AddIdentityClaimAsync(Neo4jIdentityServer4IdentityResource identityResource,
            Neo4jIdentityServer4IdentityClaim identityClaim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            identityResource.ThrowIfNull(nameof(identityResource));
            identityClaim.ThrowIfNull(nameof(identityClaim));

            try
            {
                var cypher = $@"
                MATCH 
                    (r:{IdSrv4IdentityResource}{{Name: $p0}}) 
                MERGE 
                    (l:{IdSrv4IdentityClaim} {"$p1".AsMapForNoNull<Neo4jIdentityServer4IdentityClaim>(identityClaim)})
                MERGE 
                    (r)-[:{Neo4jConstants.Relationships.HasClaim}]->(l)";

                var result = await Session.RunAsync(cypher, Params.Create(identityResource.Name, identityClaim));
                await RaiseIdentityResourceChangeEventAsync(identityResource);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<Neo4jIdentityServer4IdentityClaim> GetIdentityClaimAsync(Neo4jIdentityServer4IdentityResource identityResource,
            Neo4jIdentityServer4IdentityClaim identityClaim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            identityResource.ThrowIfNull(nameof(identityResource));
            identityClaim.ThrowIfNull(nameof(identityClaim));

            var cypher = $@"
                MATCH 
                    (r:{IdSrv4IdentityResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasClaim}]->
                    (c:{IdSrv4IdentityClaim}{{Type: $p1}}) 
                RETURN c {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(identityResource.Name, identityClaim.Type));
            var record =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4IdentityClaim>("c"));
            return record;
        }

        public async Task<IdentityResult> DeleteIdentityClaimAsync(Neo4jIdentityServer4IdentityResource identityResource,
            Neo4jIdentityServer4IdentityClaim identityClaim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            identityResource.ThrowIfNull(nameof(identityResource));
            identityClaim.ThrowIfNull(nameof(identityClaim));

            try
            {
                var cypher = $@"
                MATCH 
                    (r:{IdSrv4IdentityResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasClaim}]->
                    (c:{IdSrv4IdentityClaim}{{Type: $p1}}) 
                DETACH DELETE c";

                await Session.RunAsync(cypher,
                    Params.Create(
                        identityResource.Name,
                        identityClaim.Type));
                await RaiseIdentityResourceChangeEventAsync(identityResource);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IList<Neo4jIdentityServer4IdentityClaim>> GetIdentityClaimsAsync(Neo4jIdentityServer4IdentityResource identityResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            identityResource.ThrowIfNull(nameof(identityResource));

            var cypher = $@"
                MATCH 
                    (:{IdSrv4IdentityResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasClaim}]->
                    (s:{IdSrv4IdentityClaim})
                RETURN 
                    s{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(identityResource.Name));

            var records = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4IdentityClaim>("s"));
            return records;
        }

        public async Task<IdentityResult> DeleteIdentityClaimsAsync(Neo4jIdentityServer4IdentityResource identityResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            identityResource.ThrowIfNull(nameof(identityResource));

            try
            {
                var cypher = $@"
                MATCH 
                   (:{IdSrv4IdentityResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasClaim}]->
                    (s:{IdSrv4IdentityClaim})
                DETACH DELETE s";

                await Session.RunAsync(cypher,
                    Params.Create(identityResource.Name));
                await RaiseIdentityResourceChangeEventAsync(identityResource);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityServer4.Models.IdentityResource> RollupAsync(Neo4jIdentityServer4IdentityResource identityResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            identityResource.ThrowIfNull(nameof(identityResource));
            var foundIdentityResource = await GetIdentityResourceAsync(identityResource, cancellationToken);
            IdentityResource model = null;
            if (foundIdentityResource != null)
            {
                model = foundIdentityResource.ToModel();
                var claims = await GetIdentityClaimsAsync(foundIdentityResource, cancellationToken);
                foreach (var claim in claims)
                {
                    model.UserClaims.Add(claim.Type);
                }

                var rollup = new IdentityResourceRollup()
                {
                    IdentityResourceJson = JsonConvert.SerializeObject(model),
                };
                await AddRollupAsync(foundIdentityResource, rollup);
            }


            return model;
        }
        private async Task<IdentityResult> AddRollupAsync(
            Neo4jIdentityServer4IdentityResource identityResource,
            IdentityResourceRollup rollup,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            identityResource.ThrowIfNull(nameof(identityResource));
            rollup.ThrowIfNull(nameof(rollup));

            try
            {
                var cypher = $@"
                    MATCH (c:{IdSrv4IdentityResource}{{Name: $p0}})        
                    MERGE (rollup:{IdSrv4IdentityResourceRollup} {"$p1".AsMapForNoNull(rollup)})
                    MERGE (c)-[:{Neo4jConstants.Relationships.HasRollup}]->(rollup)";

                var result = await Session.RunAsync(cypher, Params.Create(identityResource.Name, rollup));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResource> GetRollupAsync(Neo4jIdentityServer4IdentityResource identityResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            identityResource.ThrowIfNull(nameof(identityResource));

            var cypher = $@"
                MATCH 
                    (:{IdSrv4IdentityResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasRollup}]->
                    (r:{IdSrv4IdentityResourceRollup})
                RETURN r{{ .* }}";

            var result = await Session.RunAsync(cypher,
                Params.Create(identityResource.Name));

            IdentityServer4.Models.IdentityResource model = null;
            var foundRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<IdentityResourceRollup>("r"));
            if (foundRecord == null)
            {
                model = await RollupAsync(identityResource, cancellationToken);
            }
            else
            {
                model = JsonConvert.DeserializeObject<IdentityResource>(foundRecord.IdentityResourceJson);
            }

            return model;
        }

        public async Task<IdentityResult> DeleteRollupAsync(Neo4jIdentityServer4IdentityResource identityResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            identityResource.ThrowIfNull(nameof(identityResource));

            try
            {
                var cypher = $@"
                MATCH 
                    (:{IdSrv4IdentityResource}{{Name: $p0}})-[:{Neo4jConstants.Relationships.HasRollup}]->
                    (r:{IdSrv4IdentityResourceRollup}) 
                DETACH DELETE r";

                await Session.RunAsync(cypher,
                    Params.Create(identityResource.Name));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }
    }
}
