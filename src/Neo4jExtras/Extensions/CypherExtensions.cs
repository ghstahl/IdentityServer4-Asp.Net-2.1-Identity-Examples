using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Neo4jExtras.Extensions
{
    public static class CypherExtensions
    {
        public static string AsMapFor<T>(this string paramName)
        {
            var builder = new StringBuilder();

            builder.Append('{');
            var runtimeProperties = typeof(T).GetRuntimeProperties()
                .Where(pi => !pi.GetCustomAttributes<JsonIgnoreAttribute>(true).Any());

            foreach (var runtimeProperty in runtimeProperties)
            {
                builder.AppendFormat("{0}: {1}.{0}", runtimeProperty.Name, paramName);
                builder.Append(", ");
            }

            builder.Remove(builder.Length - 2, 2);

            builder.Append('}');

            var result = builder.ToString();
            return result;
        }
        public static string AsMapForNoNull<T>(this string paramName, T obj)
        {
            var builder = new StringBuilder();

            var nonNullProperties = typeof(T).GetRuntimeProperties()
                .Select(x => new
                {
                    pi = x,
                    property = x.Name,
                    value = x.GetValue(obj)
                })
                .Where(x => (x.value != null) && !x.pi.GetCustomAttributes<JsonIgnoreAttribute>(true).Any())
                .ToList();

            builder.Append('{');
      
            foreach (var runtimeProperty in nonNullProperties)
            {
                builder.AppendFormat("{0}: {1}.{0}", runtimeProperty.pi.Name, paramName);
                builder.Append(", ");
            }

            builder.Remove(builder.Length - 2, 2);

            builder.Append('}');

            var result = builder.ToString();
            return result;
        }
    }
}
