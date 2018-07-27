using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Identity.Neo4j
{
    /*
      "neo4j": {
        "connectionString": "bolt://127.0.0.1:7687",
        "userName": "neo4j",
        "password": "password"
      }
     */
    public class Neo4JConnectionConfiguration
    {
        public string ConnectionString { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
