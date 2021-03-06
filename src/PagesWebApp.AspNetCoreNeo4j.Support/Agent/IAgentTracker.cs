﻿using System;
using System.Linq;
using System.Threading.Tasks;

namespace PagesWebApp.Agent
{
    public interface IAgentTracker
    {
        bool IsLoggedIn { get; }
        string UserName { get; }
        string UserId { get; }
        void StoreIdToken(string idToken);
        void RemoveIdToken();
    }
}
