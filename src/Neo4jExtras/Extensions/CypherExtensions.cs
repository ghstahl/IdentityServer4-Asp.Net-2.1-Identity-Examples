using System.Reflection;
using System.Text;

namespace Neo4jExtras.Extensions
{
    public static class CypherExtensions
    {
        public static string AsMapFor<T>(this string paramName)
        {
            var builder = new StringBuilder();

            builder.Append('{');

            foreach (var runtimeProperty in typeof(T).GetRuntimeProperties())
            {
                builder.AppendFormat("{0}: {1}.{0}", runtimeProperty.Name, paramName);
                builder.Append(", ");
            }

            builder.Remove(builder.Length - 2, 2);

            builder.Append('}');

            return builder.ToString();
        }
    }
}
