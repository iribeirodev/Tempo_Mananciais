using System;
using System.IO;
using System.Reflection;

namespace Infrastructure.Crosscutting
{
    public class DotEnv
    {
        private static string GetEnvLocation()
        {
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(currentDir, ".env");
        }


        public static void Load()
        {
            var filePath = GetEnvLocation();

            if (!File.Exists(filePath))
                return;

            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var parts = line.Split('=',StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    continue;

                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
        }
    }
}
