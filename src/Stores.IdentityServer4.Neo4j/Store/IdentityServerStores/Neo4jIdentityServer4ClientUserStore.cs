using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using IdentityServer4.Models;

namespace StoresIdentityServer4.Neo4j
{
    public partial class Neo4jIdentityServer4ClientUserStore<TUser> where TUser : Neo4jIdentityUser
    {
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = await GetRollupAsync(new Neo4jIdentityServer4Client()
            {
                ClientId = clientId
            });
            return client;
        }
    }
}

