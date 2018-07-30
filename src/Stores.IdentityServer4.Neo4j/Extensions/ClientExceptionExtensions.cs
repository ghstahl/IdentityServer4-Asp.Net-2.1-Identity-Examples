using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;

namespace Stores.IdentityServer4.Neo4j
{
    public static class ClientExceptionExtensions
    {
        public static IdentityResult ToIdentityResult(this ClientException ex)
        {
            return IdentityResult.Failed(new IdentityError[]
            {
                new IdentityError() {Code = ex.Code, Description = ex.Message}
            });
        }
    }
}