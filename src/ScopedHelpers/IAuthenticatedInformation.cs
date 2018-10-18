using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace ScopedHelpers
{
    public interface IAuthenticatedInformation
    {
        Task<AuthenticateResult> GetAuthenticateResultAsync();
    }
}