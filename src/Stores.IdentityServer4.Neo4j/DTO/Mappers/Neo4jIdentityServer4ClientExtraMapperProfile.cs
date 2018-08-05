using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    /// <summary>
    /// AutoMapper configuration for ClientExtra 
    /// Between model and entity
    /// </summary>
    public class Neo4jIdentityServer4ClientExtraMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiScopeMapperProfile}</cref>
        /// </see>
        /// </summary>
        public Neo4jIdentityServer4ClientExtraMapperProfile()
        {

            CreateMap<
                Neo4jIdentityServer4Client,
                StoresIdentityServer4.Neo4j.Entities.ClientExtra
            >(MemberList.Source);
            CreateMap<
                StoresIdentityServer4.Neo4j.Entities.ClientExtra,
                Neo4jIdentityServer4Client
            >(MemberList.Source);
            // entity to model
            CreateMap<Neo4jIdentityServer4Client, IdentityServer4.Models.ClientExtra>(MemberList.Destination)
                .ForMember(x => x.AllowedCorsOrigins, opt => opt.Ignore())
                .ForMember(x => x.AllowedGrantTypes, opt => opt.Ignore())
                .ForMember(x => x.AllowedScopes, opt => opt.Ignore())
                .ForMember(x => x.Claims, opt => opt.Ignore())
                .ForMember(x => x.ClientSecrets, opt => opt.Ignore())
                .ForMember(x => x.IdentityProviderRestrictions, opt => opt.Ignore())
                .ForMember(x => x.PostLogoutRedirectUris, opt => opt.Ignore())
                .ForMember(x => x.Properties, opt => opt.Ignore())
                .ForMember(x => x.RedirectUris, opt => opt.Ignore());

            // model to entity
            CreateMap<IdentityServer4.Models.ClientExtra, Neo4jIdentityServer4Client>(MemberList.Source);

        }
    }
}