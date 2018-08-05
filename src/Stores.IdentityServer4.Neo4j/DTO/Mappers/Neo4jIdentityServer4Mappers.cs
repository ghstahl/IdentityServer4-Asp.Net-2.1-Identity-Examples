using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    public static class Neo4jIdentityServer4Mappers<TProfile>
        where TProfile : Profile
    {
        public static IMapper CreateMapper()
        {
            return new MapperConfiguration(cfg => cfg.AddProfile<Neo4jIdentityServer4IdentityResourceMapperProfile>())
                .CreateMapper();
        }
    }
}