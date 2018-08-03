using IdentityServer4.Events;

namespace Stores.IdentityServer4Neo4j.Events
{
    public class IdentityResourceChangeEvent<TIdentityResource> : Event
        where TIdentityResource : StoresIdentityServer4.Neo4j.Entities.IdentityResource
    {
        public TIdentityResource IdentityResource { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityResourceChangeEvent"/> class.
        /// </summary>
        protected IdentityResourceChangeEvent()
            : base(StoresIdentityServer4.Neo4j.Events.Constants.Categories.IdentityResourceStore,
                "IdentityResource Change Event",
                EventTypes.Information,
                EventIds.IdentityResourceChange)
        {
        }
        public IdentityResourceChangeEvent(TIdentityResource identityResource) : this()
        {
            IdentityResource = identityResource;
        }
    }
}