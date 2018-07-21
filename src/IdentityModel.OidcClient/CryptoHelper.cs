﻿using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography;
using System.Text;

namespace IdentityModel.OidcClient
{
    internal class CryptoHelper
    {
        private readonly ILogger _logger;
        private readonly OidcClientOptions _options;

        public CryptoHelper(OidcClientOptions options)
        {
            _options = options;
            _logger = options.LoggerFactory.CreateLogger<CryptoHelper>();
        }

        public HashAlgorithm GetMatchingHashAlgorithm(string signatureAlgorithm)
        {
            _logger.LogTrace("GetMatchingHashAlgorithm");
            _logger.LogDebug("Determining matching hash algorithm for {signatureAlgorithm}", signatureAlgorithm);

            var signingAlgorithmBits = int.Parse(signatureAlgorithm.Substring(signatureAlgorithm.Length - 3));
            
            switch (signingAlgorithmBits)
            {
                case 256:
                    _logger.LogDebug("SHA256");
                    return SHA256.Create();
                case 384:
                    _logger.LogDebug("SHA384");
                    return SHA384.Create();
                case 512:
                    _logger.LogDebug("SHA512");
                    return SHA512.Create();
                default:
                    return null;
            }
        }

        public bool ValidateHash(string data, string hashedData, string signatureAlgorithm)
        {
            _logger.LogTrace("ValidateHash");

            var hashAlgorithm = GetMatchingHashAlgorithm(signatureAlgorithm);
            if (hashAlgorithm == null)
            {
                _logger.LogError("No appropriate hashing algorithm found.");
            }

            using (hashAlgorithm)
            {
                var hash = hashAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(data));

                byte[] leftPart = new byte[hashAlgorithm.HashSize / 16];
                Array.Copy(hash, leftPart, hashAlgorithm.HashSize / 16);

                var leftPartB64 = Base64Url.Encode(leftPart);
                var match = leftPartB64.Equals(hashedData);

                if (!match)
                {
                    _logger.LogError($"data ({leftPartB64}) does not match hash from token ({hashedData})");
                }

                return match;
            }
        }

        public string CreateState()
        {
            _logger.LogTrace("CreateState");

            return CryptoRandom.CreateUniqueId(16);
        }

        public string CreateNonce()
        {
            _logger.LogTrace("CreateNonce");

            return CryptoRandom.CreateUniqueId(16);
        }

        public Pkce CreatePkceData()
        {
            _logger.LogTrace("CreatePkceData");

            var pkce = new Pkce
            {
                CodeVerifier = CryptoRandom.CreateUniqueId()
            };

            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pkce.CodeVerifier));
                pkce.CodeChallenge = Base64Url.Encode(challengeBytes);
            }

            return pkce;
        }

        internal class Pkce
        {
            public string CodeVerifier { get; set; }
            public string CodeChallenge { get; set; }
        }
    }
}