using AutoMapper;
using StoresIdentityServer4.Neo4j.Entities;

namespace StoresIdentityServer4.Neo4j.Mappers
{
    /// <summary>
    /// AutoMapper configuration for ApiResource 
    /// Between model and entity
    /// </summary>
    public class ApiResourceMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiScopeMapperProfile}</cref>
        /// </see>
        /// </summary>
        public ApiResourceMapperProfile()
        {
            // entity to model
            CreateMap<ApiResource, IdentityServer4.Models.ApiResource>(MemberList.Destination);
            // model to entity
            CreateMap<IdentityServer4.Models.ApiResource, Entities.ApiResource>(MemberList.Source);
        }
    }
}