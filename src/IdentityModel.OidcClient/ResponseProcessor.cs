﻿using IdentityModel.Client;
using IdentityModel.OidcClient.Infrastructure;
using IdentityModel.OidcClient.Results;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityModel.OidcClient
{
    internal class ResponseProcessor
    {
        private readonly OidcClientOptions _options;
        private TokenClient _tokenClient;
        private ILogger<ResponseProcessor> _logger;
        private readonly IdentityTokenValidator _tokenValidator;
        private readonly CryptoHelper _crypto;
        private readonly Func<Task> _refreshKeysAsync;

        public ResponseProcessor(OidcClientOptions options, Func<Task> refreshKeysAsync)
        {
            _options = options;
            _refreshKeysAsync = refreshKeysAsync;
            _logger = options.LoggerFactory.CreateLogger<ResponseProcessor>();

            _tokenValidator = new IdentityTokenValidator(options, refreshKeysAsync);
            _crypto = new CryptoHelper(options);
        }

        public async Task<ResponseValidationResult> ProcessResponseAsync(AuthorizeResponse authorizeResponse, AuthorizeState state, object extraParameters)
        {
            _logger.LogTrace("ProcessResponseAsync");

            //////////////////////////////////////////////////////
            // validate common front-channel parameters
            //////////////////////////////////////////////////////

            if (string.IsNullOrEmpty(authorizeResponse.Code))
            {
                return new ResponseValidationResult("Missing authorization code.");
            }

            if (string.IsNullOrEmpty(authorizeResponse.State))
            {
                return new ResponseValidationResult("Missing state.");
            }

            if (!string.Equals(state.State, authorizeResponse.State, StringComparison.Ordinal))
            {
                return new ResponseValidationResult("Invalid state.");
            }

            switch (_options.Flow)
            {
                case OidcClientOptions.AuthenticationFlow.AuthorizationCode:
                    return await ProcessCodeFlowResponseAsync(authorizeResponse, state, extraParameters);
                case OidcClientOptions.AuthenticationFlow.Hybrid:
                    return await ProcessHybridFlowResponseAsync(authorizeResponse, state, extraParameters);
                default:
                    throw new ArgumentOutOfRangeException(nameof(_options.Flow), "Invalid authentication style.");
            }
        }

        private async Task<ResponseValidationResult> ProcessHybridFlowResponseAsync(AuthorizeResponse authorizeResponse, AuthorizeState state, object extraParameters = null)
        {
            _logger.LogTrace("ProcessHybridFlowResponseAsync");

            //////////////////////////////////////////////////////
            // validate front-channel response
            //////////////////////////////////////////////////////

            // id_token must be present
            if (authorizeResponse.IdentityToken.IsMissing())
            {
                return new ResponseValidationResult("Missing identity token.");
            }

            // id_token must be valid
            var frontChannelValidationResult = await _tokenValidator.ValidateAsync(authorizeResponse.IdentityToken);
            if (frontChannelValidationResult.IsError)
            {
                return new ResponseValidationResult(frontChannelValidationResult.Error ?? "Identity token validation error.");
            }

            // nonce must be valid
            if (!ValidateNonce(state.Nonce, frontChannelValidationResult.User))
            {
                return new ResponseValidationResult("Invalid nonce.");
            }

            // validate c_hash
            var cHash = frontChannelValidationResult.User.FindFirst(JwtClaimTypes.AuthorizationCodeHash);
            if (cHash == null)
            {
                if (_options.Policy.RequireAuthorizationCodeHash)
                {
                    return new ResponseValidationResult("c_hash is missing.");
                }
            }
            else
            {
                if (!_crypto.ValidateHash(authorizeResponse.Code, cHash.Value, frontChannelValidationResult.SignatureAlgorithm))
                {
                    return new ResponseValidationResult("Invalid c_hash.");
                }
            }

            //////////////////////////////////////////////////////
            // process back-channel response
            //////////////////////////////////////////////////////

            // redeem code for tokens
            var tokenResponse = await RedeemCodeAsync(authorizeResponse.Code, state, extraParameters);
            if (tokenResponse.IsError)
            {
                return new ResponseValidationResult(tokenResponse.Error);
            }

            // validate token response
            var tokenResponseValidationResult = await ValidateTokenResponseAsync(tokenResponse, state);
            if (tokenResponseValidationResult.IsError)
            {
                return new ResponseValidationResult(tokenResponseValidationResult.Error);
            }

            // compare front & back channel subs
            var frontChannelSub = frontChannelValidationResult.User.FindFirst(JwtClaimTypes.Subject).Value;
            var backChannelSub = tokenResponseValidationResult.IdentityTokenValidationResult.User.FindFirst(JwtClaimTypes.Subject).Value;

            if (!string.Equals(frontChannelSub, backChannelSub, StringComparison.Ordinal))
            {
                return new ResponseValidationResult($"Subject on front-channel ({frontChannelSub}) does not match subject on back-channel ({backChannelSub}).");
            }

            return new ResponseValidationResult
            {
                AuthorizeResponse = authorizeResponse,
                TokenResponse = tokenResponse,
                User = tokenResponseValidationResult.IdentityTokenValidationResult.User
            };
        }

        private async Task<ResponseValidationResult> ProcessCodeFlowResponseAsync(AuthorizeResponse authorizeResponse, AuthorizeState state, object extraParameters)
        {
            _logger.LogTrace("ProcessCodeFlowResponseAsync");
            
            //////////////////////////////////////////////////////
            // process back-channel response
            //////////////////////////////////////////////////////

            // redeem code for tokens
            var tokenResponse = await RedeemCodeAsync(authorizeResponse.Code, state, extraParameters);
            if (tokenResponse.IsError)
            {
                return new ResponseValidationResult($"Error redeeming code: {tokenResponse.Error ?? "no error code"} / {tokenResponse.ErrorDescription ?? "no description"}");
            }

            // validate token response
            var tokenResponseValidationResult = await ValidateTokenResponseAsync(tokenResponse, state);
            if (tokenResponseValidationResult.IsError)
            {
                return new ResponseValidationResult($"Error validating token response: {tokenResponseValidationResult.Error}");
            }

            return new ResponseValidationResult
            {
                AuthorizeResponse = authorizeResponse,
                TokenResponse = tokenResponse,
                User = tokenResponseValidationResult.IdentityTokenValidationResult.User
            };
        }

        internal async Task<TokenResponseValidationResult> ValidateTokenResponseAsync(TokenResponse response, AuthorizeState state, bool requireIdentityToken = true)
        {
            _logger.LogTrace("ValidateTokenResponse");
            
            // token response must contain an access token
            if (response.AccessToken.IsMissing())
            {
                return new TokenResponseValidationResult("Access token is missing on token response.");
            }

            if (requireIdentityToken)
            {
                // token response must contain an identity token (openid scope is mandatory)
                if (response.IdentityToken.IsMissing())
                {
                    return new TokenResponseValidationResult("Identity token is missing on token response.");
                }
            }

            if (response.IdentityToken.IsPresent())
            {
                // if identity token is present, it must be valid
                var validationResult = await _tokenValidator.ValidateAsync(response.IdentityToken);
                if (validationResult.IsError)
                {
                    return new TokenResponseValidationResult(validationResult.Error ?? "Identity token validation error");
                }

                // validate nonce
                if (state != null)
                {
                    if (!ValidateNonce(state.Nonce, validationResult.User))
                    {
                        return new TokenResponseValidationResult("Invalid nonce.");
                    }
                }

                // validate at_hash
                var atHash = validationResult.User.FindFirst(JwtClaimTypes.AccessTokenHash);
                if (atHash == null)
                {
                    if (_options.Policy.RequireAccessTokenHash)
                    {
                        return new TokenResponseValidationResult("at_hash is missing.");
                    }
                }
                else
                {
                    if (!_crypto.ValidateHash(response.AccessToken, atHash.Value, validationResult.SignatureAlgorithm))
                    {
                        return new TokenResponseValidationResult("Invalid access token hash.");
                    }
                }

                return new TokenResponseValidationResult(validationResult);
            }

            return new TokenResponseValidationResult((IdentityTokenValidationResult)null);
        }

        private bool ValidateNonce(string nonce, ClaimsPrincipal user)
        {
            _logger.LogTrace("ValidateNonce");

            var tokenNonce = user.FindFirst(JwtClaimTypes.Nonce)?.Value ?? "";
            var match = string.Equals(nonce, tokenNonce, StringComparison.Ordinal);

            if (!match)
            {
                _logger.LogError($"nonce ({nonce}) does not match nonce from token ({tokenNonce})");
            }

            return match;
        }

        private async Task<TokenResponse> RedeemCodeAsync(string code, AuthorizeState state, object extraParameters)
        {
            _logger.LogTrace("RedeemCodeAsync");

            var client = GetTokenClient();

            var tokenResult = await client.RequestAuthorizationCodeAsync(
                code,
                state.RedirectUri,
                codeVerifier: state.CodeVerifier,
                extra: extraParameters);

            return tokenResult;
        }

        private TokenClient GetTokenClient()
        {
            if (_tokenClient == null)
            {
                _tokenClient = TokenClientFactory.Create(_options);
            }

            return _tokenClient;
        }
    }
}