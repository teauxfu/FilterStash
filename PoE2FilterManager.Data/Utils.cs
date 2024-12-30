using PoE2FilterManager.Data;
using System.Text.Json;

namespace PoE2FilterManagerData
{
    public static class Utils
    {
        static JsonSerializerOptions _jsonOptions = new() { WriteIndented = true, ReadCommentHandling = JsonCommentHandling.Skip };
        public readonly static string DefaultSettingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    nameof(PoE2FilterManager),
                    "appsettings.json"
                );

        public static void CreateDefaultConfig(string? path = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    nameof(PoE2FilterManager),
                    "appsettings.json"
                );

            try
            {
                // HACK this approach doesn't gel well with 
                Directory.CreateDirectory(Directory.GetParent(path)!.FullName);
                Dictionary<string, AppSettings> config = [];
                config[nameof(AppSettings)] = new();
                File.WriteAllText(path, JsonSerializer.Serialize(config, _jsonOptions));
            }
            catch (Exception ex)
            {
                // todo complain, likely permissions error
                throw;
            }
        }

        public static bool ConfigIsValid(string? path = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = DefaultSettingsPath;

            return File.Exists(path)
                && JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(path), _jsonOptions) is not null;
        }
    }
}
