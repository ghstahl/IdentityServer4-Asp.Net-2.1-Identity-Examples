using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    /// <summary>
    /// AutoMapper configuration for ClientSecret 
    /// Between model and entity
    /// </summary>
    public class Neo4JIdentityServer4ClientSecretMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiSecretMapperProfile}</cref>
        /// </see>
        /// </summary>
        public Neo4JIdentityServer4ClientSecretMapperProfile()
        {

            CreateMap<
                Neo4jIdentityServer4ClientSecret,
                StoresIdentityServer4.Neo4j.Entities.Secret
            >(MemberList.Source);

            CreateMap<
                StoresIdentityServer4.Neo4j.Entities.Secret,
                Neo4jIdentityServer4ClientSecret
            >(MemberList.Source);

            // entity to model
            CreateMap<
                Neo4jIdentityServer4ClientSecret,
                IdentityServer4.Models.Secret
            >(MemberList.Destination);

            // model to entity
            CreateMap<
                IdentityServer4.Models.Secret,
                Neo4jIdentityServer4ClientSecret
            >(MemberList.Source);

        }
    }
}