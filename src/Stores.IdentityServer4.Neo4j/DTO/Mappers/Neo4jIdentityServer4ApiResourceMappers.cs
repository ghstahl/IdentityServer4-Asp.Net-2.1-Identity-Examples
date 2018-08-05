using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    public static class Neo4jIdentityServer4ApiResourceMappers
    {
        static Neo4jIdentityServer4ApiResourceMappers()
        {
            Mapper = Neo4jIdentityServer4Mappers<Neo4jIdentityServer4ApiResourceMapperProfile>.CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static IdentityServer4.Models.ApiResource ToModel(this Neo4jIdentityServer4ApiResource entity)
        {
            return Mapper.Map<IdentityServer4.Models.ApiResource>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Neo4jIdentityServer4ApiResource ToNeo4jEntity(
            this IdentityServer4.Models.ApiResource model)
        {
            return Mapper.Map<Neo4jIdentityServer4ApiResource>(model);
        }
        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Neo4jIdentityServer4ApiResource ToNeo4jEntity(
            this StoresIdentityServer4.Neo4j.Entities.ApiResource entity)
        {
            return Mapper.Map<Neo4jIdentityServer4ApiResource>(entity);
        }
    }
}