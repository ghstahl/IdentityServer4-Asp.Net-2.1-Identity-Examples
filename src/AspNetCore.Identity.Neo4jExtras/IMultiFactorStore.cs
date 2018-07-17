using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Neo4j
{
    public interface IMultiFactorUserStore<TUser, TFactor>: 
        IMultiFactorStore<TFactor>,
        IDisposable 
        where TUser : class 
        where TFactor : class
    {
        Task<IdentityResult> AddToFactorAsync(TUser user, TFactor factor, CancellationToken cancellationToken);

        Task<IList<TFactor>> GetFactorsAsync(TUser user, CancellationToken cancellationToken);
    }
    public interface IMultiFactorTest<TFactor>
        where TFactor : class
    {
        Task DropDatabaseAsync();
    }
    public interface IMultiFactorStore<TFactor>: 
        IDisposable where TFactor:class
    {

        Task<IdentityResult> CreateAsync(TFactor factor, CancellationToken cancellationToken);

        Task<IdentityResult> DeleteAsync(TFactor factor, CancellationToken cancellationToken);

        Task<TFactor> FindByIdAsync(string factorId, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateAsync(TFactor factor, CancellationToken cancellationToken);
    }
}
