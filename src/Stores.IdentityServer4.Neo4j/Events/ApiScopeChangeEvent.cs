using IdentityServer4.Events;

namespace Stores.IdentityServer4Neo4j.Events
{
    public class ApiScopeChangeEvent<TApiResource,TApiScope> : Event
        where TApiResource : StoresIdentityServer4.Neo4j.Entities.ApiResource
        where TApiScope : StoresIdentityServer4.Neo4j.Entities.ApiScope
    {
        public TApiResource ApiResource { get; }
        public TApiScope ApiScope { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiScopeChangeEvent"/> class.
        /// </summary>
        protected ApiScopeChangeEvent()
            : base("ApiScopeStore",
                "ApiScope Change Event",
                EventTypes.Information,
                EventIds.ApiScopeChange)
        {
        }
        public ApiScopeChangeEvent(TApiResource apiResource, TApiScope apiScope) : this()
        {
            ApiResource = apiResource;
            ApiScope = apiScope;
        }
    }
}