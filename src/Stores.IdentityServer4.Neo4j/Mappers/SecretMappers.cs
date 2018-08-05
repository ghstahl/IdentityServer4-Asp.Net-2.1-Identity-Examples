using AutoMapper;

namespace StoresIdentityServer4.Neo4j.Mappers
{
    /// Extension methods to map to/from entity/model for clients.
    /// </summary>
    public static class SecretMappers
    {
        static SecretMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<SecretMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static IdentityServer4.Models.Secret ToModel(this Entities.Secret entity)
        {
            return Mapper.Map<IdentityServer4.Models.Secret>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.Secret ToEntity(this IdentityServer4.Models.Secret model)
        {
            return Mapper.Map<Entities.Secret>(model);
        }
    }
}