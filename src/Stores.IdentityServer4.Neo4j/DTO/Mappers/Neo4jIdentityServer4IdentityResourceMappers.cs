using AutoMapper;
using StoresIdentityServer4.Neo4j.Mappers;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    public static class Neo4jIdentityServer4IdentityResourceMappers 
    {
        static Neo4jIdentityServer4IdentityResourceMappers()
        {
            Mapper = Neo4jIdentityServer4Mappers<Neo4jIdentityServer4IdentityResourceMapperProfile>.CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static IdentityServer4.Models.IdentityResource ToModel(this Neo4jIdentityServer4IdentityResource entity)
        {
            return Mapper.Map<IdentityServer4.Models.IdentityResource>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Neo4jIdentityServer4IdentityResource ToNeo4jEntity(
            this IdentityServer4.Models.IdentityResource model)
        {
            return Mapper.Map<Neo4jIdentityServer4IdentityResource>(model);
        }
        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Neo4jIdentityServer4IdentityResource ToNeo4jEntity(
            this StoresIdentityServer4.Neo4j.Entities.IdentityResource entity)
        {
            return Mapper.Map<Neo4jIdentityServer4IdentityResource>(entity);
        }
    }
}
   