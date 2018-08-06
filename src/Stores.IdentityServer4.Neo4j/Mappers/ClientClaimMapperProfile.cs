using System.Security.Claims;
using AutoMapper;

namespace StoresIdentityServer4.Neo4j.Mappers
{
    /// <summary>
    /// AutoMapper configuration for ClientClaim
    /// Between model and entity
    /// </summary>
    public class ClientClaimMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ClientMapperProfile}</cref>
        /// </see>
        /// </summary>
        public ClientClaimMapperProfile()
        {
            // entity to model
            CreateMap<Entities.ClientClaim, Claim>(MemberList.Destination)
                .ForMember(x => x.Issuer, options => options.Ignore())
                .ForMember(x => x.OriginalIssuer, options => options.Ignore())
                .ForMember(x => x.Properties, options => options.Ignore())
                .ForMember(x => x.Subject, options => options.Ignore())
                .ForMember(x => x.ValueType, options => options.Ignore())
                ;
            // model to entity
            CreateMap<Claim, Entities.ClientClaim>(MemberList.Source);
        }
    }
}