using System;
using System.Collections.Generic;

namespace ScopedHelpers
{
    public class ScopedOperation : IScopedOperation
    {
        private Dictionary<string, object> _dictionary;
        public Dictionary<string, object> Dictionary => _dictionary ?? (_dictionary = new Dictionary<string, object>());
    }
}
