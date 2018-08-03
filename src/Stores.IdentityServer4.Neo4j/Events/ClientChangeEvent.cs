using AspNetCore.Identity.Neo4j;
using IdentityServer4.Events;
using StoresIdentityServer4.Neo4j;
using StoresIdentityServer4.Neo4j.Entities;

namespace Stores.IdentityServer4Neo4j.Events
{
    public class ClientChangeEvent<TClient> : Event
        where TClient : Client
    {
        public TClient Client { get;  }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientChangeEvent"/> class.
        /// </summary>
        protected ClientChangeEvent()
            : base(StoresIdentityServer4.Neo4j.Events.Constants.Categories.ClientStore,
                "Client Change Event",
                EventTypes.Information,
                EventIds.ClientChange)
        {
        }
        public ClientChangeEvent( TClient client) : this()
        {
            Client = client;
        }
    }
}
