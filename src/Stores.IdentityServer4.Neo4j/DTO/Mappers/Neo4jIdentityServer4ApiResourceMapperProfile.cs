using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    /// <summary>
    /// AutoMapper configuration for ApiResource 
    /// Between model and entity
    /// </summary>
    public class Neo4jIdentityServer4ApiResourceMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiScopeMapperProfile}</cref>
        /// </see>
        /// </summary>
        public Neo4jIdentityServer4ApiResourceMapperProfile()
        {

            CreateMap<
                Neo4jIdentityServer4ApiResource,
                StoresIdentityServer4.Neo4j.Entities.ApiResource
            >(MemberList.Source);
            CreateMap<
                StoresIdentityServer4.Neo4j.Entities.ApiResource,
                Neo4jIdentityServer4ApiResource
            >(MemberList.Source);
            // entity to model
            CreateMap<Neo4jIdentityServer4ApiResource, IdentityServer4.Models.ApiResource>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opt => opt.Ignore())
                .ForMember(x => x.Scopes, opt => opt.Ignore())
                .ForMember(x => x.ApiSecrets, opt => opt.Ignore());

            // model to entity
            CreateMap<IdentityServer4.Models.ApiResource, Neo4jIdentityServer4ApiResource>(MemberList.Source);

        }
    }
}