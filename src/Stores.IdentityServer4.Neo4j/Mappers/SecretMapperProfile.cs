using System;
using System.Globalization;
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
            var format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";
            var formatProvider = CultureInfo.InstalledUICulture;
            // entity to model
            CreateMap<
                StoresIdentityServer4.Neo4j.Entities.Secret, 
                IdentityServer4.Models.Secret
            >(MemberList.Destination)
                .ForMember(x=>x.Expiration,opt=>opt.ResolveUsing(s =>
                {
                    if (string.IsNullOrEmpty(s.ExpirationJson))
                        return (DateTime?)null;
                    return  DateTime.ParseExact(s.ExpirationJson, format, formatProvider);

                }));
            // model to entity
            CreateMap<
                    IdentityServer4.Models.Secret, 
                    StoresIdentityServer4.Neo4j.Entities.Secret
                >(MemberList.Source)
                .ForMember(x=>x.ExpirationJson,opt=>opt.ResolveUsing(s =>
                {
                   
                    var dateTime = s.Expiration?.ToString(format, formatProvider);
                    return dateTime;
                }));
        }
    }
}