using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Logging;
using PagesWebApp.Agent;

namespace PagesWebApp.MiddleWare
{
    public class PageAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private IAgentTracker _agentTracker;

        /// <summary>
        /// Creates a new instance of <see cref="RewriteMiddleware"/>
        /// </summary>
        /// <param name="next">The delegate representing the next middleware in the request pipeline.</param>
        /// <param name="hostingEnvironment">The Hosting Environment.</param>
        /// <param name="loggerFactory">The Logger Factory.</param>
        /// <param name="options">The middleware options, containing the rules to apply.</param>
        public PageAuthMiddleware(
            RequestDelegate next,
            IHostingEnvironment hostingEnvironment,
            ILoggerFactory loggerFactory,
            IAgentTracker agentTracker)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            _agentTracker = agentTracker;
            

            _next = next;
            
            _logger = loggerFactory.CreateLogger<RewriteMiddleware>();
        }

        /// <summary>
        /// Executes the middleware.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
        /// <returns>A task that represents the execution of this middleware.</returns>
        public Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Request.Path.StartsWithSegments(new PathString("/Identity/Account/Login")))
            {
                if (!_agentTracker.IsLoggedIn)
                {
                    context.Response.Redirect($"/Identity/Account/ExternalAgentLogins?returnUrl={context.Request.Path}");
                    return Task.CompletedTask;
                }
            }

         
            return _next(context);
        }
    }
}