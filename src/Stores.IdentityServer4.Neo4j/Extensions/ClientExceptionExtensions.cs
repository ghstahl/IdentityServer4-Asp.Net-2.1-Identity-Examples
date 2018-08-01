using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;

namespace StoresIdentityServer4.Neo4j
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