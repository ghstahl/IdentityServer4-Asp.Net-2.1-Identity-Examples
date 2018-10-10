using System;
using System.Collections.Generic;
using System.Text;

namespace ScopedHelpers
{
    public interface IScopedOperation
    {
        Dictionary<string,object> Dictionary { get; }
    }
}
