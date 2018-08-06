using System.Security.Claims;
using AutoMapper;

namespace StoresIdentityServer4.Neo4j.Mappers
{
    /// Extension methods to map to/from entity/model for clients.
    /// </summary>
    public static class ClientClaimMappers
    {
        static ClientClaimMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientClaimMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Claim ToModel(this Entities.ClientClaim entity)
        {
            return Mapper.Map<Claim>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.ClientClaim ToEntity(this Claim model)
        {
            return Mapper.Map<Entities.ClientClaim>(model);
        }
    }
}