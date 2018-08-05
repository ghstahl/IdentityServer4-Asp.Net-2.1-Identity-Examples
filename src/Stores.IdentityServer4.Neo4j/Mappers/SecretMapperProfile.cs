using AutoMapper;

namespace StoresIdentityServer4.Neo4j.Mappers
{
    /// <summary>
    /// AutoMapper configuration for Secret
    /// Between model and entity
    /// </summary>
    public class SecretMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiScopeMapperProfile}</cref>
        /// </see>
        /// </summary>
        public SecretMapperProfile()
        {
            // entity to model
            CreateMap<StoresIdentityServer4.Neo4j.Entities.Secret, IdentityServer4.Models.Secret>(MemberList.Destination);
            // model to entity
            CreateMap<IdentityServer4.Models.Secret, StoresIdentityServer4.Neo4j.Entities.Secret>(MemberList.Source);
        }
    }
}