using AutoMapper;

namespace StoresIdentityServer4.Neo4j.Mappers
{
    public static class IdentityResourceMappers
    {
        static IdentityResourceMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<IdentityResourceMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static IdentityServer4.Models.IdentityResource ToModel(this Entities.IdentityResource entity)
        {
            return Mapper.Map<IdentityServer4.Models.IdentityResource>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.IdentityResource ToEntity(this IdentityServer4.Models.IdentityResource model)
        {
            return Mapper.Map<Entities.IdentityResource>(model);
        }
    }
}