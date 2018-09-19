using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    /// <summary>
    /// AutoMapper configuration for IdentityServer4GrantType 
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
            CreateMap<Neo4JIdentityServer4GrantType, string>()
                .ConvertUsing(r => r.GrantType);


            // model to entity
            CreateMap<string, Neo4JIdentityServer4GrantType>(MemberList.Source)
                .ForMember(dest => dest.GrantType, opt => opt.ResolveUsing(src => src.ToString()));

            CreateMap<
                Neo4JIdentityServer4GrantType,
                StoresIdentityServer4.Neo4j.Entities.IdentityServer4GrantType
            >(MemberList.Source);

            CreateMap<
                StoresIdentityServer4.Neo4j.Entities.IdentityServer4GrantType,
                Neo4JIdentityServer4GrantType
            >(MemberList.Source);

        }
    }
}