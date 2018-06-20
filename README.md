# IdentityServer4 Asp.Net 2.1 Identity Examples

[IdentityServer4 Issue](https://github.com/IdentityServer/IdentityServer4/issues/2373)  

All these examples are based on the following;  
  * asp.net 2.1 WebApp  
  * asp.net identity  

## The IdentityServer4 Host
[PagesWebApp](src/PagesWebApp)  

## Clients
[PagesWebAppClient](src/PagesWebAppClient)  
This is a full on asp.net 2.1 Identity app, which has its own user database.  This is what you would typically see pointing to Google, Twitter, and our own OIDC [PagesWebApp](src/PagesWebApp)  

[PagesWebAppClient-NoUserDatabase](src/PagesWebAppClient-NoUserDatabase)  
This is still using asp.net 2.1 Identity, however the user database is in-memory.  A little trick I do is during the [ExternalLogin Callback](src/PagesWebAppClient-NoUserDatabase/Areas/Identity/Pages/Account/ExternalLogin.cshtml.cs)  I create the user, sign them in, and then promptly delete the user.  I have found out that in the case of using an external system as my user database, that there is no need to call any UserManager stuff.  This one only points to our single OIDC [PagesWebApp](src/PagesWebApp)  
