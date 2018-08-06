using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    /// <summary>
    /// AutoMapper configuration for RedirectUri 
    /// Between model and entity
    /// </summary>
    public class Neo4JIdentityServer4ClientRedirectUriMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ClientRedirectUri}</cref>
        /// </see>
        /// </summary>
        public Neo4JIdentityServer4ClientRedirectUriMapperProfile()
        {
            // entity to model
            CreateMap<Neo4jIdentityServer4ClientRedirectUri, string>()
                .ConvertUsing(r => r.RedirectUri);


            // model to entity
            CreateMap<string, Neo4jIdentityServer4ClientRedirectUri>(MemberList.Source)
                .ForMember(dest => dest.RedirectUri, opt => opt.ResolveUsing(src => src.ToString()));

            CreateMap<
                Neo4jIdentityServer4ClientRedirectUri,
                StoresIdentityServer4.Neo4j.Entities.ClientRedirectUri
            >(MemberList.Source);

            CreateMap<
                StoresIdentityServer4.Neo4j.Entities.ClientRedirectUri,
                Neo4jIdentityServer4ClientRedirectUri
            >(MemberList.Source);

        }
    }
}