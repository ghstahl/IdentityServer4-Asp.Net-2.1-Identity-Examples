using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    /// <summary>
    /// AutoMapper configuration for CorsOrigin 
    /// Between model and entity
    /// </summary>
    public class Neo4JIdentityServer4ClientCorsOriginMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiSecretMapperProfile}</cref>
        /// </see>
        /// </summary>
        public Neo4JIdentityServer4ClientCorsOriginMapperProfile()
        {
            // entity to model
            CreateMap<Neo4jIdentityServer4ClientCorsOrigin, string>()
                .ConvertUsing(r => r.Origin);


            // model to entity
            CreateMap<string, Neo4jIdentityServer4ClientCorsOrigin>(MemberList.Source)
                .ForMember(dest => dest.Origin, opt => opt.ResolveUsing(src => src.ToString()));

            CreateMap<
                Neo4jIdentityServer4ClientCorsOrigin,
                StoresIdentityServer4.Neo4j.Entities.ClientCorsOrigin
            >(MemberList.Source);

            CreateMap<
                StoresIdentityServer4.Neo4j.Entities.ClientCorsOrigin,
                Neo4jIdentityServer4ClientCorsOrigin
            >(MemberList.Source);

        }
    }
}