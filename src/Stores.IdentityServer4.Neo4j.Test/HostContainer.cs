using AspNetCore.Identity.Neo4j;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver.V1;
using StoresIdentityServer4.Neo4j.Test.Models;
using StoresIdentityServer4.Neo4j;

namespace StoresIdentityServer4.Neo4j.Test
{
    class TenantStore : ITenantStore
    {
        public string TenantId { get; set; }
    }
    public static class HostContainer
    {
        private static ServiceProvider _serviceProvider;

        public static ServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    var serviceCollection = new ServiceCollection()
                        .AddLogging();
                    serviceCollection.AddSingleton(s =>
                        GraphDatabase.Driver("bolt://127.0.0.1:7687", AuthTokens.Basic("neo4j", "password")));
                    serviceCollection.AddScoped(s => s.GetService<IDriver>().Session());
                    serviceCollection.AddScoped<IUserStore<TestUser>, Neo4jUserStore<TestUser>>();
                    serviceCollection.AddScoped<ITenantStore>(s =>
                    {
                        var d = new TenantStore() { TenantId = "TestTenant" };
                        return d;
                    });
                    serviceCollection.AddNeo4jClientStore<TestUser>();
                    serviceCollection.AddNeo4jTest();

                    serviceCollection.AddTransient<IdentityErrorDescriber>();
                    //  serviceCollection.AddNeo4jRestHook();

                    _serviceProvider = serviceCollection.BuildServiceProvider();
                }

                return _serviceProvider;
            }
        }
    }
}