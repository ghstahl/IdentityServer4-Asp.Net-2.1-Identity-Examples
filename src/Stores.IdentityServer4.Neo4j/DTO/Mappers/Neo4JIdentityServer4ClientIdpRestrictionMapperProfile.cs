using AutoMapper;

namespace StoresIdentityServer4.Neo4j.DTO.Mappers
{
    /// <summary>
    /// AutoMapper configuration for IDPRestriction 
    /// Between model and entity
    /// </summary>
    public class Neo4JIdentityServer4ClientIdpRestrictionMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ApiSecretMapperProfile}</cref>
        /// </see>
        /// </summary>
        public Neo4JIdentityServer4ClientIdpRestrictionMapperProfile()
        {
            // entity to model
            CreateMap<Neo4JIdentityServer4ClientIdpRestriction, string>()
                .ConvertUsing(r => r.Provider);


            // model to entity
            CreateMap<string, Neo4JIdentityServer4ClientIdpRestriction>(MemberList.Source)
                .ForMember(dest => dest.Provider, opt => opt.ResolveUsing(src => src.ToString()));

            CreateMap<
                Neo4JIdentityServer4ClientIdpRestriction,
                StoresIdentityServer4.Neo4j.Entities.ClientScope
            >(MemberList.Source);

            CreateMap<
                StoresIdentityServer4.Neo4j.Entities.ClientScope,
                Neo4JIdentityServer4ClientIdpRestriction
            >(MemberList.Source);

        }
    }
}