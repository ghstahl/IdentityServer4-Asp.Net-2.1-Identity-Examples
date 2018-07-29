using System;
using System.Threading.Tasks;
using AspNetCore.Identity.Neo4j;
using Neo4j.Driver.V1;

namespace AspNetCore.Identity.Neo4jExtras
{
    public class Neo4jTest :
        INeo4jTest
    {
        static Neo4jTest()
        {
            
        }
        public Neo4jTest(ISession session)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public ISession Session { get; set; }

        public async Task DropDatabaseAsync()
        {
            //MATCH (client)-[r]->() DELETE r;
            var cypher = @"MATCH (client)-[r]->() DELETE r";
            await Session.RunAsync(cypher);
            cypher = @"MATCH (n) DELETE n";
            await Session.RunAsync(cypher);

        }

        public async Task InitializeDatabaseAsync()
        {
          
        }
    }
}
