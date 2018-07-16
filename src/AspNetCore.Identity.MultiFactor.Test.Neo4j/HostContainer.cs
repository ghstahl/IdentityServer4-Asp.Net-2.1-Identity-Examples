using System;
using AspNetCore.Identity.MultiFactor.Test.Neo4j.Models;
using AspNetCore.Identity.Neo4j;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver.V1;

namespace AspNetCore.Identity.MultiFactor.Test.Neo4j
{
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
                    serviceCollection.AddNeo4jMultiFactorStore<TestFactor>();
                    serviceCollection.AddTransient<IdentityErrorDescriber>();
                    //  serviceCollection.AddNeo4jRestHook();

                    _serviceProvider = serviceCollection.BuildServiceProvider();
                }

                return _serviceProvider;
            }
        }
    }
}