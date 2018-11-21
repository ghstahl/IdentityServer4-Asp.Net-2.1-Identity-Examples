using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PagesWebApp.Agent;

namespace PagesWebApp.ClaimsFactory
{
    public class AppClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser, TRole>
        where TUser : class
        where TRole : class
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IAgentTracker _agentTracker;
        private IChallengeQuestionsTracker _challengeQuestionsTracker;

        public AppClaimsPrincipalFactory(
            IAgentTracker agentTracker,
            IChallengeQuestionsTracker challengeQuestionsTracker,
            IHttpContextAccessor httpContextAccessor,
            UserManager<TUser> userManager,
            RoleManager<TRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor
        )
            : base(userManager, roleManager, optionsAccessor)
        {
            _agentTracker = agentTracker;
            _challengeQuestionsTracker = challengeQuestionsTracker;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            var principal = await base.CreateAsync(user);
            var items = _httpContextAccessor.HttpContext.Items;

            ((ClaimsIdentity)principal.Identity).AddClaim(new Claim(JwtClaimTypes.AuthenticationMethod, $"KBADerived"));
            ((ClaimsIdentity)principal.Identity).AddClaim(new Claim(JwtClaimTypes.AuthenticationMethod, $"agent:username:{_agentTracker.UserName}"));
            
            _challengeQuestionsTracker.Retrieve();
            foreach (var challengeQuestion in _challengeQuestionsTracker.ChallengeQuestions)
            {
                ((ClaimsIdentity)principal.Identity).AddClaim(
                    new Claim(JwtClaimTypes.AuthenticationMethod, 
                        $"agent:challengeQuestion:{challengeQuestion.Key}"));
            }
            return principal;
        }
    }
}
