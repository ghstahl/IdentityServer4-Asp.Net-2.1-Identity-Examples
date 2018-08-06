using System.Collections.Generic;
using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    /// <summary>
    /// AutoMapper configuration for Property 
    /// Between model and entity
    /// </summary>
    public class Neo4jIdentityServer4PropertyMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiScopeMapperProfile}</cref>
        /// </see>
        /// </summary>
        public Neo4jIdentityServer4PropertyMapperProfile()
        {
            // entity to model
            CreateMap<Neo4jIdentityServer4ClientProperty, KeyValuePair<string, string>>(MemberList.Source);

            // model to entity
            CreateMap<KeyValuePair<string, string>, Neo4jIdentityServer4ClientProperty>(MemberList.Source);

        }
    }
}