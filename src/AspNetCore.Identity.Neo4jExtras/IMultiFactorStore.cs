using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Neo4j
{
    public interface IMultiFactorStore<TFactor>: 
        IDisposable
    {

        Task<IdentityResult> CreateAsync(TFactor factor, CancellationToken cancellationToken);

        Task<IdentityResult> DeleteAsync(TFactor factor, CancellationToken cancellationToken);

        Task<TFactor> FindByIdAsync(string factorId, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateAsync(TFactor factor, CancellationToken cancellationToken);
    }
}
