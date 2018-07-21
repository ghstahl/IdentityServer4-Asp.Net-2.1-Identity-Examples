using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
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
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Neo4j.Driver.V1;
 

using PagesWebApp.Services;
using IdentityServer4Extras.Extensions;
using IdentityServer4Extras.Validation;

namespace PagesWebApp
{
    public class OAuth2SchemeRecord
    {
        public string Scheme { get; set; }
        public string ClientId { get; set; }
        public string Authority { get; set; }
        public string CallbackPath { get; set; }
        public List<string> AdditionalEndpointBaseAddresses { get; set; }
    }
    public class Startup
    {
        private IHostingEnvironment _hostingEnvironment;

        /*
        private static IGraphClient GetGraphClient()
        {
            var graphClient = new GraphClient(
                new Uri("http://localhost:7474/db/data"), "neo4j", "password");
            graphClient.Connect();
            return graphClient;
        }
        */
        public Startup(IConfiguration configuration,IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
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
            });

            services.AddSingleton(s => GraphDatabase.Driver(Configuration.GetConnectionString("DefaultConnection"), AuthTokens.Basic("neo4j", "password")));
            services.AddScoped(s => s.GetService<IDriver>().Session());

            services.AddIdentity<ApplicationUser, Neo4jIdentityRole>()
                .AddNeo4jDataStores()
                .AddNeo4jMultiFactorStore<ApplicationFactor>()
                .AddDefaultTokenProviders();

            services.AddDetection();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddTransient<IEmailSender, EmailSender>();

           

            // configure identity server with in-memory stores, keys, clients and scopes
            var identityServerBuilder = services.AddIdentityServer(options =>
                {
                    options.UserInteraction.LoginUrl = "/identity/account/login";
                    options.UserInteraction.LogoutUrl = "/identity/account/logout";
                    options.UserInteraction.ConsentUrl = "/identity/consent";
                    options.Authentication.CheckSessionCookieName = $".idsrv.session.{Configuration["appName"]}";
                })
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddAspNetIdentity<ApplicationUser>();
            
            // My Overrides.
            identityServerBuilder.ReplaceRedirectUriValidator<StrictRemoteRedirectUriValidator>();


            var authenticationBuilder = services.AddAuthentication();
            var googleClientId = Configuration["Google-ClientId"];
            var googleClientSecret = Configuration["Google-ClientSecret"];
            if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
            {
                authenticationBuilder.AddGoogle(options =>
                {
                    options.ClientId = googleClientId;
                    options.ClientSecret = googleClientSecret;
                    options.Events.OnRemoteFailure = context =>
                    {
                        context.Response.Redirect("/");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    };
                });
            }

            var section = Configuration.GetSection("oauth2");
            var oAuth2SchemeRecords = new List<OAuth2SchemeRecord>();
            section.Bind(oAuth2SchemeRecords);
            foreach (var record in oAuth2SchemeRecords)
            {
                var scheme = record.Scheme;
                authenticationBuilder.P7AddOpenIdConnect(scheme, scheme, options =>
                {
                    options.Authority = record.Authority;
                    options.CallbackPath = record.CallbackPath;
                    options.RequireHttpsMetadata = false;

                    options.ClientId = record.ClientId;
                    options.SaveTokens = true;

                    options.Events.OnRedirectToIdentityProvider = context =>
                    {
                        if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
                        {
                            context.ProtocolMessage.AcrValues = "some-acr=some-value";
                        }
                        return Task.CompletedTask;
                    };
                    options.Events.OnRemoteFailure = context =>
                    {
                        context.Response.Redirect("/");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    };
                });
            }


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // app.UseAuthentication(); // not needed, since UseIdentityServer adds the authentication middleware
            app.UseIdentityServer();

            app.UseMvc();
        }
    }
}
