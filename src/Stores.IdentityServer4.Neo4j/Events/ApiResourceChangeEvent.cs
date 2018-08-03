using IdentityServer4.Events;

namespace Stores.IdentityServer4Neo4j.Events
{
    public class ApiResourceChangeEvent<TApiResource> : Event
        where TApiResource : StoresIdentityServer4.Neo4j.Entities.ApiResource
    {
        public TApiResource ApiResource { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResourceChangeEvent"/> class.
        /// </summary>
        protected ApiResourceChangeEvent()
            : base("ApiResourceStore",
                "ApiResource Change Event",
                EventTypes.Information,
                EventIds.ApiResourceChange)
        {
        }
        public ApiResourceChangeEvent(TApiResource apiResource) : this()
        {

        }
    }
}