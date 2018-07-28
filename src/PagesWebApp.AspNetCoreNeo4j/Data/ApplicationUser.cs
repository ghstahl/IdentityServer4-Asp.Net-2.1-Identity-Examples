using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using Neo4jExtras;

namespace Microsoft.AspNetCore.Identity
{
    public class ApplicationUser : Neo4jIdentityUser
    {
    }

    [Neo4jLabel("ApplicationFactor")]
    public class ApplicationFactor : ChallengeFactor
    {
       
    }
}
