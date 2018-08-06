using System.Security.Claims;
using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    public static class Neo4jIdentityServer4ClientClaimMappers
    {
        static Neo4jIdentityServer4ClientClaimMappers()
        {
            Mapper = new MapperConfiguration(
                    cfg => cfg.AddProfile<
                        Neo4JIdentityServer4ClientClaimMapperProfile
                    >())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Claim ToModel(
            this Neo4jIdentityServer4ClientClaim entity)
        {
            return Mapper.Map<Claim>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Neo4jIdentityServer4ClientClaim ToNeo4jEntity(
            this Claim model)
        {
            return Mapper.Map<Neo4jIdentityServer4ClientClaim>(model);
        }
        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Neo4jIdentityServer4ClientClaim ToNeo4jEntity(
            this StoresIdentityServer4.Neo4j.Entities.ClientClaim entity)
        {
            return Mapper.Map<Neo4jIdentityServer4ClientClaim>(entity);
        }
    }
}