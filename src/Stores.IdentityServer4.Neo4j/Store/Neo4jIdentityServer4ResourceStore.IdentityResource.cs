using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras.Extensions;

namespace Stores.IdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ResourceStore<TUser>
    {        
       
        public Task<IdentityResult> CreateIdentityResourceAsync(Neo4jIdentityServer4IdentityResource identityResource, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteIdentityResourceAsync(Neo4jIdentityServer4IdentityResource identityResource, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteIdentityResourcesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<Neo4jIdentityServer4IdentityResource> FindIdentityResourceAsync(string identityResource, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateIdentityResourceAsync(Neo4jIdentityServer4IdentityResource identityResource, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

    }
}