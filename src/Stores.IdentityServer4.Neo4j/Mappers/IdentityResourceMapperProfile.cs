using AutoMapper;
using StoresIdentityServer4.Neo4j.Entities;

namespace StoresIdentityServer4.Neo4j.Mappers
{
    /// <summary>
    /// AutoMapper configuration for IdentityResource 
    /// Between model and entity
    /// </summary>
    public class IdentityResourceMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiScopeMapperProfile}</cref>
        /// </see>
        /// </summary>
        public IdentityResourceMapperProfile()
        {
            // entity to model
            CreateMap<IdentityResource, IdentityServer4.Models.IdentityResource>(MemberList.Destination);
            // model to entity
            CreateMap<IdentityServer4.Models.IdentityResource, Entities.IdentityResource>(MemberList.Source);
        }
    }
}