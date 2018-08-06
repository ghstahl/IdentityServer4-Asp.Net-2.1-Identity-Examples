using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    /// <summary>
    /// AutoMapper configuration for ClientScope 
    /// Between model and entity
    /// </summary>
    public class Neo4JIdentityServer4ClientScopeMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiSecretMapperProfile}</cref>
        /// </see>
        /// </summary>
        public Neo4JIdentityServer4ClientScopeMapperProfile()
        {
            // entity to model
            CreateMap<Neo4jIdentityServer4ClientScope, string>()
                .ConvertUsing(r => r.Scope);


            // model to entity
            CreateMap<string, Neo4jIdentityServer4ClientScope>(MemberList.Source)
                .ForMember(dest => dest.Scope, opt => opt.ResolveUsing(src => src.ToString()));

            CreateMap<
                Neo4jIdentityServer4ClientScope,
                StoresIdentityServer4.Neo4j.Entities.ClientScope
            >(MemberList.Source);

            CreateMap<
                StoresIdentityServer4.Neo4j.Entities.ClientScope,
                Neo4jIdentityServer4ClientScope
            >(MemberList.Source);

        }
    }
}