using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    /// <summary>
    /// AutoMapper configuration for ApiSecret 
    /// Between model and entity
    /// </summary>
    public class Neo4JIdentityServer4ApiSecretMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiSecretMapperProfile}</cref>
        /// </see>
        /// </summary>
        public Neo4JIdentityServer4ApiSecretMapperProfile()
        {

            CreateMap<
                Neo4jIdentityServer4ApiSecret,
                StoresIdentityServer4.Neo4j.Entities.ApiSecret
            >(MemberList.Source);

            CreateMap<
                StoresIdentityServer4.Neo4j.Entities.ApiSecret,
                Neo4jIdentityServer4ApiSecret
            >(MemberList.Source);

            // entity to model
            CreateMap<
                Neo4jIdentityServer4ApiSecret, 
                IdentityServer4.Models.Secret
            >(MemberList.Destination);

            // model to entity
            CreateMap<
                IdentityServer4.Models.Secret, 
                Neo4jIdentityServer4ApiSecret
            >(MemberList.Source);

        }
    }
}