using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DisCaddy
{
    public static class ConfigLoader
    {
        public static AppConfig Load()
        {
            string localPath = Path.Combine(FileSystem.AppDataDirectory, "config.json");
            if (File.Exists(localPath))
            {
                string fileJson = File.ReadAllText(localPath);
                return JsonSerializer.Deserialize<AppConfig>(fileJson)
                    ?? throw new Exception("Failed to deserialize local config.json.");
            }

            var assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "DisCaddy.config.json";

            using Stream? stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException($"Embedded config file '{resourceName}' not found.");
            using var reader = new StreamReader(stream);
            string json = reader.ReadToEnd();

            return JsonSerializer.Deserialize<AppConfig>(json)
                ?? throw new Exception("Failed to deserialize embedded config.json.");
        }
    }
}
