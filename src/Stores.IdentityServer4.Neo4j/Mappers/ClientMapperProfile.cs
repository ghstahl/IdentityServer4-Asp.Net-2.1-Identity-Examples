using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using StoresIdentityServer4.Neo4j.Entities;

namespace StoresIdentityServer4.Neo4j.Mappers
{
    /// <summary>
    /// AutoMapper configuration for Client
    /// Between model and entity
    /// </summary>
    public class ClientMapperProfile : Profile
    {
        /// <summary>
        /// <see>
        ///     <cref>{ClientMapperProfile}</cref>
        /// </see>
        /// </summary>
        public ClientMapperProfile()
        {
            // entity to model
            CreateMap<Entities.ClientExtra, IdentityServer4.Models.ClientExtra>(MemberList.Destination);
            // model to entity
            CreateMap<IdentityServer4.Models.ClientExtra, Entities.ClientExtra>(MemberList.Source);
        }
    }
}
