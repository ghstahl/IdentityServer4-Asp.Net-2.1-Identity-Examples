using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    public static class Neo4JIdentityServer4ClientGrantTypeMappers
    {
        static Neo4JIdentityServer4ClientGrantTypeMappers()
        {
            Mapper = new MapperConfiguration(
                    cfg => cfg.AddProfile<
                        Neo4JIdentityServer4ClientGrantTypeMapperProfile
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
            this Neo4JIdentityServer4GrantType entity)
        {
            return Mapper.Map<string>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Neo4JIdentityServer4GrantType ToNeo4jClientAllowedGrantTypeEntity(
            this string model)
        {
            return Mapper.Map<Neo4JIdentityServer4GrantType>(model);
        }

    }
}