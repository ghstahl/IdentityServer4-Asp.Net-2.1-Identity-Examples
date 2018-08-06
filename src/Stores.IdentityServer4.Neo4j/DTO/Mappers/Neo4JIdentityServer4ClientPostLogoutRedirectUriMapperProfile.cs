using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    /// <summary>
    /// AutoMapper configuration for PostLogoutRedirectUri 
    /// Between model and entity
    /// </summary>
    public class Neo4JIdentityServer4ClientPostLogoutRedirectUriMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiSecretMapperProfile}</cref>
        /// </see>
        /// </summary>
        public Neo4JIdentityServer4ClientPostLogoutRedirectUriMapperProfile()
        {
            // entity to model
            CreateMap<Neo4jIdentityServer4ClientPostLogoutRedirectUri, string>()
                .ConvertUsing(r => r.PostLogoutRedirectUri);


            // model to entity
            CreateMap<string, Neo4jIdentityServer4ClientPostLogoutRedirectUri>(MemberList.Source)
                .ForMember(dest => dest.PostLogoutRedirectUri, opt => opt.ResolveUsing(src => src.ToString()));

            CreateMap<
                Neo4jIdentityServer4ClientPostLogoutRedirectUri,
                StoresIdentityServer4.Neo4j.Entities.ClientPostLogoutRedirectUri
            >(MemberList.Source);

            CreateMap<
                StoresIdentityServer4.Neo4j.Entities.ClientPostLogoutRedirectUri,
                Neo4jIdentityServer4ClientPostLogoutRedirectUri
            >(MemberList.Source);

        }
    }
}