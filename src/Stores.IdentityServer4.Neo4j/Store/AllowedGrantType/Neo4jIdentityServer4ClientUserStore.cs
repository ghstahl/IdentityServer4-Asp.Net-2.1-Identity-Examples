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

namespace StoresIdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStore<TUser> where TUser : Neo4jIdentityUser
    {
        private static readonly string IdSrv4GrantType;
        private static readonly string IdSrv4AllowedGrantType;

        

        public async Task<IdentityResult> AddAllowedGrantTypeToClientAsync(
            Neo4jIdentityServer4Client client,
            Neo4JIdentityServer4GrantType identityServer4GrantType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            identityServer4GrantType.ThrowIfNull(nameof(identityServer4GrantType));
            try
            {
                var cypher = $@"
                MATCH (client:{IdSrv4Client} {{ClientId: $p0}})
                CREATE UNIQUE(
                    (client)-[:{Neo4jConstants.Relationships.HasGrantType}]->
                    (:{IdSrv4AllowedGrantType} {"$p1".AsMapForNoNull(identityServer4GrantType)}))";


                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, identityServer4GrantType));
                await RaiseClientChangeEventAsync(client);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteAllowedGrantTypeToClientAsync(Neo4jIdentityServer4Client client,
            Neo4JIdentityServer4GrantType identityServer4GrantType, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            identityServer4GrantType.ThrowIfNull(nameof(identityServer4GrantType));
            try
            {
                var cypher = $@"
                MATCH 
                    (client:{IdSrv4Client})
                    -[:{Neo4jConstants.Relationships.HasGrantType}]->
                    (allowedGT:{IdSrv4AllowedGrantType})
                WHERE client.ClientId = $p0 AND allowedGT.IdentityServer4GrantType = $p1 
                DETACH DELETE allowedGT";
 
                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, identityServer4GrantType.GrantType));
                await RaiseClientChangeEventAsync(client);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> CreateGrantTypeAsync(
            Neo4JIdentityServer4GrantType identityServer4GrantType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            identityServer4GrantType.ThrowIfNull(nameof(identityServer4GrantType));
            try
            {
                var cypher = $@"CREATE (r:{IdSrv4GrantType} $p0)";
                await Session.RunAsync(cypher, Params.Create(identityServer4GrantType.ConvertToMap()));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> UpdateGrantTypeAsync(
            Neo4JIdentityServer4GrantType identityServer4GrantType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            identityServer4GrantType.ThrowIfNull(nameof(identityServer4GrantType));
            try
            {
                var cypher = $@"
                MATCH (r:{IdSrv4GrantType})
                WHERE r.IdentityServer4GrantType = $p0
                SET r = $p1";

                await Session.RunAsync(cypher, Params.Create(identityServer4GrantType.GrantType, identityServer4GrantType.ConvertToMap()));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteGrantTypeAsync(
            Neo4JIdentityServer4GrantType identityServer4GrantType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            identityServer4GrantType.ThrowIfNull(nameof(identityServer4GrantType));
            try
            {
                var cypher = $@"
                MATCH (r:{IdSrv4GrantType})
                WHERE r.GrantType = $p0
                DETACH DELETE r";

                await Session.RunAsync(cypher, Params.Create(identityServer4GrantType.GrantType));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteGrantTypesAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            try
            {
                var cypher = $@"
                MATCH (r:{IdSrv4GrantType})
                DETACH DELETE r";

                await Session.RunAsync(cypher);
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<Neo4JIdentityServer4GrantType> FindGrantTypeAsync(
            Neo4JIdentityServer4GrantType identityServer4GrantType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            identityServer4GrantType.ThrowIfNull(nameof(identityServer4GrantType));

            var cypher = $@"
                MATCH (r:{IdSrv4GrantType})
                WHERE r.GrantType = $p0
                RETURN r {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(identityServer4GrantType.GrantType));
            var grantTypeRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4JIdentityServer4GrantType>("r"));
            return grantTypeRecord;
        }

        public async Task<IList<Neo4JIdentityServer4GrantType>> GetAllowedGrantTypesAsync(
            Neo4jIdentityServer4Client client, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                MATCH (c:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasGrantType}]->(g:{IdSrv4AllowedGrantType})
                WHERE c.ClientId = $p0
                RETURN g{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var grantTypes = await result.ToListAsync(r => r.MapTo<Neo4JIdentityServer4GrantType>("g"));
            return grantTypes;

        }
    }
}
