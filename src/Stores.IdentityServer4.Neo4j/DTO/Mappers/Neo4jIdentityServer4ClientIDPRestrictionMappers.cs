using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    public static class Neo4jIdentityServer4ClientIDPRestrictionMappers
    {
        static Neo4jIdentityServer4ClientIDPRestrictionMappers()
        {
            Mapper = new MapperConfiguration(
                    cfg => cfg.AddProfile<
                        Neo4JIdentityServer4ClientTIDPRestrictionMapperProfile
                    >())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static string ToModel(
            this Neo4jIdentityServer4ClienTIDPRestriction entity)
        {
            return Mapper.Map<string>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Neo4jIdentityServer4ClienTIDPRestriction ToNeo4jClientIDPRestrictionEntity(
            this string model)
        {
            return Mapper.Map<Neo4jIdentityServer4ClienTIDPRestriction>(model);
        }
         
    }
}