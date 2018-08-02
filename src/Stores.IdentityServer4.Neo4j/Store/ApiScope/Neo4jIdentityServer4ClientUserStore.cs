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
 
        private static readonly string IdSrv4ClientApiScope;
        private static readonly string IdSrv4ClientApiScopeClaim;

        public Task<IdentityResult> CreateApiScopeAsync(
            Neo4jIdentityServer4ApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateApiScopeAsync(
            Neo4jIdentityServer4ApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteApiScopeAsync(
            Neo4jIdentityServer4ApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteApiScopesAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<Neo4jIdentityServer4ApiScope> FindApiScopeAsync(
            Neo4jIdentityServer4ApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> AddApiScopeClaimToApiScopeAsync(
            Neo4jIdentityServer4ApiScope apiScope,
            Neo4jIdentityServer4ApiScopeClaim apiScopeClaim, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> RemoveApiScopeClaimFromApiScopeAsync(
            Neo4jIdentityServer4ApiScope apiScope,
            Neo4jIdentityServer4ApiScopeClaim apiScopeClaim, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IList<Neo4jIdentityServer4ApiScopeClaim>> GetApiScopeClaimsAsync(
            Neo4jIdentityServer4ApiScope apiScope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
