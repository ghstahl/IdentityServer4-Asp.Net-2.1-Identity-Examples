using System;
using System.Linq;
using AutoMapper;
using StoresIdentityServer4.Neo4j.Entities;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    /// <summary>
    /// AutoMapper configuration for IdentityResource 
    /// Between model and entity
    /// </summary>
    public class Neo4jIdentityServer4IdentityResourceMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiScopeMapperProfile}</cref>
        /// </see>
        /// </summary>
        public Neo4jIdentityServer4IdentityResourceMapperProfile()
        {

            CreateMap<
                Neo4jIdentityServer4IdentityResource,
                StoresIdentityServer4.Neo4j.Entities.IdentityResource
            >(MemberList.Source);
            CreateMap<
                StoresIdentityServer4.Neo4j.Entities.IdentityResource,
                Neo4jIdentityServer4IdentityResource
            >(MemberList.Source);
            // entity to model
            CreateMap<Neo4jIdentityServer4IdentityResource, IdentityServer4.Models.IdentityResource>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opt => opt.Ignore());

            // model to entity
            CreateMap<IdentityServer4.Models.IdentityResource, Neo4jIdentityServer4IdentityResource>(MemberList.Source);

        }
    }
}
