using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
 

namespace StoresIdentityServer4.Neo4j.Mappers
{
    // <summary>
    /// Extension methods to map to/from entity/model for clients.
    /// </summary>
    public static class ClientMappers
    {
        static ClientMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static IdentityServer4.Models.ClientExtra ToModel(this Entities.ClientExtra entity)
        {
            return Mapper.Map<IdentityServer4.Models.ClientExtra>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.ClientExtra ToEntity(this IdentityServer4.Models.ClientExtra model)
        {
            return Mapper.Map<Entities.ClientExtra>(model);
        }
    }


    // <summary>
}
