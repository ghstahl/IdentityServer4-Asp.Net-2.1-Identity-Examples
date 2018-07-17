using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using AspNetCore.Identity.Neo4j.Internal;
using AspNetCore.Identity.Neo4j.Internal.Extensions;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;

namespace AspNetCore.Identity.Neo4jExtras
{

    public class Neo4jMultiFactorStore<TUser, TFactor> :
        IMultiFactorUserStore<TUser, TFactor>,
        IMultiFactorTest<TFactor>
        where TUser : Neo4jIdentityUser
        where TFactor : ChallengeFactor
    {
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

        public async Task DropDatabaseAsync()
        {
            //MATCH (client)-[r]->() DELETE r;
            var cypher = @"MATCH (client)-[r]->() DELETE r";
            await Session.RunAsync(cypher);
            cypher = @"MATCH (n) DELETE n";
            await Session.RunAsync(cypher);

        }

        public IdentityErrorDescriber ErrorDescriber { get; set; }

        public ISession Session { get; set; }

        private bool _disposed;

        public static string User { get; set; }
        public static string Factor { get; set; }

        static Neo4jMultiFactorStore()
        {
            User = typeof(TUser).GetNeo4jLabelName();
            Factor = typeof(TFactor).GetNeo4jLabelName();
        }



        public Neo4jMultiFactorStore(ISession session, IdentityErrorDescriber describer)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
            ErrorDescriber = describer ?? new IdentityErrorDescriber();
        }

        public async Task<IdentityResult> CreateAsync(TFactor factor,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            factor.ThrowIfNull(nameof(factor));

            var cypher = $@"CREATE (r:{Factor} $p0)";
            await Session.RunAsync(cypher, Params.Create(factor.ConvertToMap()));
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(TFactor factor,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            factor.ThrowIfNull(nameof(factor));

            var cypher = $@"
                MATCH (r:{Factor})
                WHERE r.FactorId = $p0
                SET r = $p1";

            await Session.RunAsync(cypher, Params.Create(factor.FactorId, factor.ConvertToMap()));
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TFactor factor,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            factor.ThrowIfNull(nameof(factor));

            var cypher = $@"
                MATCH (r:{Factor})
                WHERE r.FactorId = $p0
                DETACH DELETE r";

            await Session.RunAsync(cypher, Params.Create(factor.FactorId));
            return IdentityResult.Success;
        }

        public async Task<TFactor> FindByIdAsync(string factorId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            factorId.ThrowIfNull(nameof(factorId));

            var cypher = $@"
                MATCH (r:{Factor})
                WHERE r.FactorId = $p0
                RETURN r {{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(factorId));
            var factor = await result.SingleOrDefaultAsync(r => r.MapTo<TFactor>("r"));
            return factor;
        }

        public async Task<IdentityResult> AddToFactorAsync(TUser user, TFactor factor,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            factor.ThrowIfNull(nameof(factor));

            var cypher = $@"
                MATCH (u:{User} {{Id: $p0}})
                MERGE (l:{Factor} {"$p1".AsMapFor<TFactor>()})
                MERGE (u)-[:{Neo4jConstants.Relationships.HasFactor}]->(l)";

            var result = await Session.RunAsync(cypher, Params.Create(user.Id, factor));
            return IdentityResult.Success;
        }

        public async Task<IList<TFactor>> GetFactorsAsync(TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));


            var cypher = $@"
                MATCH (u:{User})-[:{Neo4jConstants.Relationships.HasFactor}]->(r:{Factor})
                WHERE u.Id = $p0
                RETURN r{{ .* }}";

            var result = await Session.RunAsync(cypher, Params.Create(user.Id));
      
            var factors = await result.ToListAsync(r => r.MapTo<TFactor>("r"));
            return factors;
        }
    }
}
