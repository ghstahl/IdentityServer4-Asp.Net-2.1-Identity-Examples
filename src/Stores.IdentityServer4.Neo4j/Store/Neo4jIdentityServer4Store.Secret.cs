using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver.V1;
using Neo4jExtras;
using Neo4jExtras.Extensions;

namespace Stores.IdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4Store<TUser> where TUser : Neo4jIdentityUser
    {
        public async Task<IdentityResult> AddSecretAsync(Neo4jIdentityServer4Client client, Secret secret,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var foundSecret = (client.ClientSecrets.Where(
                item => item.Type == secret.Type && item.Value == secret.Value)).FirstOrDefault();
            if (foundSecret == null)
            {
                client.ClientSecrets.Add(secret);
                return IdentityResult.Success;
            }
           return IdentityResult.Failed(new IdentityError()
           {
               Code = "Already.Exist",Description = "The secret already exists"
           });
        }

        public async Task<IdentityResult> RemoveSecretAsync(Neo4jIdentityServer4Client client, Secret secret,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var filtered = (client.ClientSecrets.Where(
                item => !(item.Type == secret.Type && item.Value == secret.Value))).ToList();
            client.ClientSecrets = filtered;
            return IdentityResult.Success;
        }

        public async Task<Secret> GetSecretAsync(Neo4jIdentityServer4Client client, Secret secret,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var foundSecret = (client.ClientSecrets.Where(
                item => item.Type == secret.Type && item.Value == secret.Value)).FirstOrDefault();
            return foundSecret;
        }

        public async Task<IList<Secret>> GetSecretsAsync(Neo4jIdentityServer4Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return client.ClientSecrets;
        }
    }
}