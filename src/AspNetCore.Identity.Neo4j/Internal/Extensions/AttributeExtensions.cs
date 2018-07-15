using System;

namespace AspNetCore.Identity.Neo4j.Internal.Extensions
{
    internal static class AttributeExtensions
    {
        public static string GetNeo4jLabelName(this Type entityType)
        {
            var labels = entityType.GetCustomAttributes(typeof(Neo4jLabelAttribute), true);
            if (labels?.Length != 1)
            {
                throw new InvalidProgramException($"Neo4jLabelAttribute is not found on {entityType.Name} class");
            }

            var label = (Neo4jLabelAttribute)labels[0];
            return label.LabelName;
        }
    }
}