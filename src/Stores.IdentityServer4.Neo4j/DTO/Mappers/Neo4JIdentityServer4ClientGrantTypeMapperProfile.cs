using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    /// <summary>
    /// AutoMapper configuration for ClientGrantType 
    /// Between model and entity
    /// </summary>
    public class Neo4JIdentityServer4ClientGrantTypeMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiSecretMapperProfile}</cref>
        /// </see>
        /// </summary>
        public Neo4JIdentityServer4ClientGrantTypeMapperProfile()
        {
            // entity to model
            CreateMap<Neo4jIdentityServer4ClientGrantType, string>()
                .ConvertUsing(r => r.GrantType);


            // model to entity
            CreateMap<string, Neo4jIdentityServer4ClientGrantType>(MemberList.Source)
                .ForMember(dest => dest.GrantType, opt => opt.ResolveUsing(src => src.ToString()));

            CreateMap<
                Neo4jIdentityServer4ClientGrantType,
                StoresIdentityServer4.Neo4j.Entities.ClientGrantType
            >(MemberList.Source);

            CreateMap<
                StoresIdentityServer4.Neo4j.Entities.ClientGrantType,
                Neo4jIdentityServer4ClientGrantType
            >(MemberList.Source);

        }
    }
}