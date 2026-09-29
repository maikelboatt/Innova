using System.IO;
using System.Reflection;

namespace Innova.Infrastructure.Persistence
{
    public static class SqlLoader
    {
        private static readonly Dictionary<string, string> Cache = new();

        public static string Load( Assembly assembly, string resourceName )
        {
            if (Cache.TryGetValue(resourceName, out string? cached))
                return cached;

            using Stream? stream = assembly.GetManifestResourceStream(resourceName);

            if (stream is null)
            {
                string available = string.Join(
                    Environment.NewLine,
                    assembly.GetManifestResourceNames());

                throw new InvalidOperationException(
                    $"SQL resource '{resourceName}' not found. Available resources:{Environment.NewLine}{available}");
            }

            using StreamReader reader = new(stream);
            string sql = reader.ReadToEnd();

            Cache[resourceName] = sql;
            return sql;
        }
    }
}
