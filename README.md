# IdentityServer4 Asp.Net 2.1 Identity Examples

![Architecture](./Apps.png)


All these examples are based on the following;  
  * asp.net 2.1 WebApp  
  * asp.net identity  
  
## Minimal Apps
1. Neo4J desktop  
   The User Database
2. [PagesWebApp.AspNetCoreNeo4j](src/PagesWebApp.AspNetCoreNeo4j)  
   The Federated Gateway
3. [PagesWebApp.AspNetCoreNeo4j.Support](src/PagesWebApp.AspNetCoreNeo4j.Support)  
   The External Support IDP  



## Neo4j Setup
[Neo4j Desktop](https://neo4j.com/download/)  

```
"neo4JConnectionConfiguration": {
    "connectionString": "bolt://127.0.0.1:7687",
    "userName": "neo4j",
    "password": "password"
  }
```
Make sure you create a project in Neo4j using the above settings.  The following projects require Neo4J;
[PagesWebApp.AspNetCoreNeo4j](src/PagesWebApp.AspNetCoreNeo4j)  
[PagesWebApp.AspNetCoreNeo4j.Support](src/PagesWebApp.AspNetCoreNeo4j.Support)  


## IdentityServer4 Federated Gateways 
[PagesWebApp](src/PagesWebApp)  
This Federated Gateway is using the out-of-the-box framework provided by Microsoft.  

[PagesWebApp.AspNetCoreNeo4j](src/PagesWebApp.AspNetCoreNeo4j)  
This Federated Gateway is using Neo4j as its user store.  It also points to our Support IDP as an external IDP.  

## External IDP
[PagesWebApp.AspNetCoreNeo4j.Support](src/PagesWebApp.AspNetCoreNeo4j.Support)  
This IDP shares the same Neo4j user database as [PagesWebApp.AspNetCoreNeo4j](src/PagesWebApp.AspNetCoreNeo4j).  The support IDP is used by a support agent to ask many factor questions which results in a login.  The Support idp is recorded in the id_token so that the final clients know the difference between a user login vs on that was done over the phone.

Don't forget to add many factors to a user using the Federated Gateway [PagesWebApp.AspNetCoreNeo4j](src/PagesWebApp.AspNetCoreNeo4j);  

![Many Factor](./many-factor.png)

## Web Clients
[PagesWebAppClient](src/PagesWebAppClient)  
This is a full on asp.net 2.1 Identity app, which has its own user database.  This is what you would typically see pointing to Google, Twitter, and our own OIDC [PagesWebApp](src/PagesWebApp)  

[PagesWebAppClient-NoUserDatabase](src/PagesWebAppClient-NoUserDatabase)  
This is still using asp.net 2.1 Identity, however the user database is in-memory.  A little trick I do is during the [ExternalLogin Callback](src/PagesWebAppClient-NoUserDatabase/Areas/Identity/Pages/Account/ExternalLogin.cshtml.cs)  I create the user, sign them in, and then promptly delete the user.  I have found out that in the case of using an external system as my user database, that there is no need to call any UserManager stuff.  This one only points to our single OIDC [PagesWebApp](src/PagesWebApp)  

## Native Clients
[NetCoreConsoleClient](src/NetCoreConsoleClient)  
[WpfApp](src/WpfApp)  

# References
[learnrazorpages](https://www.learnrazorpages.com/razor-pages)  
[IdentityServer4.AspNetIdentity](https://github.com/IdentityServer/IdentityServer4.AspNetIdentity)  
[asp.net core 2.1](https://docs.microsoft.com/en-us/aspnet/core/aspnetcore-2.1?view=aspnetcore-2.1)  
[AspNetCore.Identity.Neo4j](https://github.com/dopare/AspNetCore.Identity.Neo4j)  


