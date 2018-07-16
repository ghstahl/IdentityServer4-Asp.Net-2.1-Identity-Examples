using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using AspNetCore.Identity.Neo4j.Internal;
using AspNetCore.Identity.Neo4j.Internal.Extensions;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;

namespace AspNetCore.Identity.Neo4jExtras
{
    public class Neo4jMultiFactorStore<TFactor>: 
        IMultiFactorStore<TFactor>
        where TFactor : ChallengeFactor
    {
        public IdentityErrorDescriber ErrorDescriber { get; set; }

        public ISession Session { get; set; }

        private bool _disposed;

        public static string Factor { get; set; }
        static Neo4jMultiFactorStore()
        {
            Factor = typeof(TFactor).GetNeo4jLabelName();
        }

        public Neo4jMultiFactorStore(ISession session, IdentityErrorDescriber describer)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
            ErrorDescriber = describer ?? new IdentityErrorDescriber();
        }

        public async Task<IdentityResult> CreateAsync(TFactor factor, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (factor == null)
            {
                throw new ArgumentNullException(nameof(factor));
            }

            var cypher = $@"CREATE (r:{Factor} $p0)";
            await Session.RunAsync(cypher, Params.Create(factor.ConvertToMap()));
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> UpdateAsync(TFactor factor, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (factor == null)
            {
                throw new ArgumentNullException(nameof(factor));
            }

            var cypher = $@"
                MATCH (r:{Factor})
                WHERE r.Id = $p0
                SET r = $p1";

            await Session.RunAsync(cypher, Params.Create(factor.Id, factor.ConvertToMap()));
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> DeleteAsync(TFactor factor, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (factor == null)
            {
                throw new ArgumentNullException(nameof(factor));
            }

            var cypher = $@"
                MATCH (r:{Factor})
                WHERE r.Id = $p0
                DETACH DELETE r";

            await Session.RunAsync(cypher, Params.Create(factor.Id));
            return IdentityResult.Success;
        }

        public async Task<TFactor> FindByIdAsync(string factorId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (factorId == null)
            {
                throw new ArgumentNullException(nameof(factorId));
            }

            var cypher = $@"
                MATCH (r:{Factor})
                WHERE r.Id = $p0
                RETURN r {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(factorId));
            var factor = await result.SingleOrDefaultAsync(r => r.MapTo<TFactor>("r"));
            return factor;
        }

        

        /// <summary>
        /// Dispose the stores
        /// </summary>
        public void Dispose() => _disposed = true;

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}
