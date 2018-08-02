using AutoMapper;
using StoresIdentityServer4.Neo4j.Entities;

namespace StoresIdentityServer4.Neo4j.Mappers
{
    /// <summary>
    /// AutoMapper configuration for Client
    /// Between model and entity
    /// </summary>
    public class ApiScopeMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiScopeMapperProfile}</cref>
        /// </see>
        /// </summary>
        public ApiScopeMapperProfile()
        {
            // entity to model
            CreateMap<ApiScope, IdentityServer4.Models.Scope>(MemberList.Destination);
            // model to entity
            CreateMap<IdentityServer4.Models.Scope, Entities.ApiScope>(MemberList.Source);
        }
    }
}