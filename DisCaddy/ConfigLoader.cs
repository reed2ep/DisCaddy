using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DisCaddy.Models;

namespace DisCaddy
{
    public static class ConfigLoader
    {
        public static AppConfig Load()
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, "config.json");

            if (!File.Exists(path))
            {
                // fallback to bundled file
                var assembly = Assembly.GetExecutingAssembly();
                using var stream = assembly.GetManifestResourceStream("DisCaddy.config.json");
                using var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();
                return JsonSerializer.Deserialize<AppConfig>(json);
            }

            string fileJson = File.ReadAllText(path);
            return JsonSerializer.Deserialize<AppConfig>(fileJson);
        }
    }
}
