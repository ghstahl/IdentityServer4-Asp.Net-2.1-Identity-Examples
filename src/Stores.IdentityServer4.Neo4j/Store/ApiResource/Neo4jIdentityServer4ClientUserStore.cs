using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using Microsoft.AspNetCore.Identity;
using Neo4jExtras.Extensions;
using Stores.IdentityServer4Neo4j.Events;

namespace StoresIdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStore<TUser> 
        where TUser : Neo4jIdentityUser
    {
        private static readonly string IdSrv4ClientApiResource;
        private static readonly string IdSrv4ClientApiResourceClaim;
        private static readonly string IdSrv4ClientApiScope;
        private static readonly string IdSrv4ClientApiScopeClaim;
        private static readonly string IdSrv4ClientApiSecret;

        public async Task<IdentityResult> CreateApiResourceAsync(Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> UpdateApiResourceAsync(Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> DeleteApiResourceAsync(Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> DeleteApiResourcesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<Neo4jIdentityServer4ApiResource> FindApiResourceAsync(Neo4jIdentityServer4ApiResource apiResource,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
