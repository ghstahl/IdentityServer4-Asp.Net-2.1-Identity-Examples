using System.Collections.Generic;

namespace PagesWebApp.Agent
{
    public interface IChallengeQuestionsTracker
    {
        Dictionary<string,bool> ChallengeQuestions { get; }
        void Store();
        void Retrieve();
        void Remove();
    }
}