﻿using AspNetCore.Identity.Neo4j;
using IdentityServer4.Stores;
using IdentityServer4Extras;
using Microsoft.Extensions.DependencyInjection;
using Stores.IdentityServer4Neo4j.Events;

namespace StoresIdentityServer4.Neo4j
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNeo4jClientStore<TUser>(this IServiceCollection serviceCollection)
            where TUser : Neo4jIdentityUser
        {
            serviceCollection.AddScoped<Neo4jIdentityServer4ClientUserStore<TUser>>();
            serviceCollection
                .AddScoped<
                        IIdentityServer4ClientUserStore<
                            TUser,
                            Neo4jIdentityServer4Client,
                            Neo4jIdentityServer4ClientSecret,
                            Neo4JIdentityServer4GrantType,
                            Neo4jIdentityServer4ApiResource,
                            Neo4jIdentityServer4ApiResourceClaim,
                            Neo4jIdentityServer4ApiSecret,
                            Neo4jIdentityServer4ApiScope,
                            Neo4jIdentityServer4ApiScopeClaim,
                            Neo4jIdentityServer4ClientClaim,
                            Neo4jIdentityServer4ClientCorsOrigin,
                            Neo4jIdentityServer4ClientScope,
                            Neo4JIdentityServer4ClientIdpRestriction,
                            Neo4jIdentityServer4ClientProperty,
                            Neo4jIdentityServer4ClientPostLogoutRedirectUri,
                            Neo4jIdentityServer4ClientRedirectUri,
                            Neo4jIdentityServer4IdentityResource,
                            Neo4jIdentityServer4IdentityClaim>>
                    (x => { return x.GetService<Neo4jIdentityServer4ClientUserStore<TUser>>(); });

            serviceCollection.AddScoped<IClientStore>
                (x => { return x.GetService<Neo4jIdentityServer4ClientUserStore<TUser>>(); });
            serviceCollection.AddScoped<IResourceStore>
                (x => { return x.GetService<Neo4jIdentityServer4ClientUserStore<TUser>>(); });

            //   serviceCollection.AddScoped<INeo4jEventSink>(x => { return x.GetService<Neo4jIdentityServer4ClientUserStore<TUser>>(); });
            serviceCollection
                .AddTransient<
                    INeo4jIdentityServer4Database,
                    Neo4jIdentityServer4ClientUserStore<TUser>>();
            serviceCollection
                .AddTransient<
                    IIdentityServer4ModelsPopulation<TUser>,
                    Neo4jIdentityServer4ClientUserStore<TUser>>();



            serviceCollection.AddScoped<INeo4jEventSink, ClientEventSink<TUser>>();
            serviceCollection.AddScoped<INeo4jEventSink, ApiScopeEventSink<TUser>>();
            serviceCollection.AddScoped<INeo4jEventSink, ApiResourceEventSink<TUser>>();
            serviceCollection.AddScoped<INeo4jEventSink, IdentityResourceEventSink<TUser>>();


            serviceCollection.AddScoped<INeo4jEventService, Neo4jEventService>();
            serviceCollection
                .AddScoped<IIdentityServer4ClientUserStoreAccessor<TUser,
                        Neo4jIdentityServer4Client,
                        Neo4jIdentityServer4ClientSecret,
                        Neo4JIdentityServer4GrantType,
                        Neo4jIdentityServer4ApiResource,
                        Neo4jIdentityServer4ApiResourceClaim,
                        Neo4jIdentityServer4ApiSecret,
                        Neo4jIdentityServer4ApiScope,
                        Neo4jIdentityServer4ApiScopeClaim,
                        Neo4jIdentityServer4ClientClaim,
                        Neo4jIdentityServer4ClientCorsOrigin,
                        Neo4jIdentityServer4ClientScope,
                        Neo4JIdentityServer4ClientIdpRestriction,
                        Neo4jIdentityServer4ClientProperty,
                        Neo4jIdentityServer4ClientPostLogoutRedirectUri,
                        Neo4jIdentityServer4ClientRedirectUri,
                        Neo4jIdentityServer4IdentityResource,
                        Neo4jIdentityServer4IdentityClaim>,
                    Neo4jIdentityServer4ClientUserStoreAccessor<TUser>>();
        }
    }
}