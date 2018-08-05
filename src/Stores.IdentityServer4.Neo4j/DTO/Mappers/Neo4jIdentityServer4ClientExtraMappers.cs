using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    public static class Neo4jIdentityServer4ClientExtraMappers
    {
        static Neo4jIdentityServer4ClientExtraMappers()
        {
            Mapper = Neo4jIdentityServer4Mappers<Neo4jIdentityServer4ClientExtraMapperProfile>.CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static IdentityServer4.Models.ClientExtra ToModel(this Neo4jIdentityServer4Client entity)
        {
            return Mapper.Map<IdentityServer4.Models.ClientExtra>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Neo4jIdentityServer4Client ToNeo4jEntity(
            this IdentityServer4.Models.ClientExtra model)
        {
            return Mapper.Map<Neo4jIdentityServer4Client>(model);
        }
        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Neo4jIdentityServer4Client ToNeo4jEntity(
            this StoresIdentityServer4.Neo4j.Entities.ClientExtra entity)
        {
            return Mapper.Map<Neo4jIdentityServer4Client>(entity);
        }
    }
}