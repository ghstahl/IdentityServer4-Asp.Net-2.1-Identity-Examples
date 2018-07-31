using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Stores.IdentityServer4.Neo4j.Entities;

namespace Stores.IdentityServer4.Neo4j
{
    public interface IIdentityServer4ClientUserStore2<
        TUser,
        TClient,
        TGrantType> :
        IIdentityServer4ClientStore2<TClient, TGrantType>
        where TUser : class
        where TClient : ClientRoot
        where TGrantType : ClientGrantType
    {
        Task CreateConstraintsAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<IdentityResult> AddClientToUserAsync(TUser user, TClient client,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<TClient>> GetClientsAsync(TUser user,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}