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
            // model to entity
            CreateMap<
                    IdentityServer4.Models.ClientExtra,
                    Neo4jIdentityServer4Client>(MemberList.Destination)
                .ForMember(x => x.Description, opt => opt.Ignore());

            // entity to model
            CreateMap< 
                    Neo4jIdentityServer4Client,
                    IdentityServer4.Models.ClientExtra
                    >(MemberList.Source);

        }
    }
}