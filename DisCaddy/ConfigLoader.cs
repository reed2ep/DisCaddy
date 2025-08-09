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
        public static async Task<AppConfig> LoadAsync()
        {
            var local = Path.Combine(FileSystem.AppDataDirectory, "config.json");
            if (File.Exists(local))
            {
                var txt = await File.ReadAllTextAsync(local);
                return JsonSerializer.Deserialize<AppConfig>(txt) ?? new();
            }

            using var s = await FileSystem.OpenAppPackageFileAsync("config.json");
            using var r = new StreamReader(s);
            var json = await r.ReadToEndAsync();
            return JsonSerializer.Deserialize<AppConfig>(json) ?? new();
        }
    }
}
