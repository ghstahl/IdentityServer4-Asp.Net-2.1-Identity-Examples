using Microsoft.Extensions.Configuration;

namespace IdentityServer4Extras.Extensions
{
    public static class AppSettingsExtensions
    {
        public static T FromSection<T>(this IConfiguration configuration,string sectionName) where T: class, new()
        {
            var sectionNeo4JConnectionConfiguration = configuration.GetSection(sectionName);
            var t = new T();
            var neo4JConnectionConfiguration = t;
            sectionNeo4JConnectionConfiguration.Bind(t);
            return t;
        }
    }
}
