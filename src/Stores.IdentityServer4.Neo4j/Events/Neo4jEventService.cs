using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Events;

namespace Stores.IdentityServer4Neo4j.Events
{
    public class Neo4jEventService : INeo4jEventService
    {
        private IEnumerable<INeo4jEventSink> _sinks;

        public Neo4jEventService( IEnumerable<INeo4jEventSink> sinks)
        {
            _sinks = sinks;
        }
        public async Task RaiseAsync(Event evt)
        {
            if (_sinks != null)
            {
                foreach (var sink in _sinks)
                {
                    await sink.PersistAsync(evt);
                }
            }
        }

        public bool CanRaiseEventType(EventTypes evtType)
        {
            throw new NotImplementedException();
        }
    }
}
