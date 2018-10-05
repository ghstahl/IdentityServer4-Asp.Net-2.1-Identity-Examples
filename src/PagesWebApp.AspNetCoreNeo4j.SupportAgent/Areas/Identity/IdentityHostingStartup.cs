using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PagesWebApp.AspNetCoreNeo4j.SupportAgent.Data;

[assembly: HostingStartup(typeof(PagesWebApp.AspNetCoreNeo4j.SupportAgent.Areas.Identity.IdentityHostingStartup))]
namespace PagesWebApp.AspNetCoreNeo4j.SupportAgent.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}