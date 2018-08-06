using System.Collections.Generic;
using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    public static class Neo4jIdentityServer4PropertyMappers
    {
        static Neo4jIdentityServer4PropertyMappers()
        {
            Mapper = new MapperConfiguration(
                    cfg => cfg.AddProfile<
                        Neo4jIdentityServer4PropertyMapperProfile
                    >())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static KeyValuePair<string, string> ToModel(this Neo4jIdentityServer4ClientProperty entity)
        {
            return Mapper.Map<KeyValuePair<string, string>>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Neo4jIdentityServer4ClientProperty ToNeo4jClientPropertyEntity(this KeyValuePair<string, string> model)
        {
            return Mapper.Map<Neo4jIdentityServer4ClientProperty>(model);
        }
    }
}