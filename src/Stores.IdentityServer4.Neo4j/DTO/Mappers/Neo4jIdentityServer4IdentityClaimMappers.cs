using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    public static class Neo4jIdentityServer4IdentityClaimMappers
    {
        static Neo4jIdentityServer4IdentityClaimMappers()
        {
            Mapper = new MapperConfiguration(
                    cfg => cfg.AddProfile<
                        Neo4jIdentityServer4IdentityClaimMapperProfile
                    >())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static string ToModel(this Neo4jIdentityServer4IdentityClaim entity)
        {
            return Mapper.Map<string>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Neo4jIdentityServer4IdentityClaim ToNeo4jIdentityClaimEntity(this string model)
        {
            return Mapper.Map<Neo4jIdentityServer4IdentityClaim>(model);
        }
    }
}