namespace Stores.IdentityServer4Neo4j.Events
{
    public static class EventIds
    { 
        //////////////////////////////////////////////////////
        /// ClientStore related events
        //////////////////////////////////////////////////////
        private const int ClientStoreEventsStart = 1000;

        public const int ClientChange = ClientStoreEventsStart + 0;

        //////////////////////////////////////////////////////
        /// ApiScopeStore related events
        //////////////////////////////////////////////////////
        private const int ApiScopeStoreEventsStart = 2000;

        public const int ApiScopeChange = ApiScopeStoreEventsStart + 0;

        //////////////////////////////////////////////////////
        /// ApiResourceStore related events
        //////////////////////////////////////////////////////
        private const int ApiResourceStoreEventsStart = 3000;

        public const int ApiResourceChange = ApiResourceStoreEventsStart + 0;

    }
}