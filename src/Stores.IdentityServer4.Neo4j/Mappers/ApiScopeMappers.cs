using AutoMapper;

namespace StoresIdentityServer4.Neo4j.Mappers
{
    /// Extension methods to map to/from entity/model for clients.
    /// </summary>
    public static class ApiScopeMappers
    {
        static ApiScopeMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiScopeMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static IdentityServer4.Models.Scope ToModel(this Entities.ApiScope entity)
        {
            return Mapper.Map<IdentityServer4.Models.Scope>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.ApiScope ToEntity(this IdentityServer4.Models.Scope model)
        {
            return Mapper.Map<Entities.ApiScope>(model);
        }
    }
}