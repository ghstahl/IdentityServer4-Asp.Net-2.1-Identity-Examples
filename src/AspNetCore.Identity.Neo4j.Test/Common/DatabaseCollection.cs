using Xunit;

namespace AspNetCore.Identity.Neo4j.Test.Common
{
    [CollectionDefinition("Real Database")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}
