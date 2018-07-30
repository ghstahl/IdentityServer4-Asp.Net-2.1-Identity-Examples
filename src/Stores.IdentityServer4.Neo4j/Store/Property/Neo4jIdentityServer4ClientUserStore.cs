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

namespace Stores.IdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStore<TUser> where TUser : Neo4jIdentityUser
    {
        private static readonly string IdSrv4ClientProperty;
        public async Task<IdentityResult> AddPropertyToClientAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientProperty property,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            property.ThrowIfNull(nameof(property));
            try
            {
                var cypher = $@"
                MATCH (client:{IdSrv4Client} {{ClientId: $p0}})
                MERGE (property:{IdSrv4ClientProperty} {"$p1".AsMapFor<Neo4jIdentityServer4ClientProperty>()})
                MERGE (client)-[:{Neo4jConstants.Relationships.HasProperty}]->(property)";

                var result = await Session.RunAsync(cypher, Params.Create(client.ClientId, property));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }


        public async Task<IdentityResult> DeletePropertyAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientProperty property,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            property.ThrowIfNull(nameof(property));
            try
            {
                var cypher = $@"
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasProperty}]->(property:{
                        IdSrv4ClientProperty
                    })
                WHERE client.ClientId = $p0 AND property.Key = $p1 AND property.Value = $p2 
                DETACH DELETE property";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId,
                        property.Key,
                        property.Value
                    ));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> DeletePropertiesAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            try
            {
                var cypher = $@"
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasProperty}]->(property:{
                        IdSrv4ClientProperty
                    })
                WHERE client.ClientId = $p0  
                DETACH DELETE property";

                await Session.RunAsync(cypher,
                    Params.Create(
                        client.ClientId
                    ));
                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<Neo4jIdentityServer4ClientProperty> FindPropertyAsync(Neo4jIdentityServer4Client client,
            Neo4jIdentityServer4ClientProperty property,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));
            property.ThrowIfNull(nameof(property));
            var cypher = $@"
                MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasProperty}]->(property:{
                    IdSrv4ClientProperty
                })
                WHERE client.ClientId = $p0 AND property.Key = $p1 AND property.Value = $p2  
                RETURN property{{ .* }}";

            var result = await Session.RunAsync(cypher,
                Params.Create(
                    client.ClientId,
                    property.Key,
                    property.Value

                ));

            var foundRecord =
                await result.SingleOrDefaultAsync(r => r.MapTo<Neo4jIdentityServer4ClientProperty>("property"));

            return foundRecord;
        }

        public async Task<IList<Neo4jIdentityServer4ClientProperty>> GetPropertiesAsync(
            Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            client.ThrowIfNull(nameof(client));

            var cypher = $@"
                 MATCH (client:{IdSrv4Client})-[:{Neo4jConstants.Relationships.HasProperty}]->(property:{
                    IdSrv4ClientProperty
                })
                WHERE client.ClientId = $p0
                RETURN property{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(client.ClientId));

            var records = await result.ToListAsync(r => r.MapTo<Neo4jIdentityServer4ClientProperty>("property"));
            return records;
        }
    }
}
