using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    public static class Neo4JIdentityServer4ClientRedirectUriUriMappers
    {
        static Neo4JIdentityServer4ClientRedirectUriUriMappers()
        {
            Mapper = new MapperConfiguration(
                    cfg => cfg.AddProfile<
                        Neo4JIdentityServer4ClientRedirectUriMapperProfile
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
            this Neo4jIdentityServer4ClientRedirectUri entity)
        {
            return Mapper.Map<string>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Neo4jIdentityServer4ClientRedirectUri ToNeo4jClientRedirectUriEntity(
            this string model)
        {
            return Mapper.Map<Neo4jIdentityServer4ClientRedirectUri>(model);
        }

    }
}