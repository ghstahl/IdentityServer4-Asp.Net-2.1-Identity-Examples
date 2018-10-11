using System.Collections.Generic;

namespace ScopedHelpers
{
    public interface ITokenStore
    {
        void HarvestAndStore();
        Dictionary<string, string> Tokens { get; }
    }
}