using System.Threading;
using System.Threading.Tasks;

namespace StoresIdentityServer4.Neo4j
{
    public interface INeo4jIdentityServer4Database
    {
        Task CreateConstraintsAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}