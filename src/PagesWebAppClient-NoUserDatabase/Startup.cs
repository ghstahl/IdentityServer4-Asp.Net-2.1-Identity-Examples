using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
 using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using PagesWebAppClient.InMemory;
using PagesWebAppClient.Services;
using PagesWebAppClient.Utils;
using OAuth2SchemeRecord = IdentityModelExtras.OAuth2SchemeRecord;

namespace PagesWebAppClient
{
    public class TokenClientAccessor
    {
        private IOptions<List<OAuth2SchemeRecord>> _oAuth2SchemeRecords;
        private Dictionary<string, TokenClient> TokenClients { get; set; }
        public TokenClientAccessor(IOptions<List<OAuth2SchemeRecord>> oAuth2SchemeRecords)
        {
            _oAuth2SchemeRecords = oAuth2SchemeRecords;
            TokenClients = new Dictionary<string, TokenClient>();
        }
        public async Task<TokenClient> TokenClientByAuthorityAsync(string authority)
        {
            var record = (from item in _oAuth2SchemeRecords.Value
                where item.Authority == authority
                select item).FirstOrDefault();
            if (record == null)
                return null;
            if (!TokenClients.ContainsKey(record.Authority))
            {
                var disco = await DiscoveryClient.GetAsync(record.Authority);
                if (disco.IsError)
                    throw new Exception(disco.Error);
                var tokenClient = new TokenClient(
                    disco.TokenEndpoint,
                    record.ClientId,
                    record.ClientSecret);
                TokenClients.Add(record.Authority, tokenClient);
            }
            return TokenClients[record.Authority];
        }
    }
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.ConsentCookie.Name = $"{Configuration["applicationName"]}.AspNetCore.Consent";
            });
            services.AddSingleton<ConfiguredDiscoverCacheContainerFactory>();

            var inMemoryStore = new InMemoryStore<ApplicationUser, ApplicationRole>();

            services.AddSingleton<IUserStore<ApplicationUser>>(provider =>
            {
                return inMemoryStore;
            });
            services.AddSingleton<IUserRoleStore<ApplicationUser>>(provider =>
            {
                return inMemoryStore;
            });
            services.AddSingleton<IRoleStore<ApplicationRole>>(provider =>
            {
                return inMemoryStore;
            });
            var identityBuilder = services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddDefaultTokenProviders();
    //        identityBuilder.Services.AddTransientDecorator<IAuthenticationService, IdentityServerAuthenticationService>();
    //       identityBuilder.Services.AddTransientDecorator<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsFactory<ApplicationUser>>();

            services.ConfigureApplicationCookie(options => {
                options.Cookie.Name = $"{Configuration["applicationName"]}.AspNetCore.Identity.Application";
            });
            services.AddAuthentication<ApplicationUser>(Configuration);
         

            // Hosting doesn't add IHttpContextAccessor by default
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddSessionStateTempDataProvider();

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.Cookie.Name = $"{Configuration["applicationName"]}.Session";
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(3600);
                options.Cookie.HttpOnly = true;
            });
            services.Configure<List<OAuth2SchemeRecord>>(Configuration.GetSection("oauth2"));
            services.AddSingleton<TokenClientAccessor>();
        }
       
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseSession();
            app.UseMvc();
        }
    }
}
