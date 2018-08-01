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
        public Task<IdentityResult> CreateApiResourceAsync(Neo4jIdentityServer4ApiResource apiResource, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteApiResourceAsync(Neo4jIdentityServer4ApiResource apiResource, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteApiResourcesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Neo4jIdentityServer4ApiResource> FindApiResourceAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateApiResourceAsync(Neo4jIdentityServer4ApiResource apiResource, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }


    }
}