using System.Security.Claims;
using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    /// <summary>
    /// AutoMapper configuration for ClientClaim 
    /// Between model and entity
    /// </summary>
    public class Neo4JIdentityServer4ClientClaimMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiScopeMapperProfile}</cref>
        /// </see>
        /// </summary>
        public Neo4JIdentityServer4ClientClaimMapperProfile()
        {

            CreateMap<
                Neo4jIdentityServer4ClientClaim,
                StoresIdentityServer4.Neo4j.Entities.ClientClaim
            >(MemberList.Source);
            CreateMap<
                StoresIdentityServer4.Neo4j.Entities.ClientClaim,
                Neo4jIdentityServer4ClientClaim
            >(MemberList.Source);
            // entity to model
            CreateMap<Neo4jIdentityServer4ClientClaim, Claim>(MemberList.Destination)
                .ForMember(x => x.Issuer, options => options.Ignore())
                .ForMember(x => x.OriginalIssuer, options => options.Ignore())
                .ForMember(x => x.Properties, options => options.Ignore())
                .ForMember(x => x.Subject, options => options.Ignore())
                .ForMember(x => x.ValueType, options => options.Ignore())
                ;

            // model to entity
            CreateMap<Claim, Neo4jIdentityServer4ClientClaim>(MemberList.Source);

        }
    }
}