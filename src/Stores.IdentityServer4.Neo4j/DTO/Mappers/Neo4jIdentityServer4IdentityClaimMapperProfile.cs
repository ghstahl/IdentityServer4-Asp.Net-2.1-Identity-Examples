using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    /// <summary>
    /// AutoMapper configuration for IdentityClaim 
    /// Between model and entity
    /// </summary>
    public class Neo4jIdentityServer4IdentityClaimMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiScopeMapperProfile}</cref>
        /// </see>
        /// </summary>
        public Neo4jIdentityServer4IdentityClaimMapperProfile()
        {
            // entity to model
            CreateMap<Neo4jIdentityServer4IdentityClaim, string>().ConvertUsing(r => r.Type);
           

            // model to entity
            CreateMap<string, Neo4jIdentityServer4IdentityClaim>(MemberList.Source)
                .ForMember(dest => dest.Type, opt => opt.ResolveUsing(src=>src.ToString()));

        }
    }
}