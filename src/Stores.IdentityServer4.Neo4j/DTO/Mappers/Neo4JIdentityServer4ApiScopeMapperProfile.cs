using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    /// <summary>
    /// AutoMapper configuration for ApiScope 
    /// Between model and entity
    /// </summary>
    public class Neo4JIdentityServer4ApiScopeMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiScopeMapperProfile}</cref>
        /// </see>
        /// </summary>
        public Neo4JIdentityServer4ApiScopeMapperProfile()
        {

            CreateMap<
                Neo4jIdentityServer4ApiScope,
                StoresIdentityServer4.Neo4j.Entities.ApiScope
            >(MemberList.Source);
            CreateMap<
                StoresIdentityServer4.Neo4j.Entities.ApiScope,
                Neo4jIdentityServer4ApiScope
            >(MemberList.Source);
            // entity to model
            CreateMap<Neo4jIdentityServer4ApiScope, IdentityServer4.Models.Scope>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opt => opt.Ignore());

            // model to entity
            CreateMap<IdentityServer4.Models.Scope, Neo4jIdentityServer4ApiScope>(MemberList.Source);

        }
    }
}