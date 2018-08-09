using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4Extras;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras;
using Neo4jExtras.Extensions;
using Stores.IdentityServer4Neo4j.Events;
using StoresIdentityServer4.Neo4j.DTO.Mappers;
using StoresIdentityServer4.Neo4j.Mappers;


namespace StoresIdentityServer4.Neo4j
{

    public partial class Neo4jIdentityServer4ClientUserStore<TUser> :
        IIdentityServer4ClientUserStore<
            TUser,
            Neo4jIdentityServer4Client,
            Neo4jIdentityServer4ClientSecret,
            Neo4jIdentityServer4ClientGrantType,
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
            Neo4jIdentityServer4IdentityClaim
        >
        where TUser : Neo4jIdentityUser
    {
        private bool _disposed;

        /// <summary>
        /// Dispose the stores
        /// </summary>
        public void Dispose() => _disposed = true;

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public static string User { get; set; }

        /// <summary>
        /// Gets the database session for this store.
        /// </summary>
        public ISession Session { get; }

        private INeo4jEventService _eventService;

        static Neo4jIdentityServer4ClientUserStore()
        {
            User = typeof(TUser).GetNeo4jLabelName();

            IdSrv4ApiResource = typeof(Neo4jIdentityServer4ApiResource).GetNeo4jLabelName();
            IdSrv4ApiResourceClaim = typeof(Neo4jIdentityServer4ApiResourceClaim).GetNeo4jLabelName();
            IdSrv4ClientApiScope = typeof(Neo4jIdentityServer4ApiScope).GetNeo4jLabelName();
            IdSrv4ClientApiScopeClaim = typeof(Neo4jIdentityServer4ApiScopeClaim).GetNeo4jLabelName();
            IdSrv4ApiSecret = typeof(Neo4jIdentityServer4ApiSecret).GetNeo4jLabelName();

            IdSrv4ClientRollup = typeof(Neo4jIdentityServer4ClientRollup).GetNeo4jLabelName();
            IdSrv4ApiScopeRollup = typeof(Neo4jIdentityServer4ApiScopeRollup).GetNeo4jLabelName();
            IdSrv4ApiResourceRollup = typeof(Neo4jIdentityServer4ApiResourceRollup).GetNeo4jLabelName();

            IdSrv4Client = typeof(Neo4jIdentityServer4Client).GetNeo4jLabelName();
            IdSrv4ClientSecret = typeof(Neo4jIdentityServer4ClientSecret).GetNeo4jLabelName();
            IdSrv4ClientGrantType = typeof(Neo4jIdentityServer4ClientGrantType).GetNeo4jLabelName();
            IdSrv4ClientClaim = typeof(Neo4jIdentityServer4ClientClaim).GetNeo4jLabelName();
            IdSrv4ClientCorsOrigin = typeof(Neo4jIdentityServer4ClientCorsOrigin).GetNeo4jLabelName();
            IdSrv4ClientScope = typeof(Neo4jIdentityServer4ClientScope).GetNeo4jLabelName();
            IdSrv4ClienTIDPRestriction = typeof(Neo4JIdentityServer4ClientIdpRestriction).GetNeo4jLabelName();
            IdSrv4ClientProperty = typeof(Neo4jIdentityServer4ClientProperty).GetNeo4jLabelName();
            IdSrv4ClientPostLogoutRedirectUri =
                typeof(Neo4jIdentityServer4ClientPostLogoutRedirectUri).GetNeo4jLabelName();
            IdSrv4ClientRedirectUri = typeof(Neo4jIdentityServer4ClientRedirectUri).GetNeo4jLabelName();

            IdSrv4IdentityResource = typeof(Neo4jIdentityServer4IdentityResource).GetNeo4jLabelName();
            IdSrv4IdentityClaim = typeof(Neo4jIdentityServer4IdentityClaim).GetNeo4jLabelName();
            IdSrv4IdentityResourceRollup = typeof(Neo4jIdentityServer4IdentityResourceRollup).GetNeo4jLabelName();

        }
        public Neo4jIdentityServer4ClientUserStore(
            ISession session,
            INeo4jEventService eventService)
        {
            Session = session;
            _eventService = eventService;
        }

        private Task RaiseClientChangeEventAsync(
            Neo4jIdentityServer4Client client)
        {
            return _eventService.RaiseAsync(new ClientChangeEvent<Neo4jIdentityServer4Client>(client));
        }


        public async Task<IdentityResult> EnsureStandardAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var identityResources = new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Address(),
                new IdentityResources.Phone(),
            };

            return await InsertIdentityResources(identityResources, cancellationToken);
        }

        public async Task<IdentityResult> InsertIdentityResource(
            IdentityResource model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            model.ThrowIfNull(nameof(model));
            try
            {
                var dto = model.ToNeo4jEntity();
                var result = await CreateIdentityResourceAsync(dto);
                if (!result.Succeeded)
                    return result;
                foreach (var claim in model.UserClaims)
                {
                    var dtoClaim = new Neo4jIdentityServer4IdentityClaim()
                    {
                        Type = claim
                    };
                    result = await AddIdentityClaimAsync(dto, dtoClaim);
                    if (!result.Succeeded)
                        return result;
                }

                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> InsertIdentityResources(
            IEnumerable<IdentityResource> models,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            models.ThrowIfNull(nameof(models));
            foreach (var model in models)
            {
                var result = await InsertIdentityResource(model, cancellationToken);
                if (!result.Succeeded)
                    return result;
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> InsertApiResource(
            ApiResource model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            model.ThrowIfNull(nameof(model));
            try
            {
                var dto = model.ToNeo4jEntity();
                var result = await CreateApiResourceAsync(dto, cancellationToken);
                if (!result.Succeeded)
                    return result;

                foreach (var claim in model.UserClaims)
                {
                    var dtoClaim = new Neo4jIdentityServer4ApiResourceClaim()
                    {
                        Type = claim
                    };
                    result = await AddApiResourceClaimAsync(dto, dtoClaim, cancellationToken);
                    if (!result.Succeeded)
                        return result;
                }

                foreach (var scope in model.Scopes)
                {
                    var dtoScope = scope.ToNeo4jEntity();
                    result = await AddApiScopeAsync(dto, dtoScope, cancellationToken);
                    if (!result.Succeeded)
                        return result;
                    foreach (var claim in scope.UserClaims)
                    {
                        var dtoClaim = new Neo4jIdentityServer4ApiScopeClaim()
                        {
                            Type = claim
                        };
                        result = await AddApiScopeClaimAsync(dto, dtoScope, dtoClaim, cancellationToken);
                        if (!result.Succeeded)
                            return result;
                    }
                }

                foreach (var apiSecret in model.ApiSecrets)
                {
                    var dtoSecret = apiSecret.ToNeo4jApiSecretEntity();
                    result = await AddApiSecretAsync(dto, dtoSecret, cancellationToken);
                    if (!result.Succeeded)
                        return result;
                }

                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }
        }

        public async Task<IdentityResult> InsertApiResources(
            IEnumerable<ApiResource> models,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            models.ThrowIfNull(nameof(models));
            foreach (var model in models)
            {
                var result = await InsertApiResource(model, cancellationToken);
                if (!result.Succeeded)
                    return result;
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> InsertClient(
            TUser user,
            ClientExtra model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            model.ThrowIfNull(nameof(model));
            try
            {
                var dto = model.ToNeo4jEntity();
                var result = await AddClientToUserAsync(user, dto, cancellationToken);
                if (!result.Succeeded)
                    return result;
                foreach (var mod in model.Claims)
                {
                    var dtoClaim = mod.ToNeo4jEntity();
                    result = await AddClaimToClientAsync(dto, dtoClaim, cancellationToken);
                    if (!result.Succeeded)
                        return result;
                }
                foreach (var mod in model.AllowedScopes)
                {
                    var dtoScope = mod.ToNeo4jClientScopeEntity();
                    result = await AddScopeToClientAsync(dto, dtoScope, cancellationToken);
                    if (!result.Succeeded)
                        return result;

                }
                foreach (var mod in model.ClientSecrets)
                {
                    var dtoSecret = mod.ToNeo4jClientSecretEntity();
                    result = await AddSecretToClientAsync(dto, dtoSecret, cancellationToken);
                    if (!result.Succeeded)
                        return result;
                }
                foreach (var mod in model.IdentityProviderRestrictions)
                {
                    var dtoIdentityProviderRestrictions = mod.ToNeo4JClientIdpRestrictionEntity();
                    result = await AddIdPRestrictionToClientAsync(dto, dtoIdentityProviderRestrictions, cancellationToken);
                    if (!result.Succeeded)
                        return result;
                }
                foreach (var mod in model.Properties)
                {
                  
                    var propertyDTO = mod.ToNeo4jClientPropertyEntity();
                    result = await AddPropertyToClientAsync(dto, propertyDTO, cancellationToken);
                    if (!result.Succeeded)
                        return result;
                }
                foreach (var mod in model.AllowedCorsOrigins)
                {

                    var allowedCorsOriginsDTO = mod.ToNeo4jClientCorsOriginEntity();
                    result = await AddCorsOriginToClientAsync(dto, allowedCorsOriginsDTO, cancellationToken);
                    if (!result.Succeeded)
                        return result;
                }
                foreach (var mod in model.AllowedGrantTypes)
                {
                    var allowedGrantTypesDTO = mod.ToNeo4jClientAllowedGrantTypeEntity();
                    result = await AddAllowedGrantTypeToClientAsync(dto, allowedGrantTypesDTO, cancellationToken);
                    if (!result.Succeeded)
                        return result;
                }
                foreach (var mod in model.PostLogoutRedirectUris)
                {
                    var postLogoutRedirectUrisDTO = mod.ToNeo4jClientPostLogoutRedirectUriEntity();
                    result = await AddPostLogoutRedirectUriToClientAsync(dto, postLogoutRedirectUrisDTO, cancellationToken);
                    if (!result.Succeeded)
                        return result;
                }
                foreach (var mod in model.RedirectUris)
                {
                    var redirectUriDTO = mod.ToNeo4jClientRedirectUriEntity();
                    result = await AddRedirectUriToClientAsync(dto, redirectUriDTO, cancellationToken);
                    if (!result.Succeeded)
                        return result;
                }
                

                return IdentityResult.Success;
            }
            catch (ClientException ex)
            {
                return ex.ToIdentityResult();
            }

            //            AddClientToUserAsync

        }

        public async Task<IdentityResult> InsertClients(TUser user, IEnumerable<ClientExtra> models,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            models.ThrowIfNull(nameof(models));
            foreach (var model in models)
            {
                var result = await InsertClient(user, model, cancellationToken);
                if (!result.Succeeded)
                    return result;
            }

            return IdentityResult.Success;
        }


    }
}