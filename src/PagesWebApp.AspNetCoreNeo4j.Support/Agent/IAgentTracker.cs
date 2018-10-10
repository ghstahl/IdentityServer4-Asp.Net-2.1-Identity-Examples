using System;
using System.Collections.Generic;
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
    public interface IChallengeQuestionsTracker
    {
        Dictionary<string,bool> ChallengeQuestions { get; }
        void Store();
        void Retrieve();
        void Remove();
    }
}
