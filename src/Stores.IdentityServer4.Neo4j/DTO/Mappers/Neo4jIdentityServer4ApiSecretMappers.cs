using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    public static class Neo4jIdentityServer4ApiSecretMappers
    {
        static Neo4jIdentityServer4ApiSecretMappers()
        {
            Mapper = Neo4jIdentityServer4Mappers<Neo4JIdentityServer4ApiSecretMapperProfile>.CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static IdentityServer4.Models.Secret ToModel(this Neo4jIdentityServer4ApiSecret entity)
        {
            return Mapper.Map<IdentityServer4.Models.Secret>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Neo4jIdentityServer4ApiSecret ToNeo4jEntity(
            this IdentityServer4.Models.Secret model)
        {
            return Mapper.Map<Neo4jIdentityServer4ApiSecret>(model);
        }
        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Neo4jIdentityServer4ApiSecret ToNeo4jEntity(
            this StoresIdentityServer4.Neo4j.Entities.ApiSecret entity)
        {
            return Mapper.Map<Neo4jIdentityServer4ApiSecret>(entity);
        }
    }
}