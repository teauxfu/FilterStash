using System.Text.Json;

namespace FilterStash
{
    public static class Utils
    {
        static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        /// <summary>
        /// C:\Users\USER\Documents\My Games\Path of Exile 2
        /// </summary>
        public static readonly string DefaultFiltersPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "My Games",
            "Path of Exile 2"
        );

        /// <summary>
        /// C:\Users\USER\Documents\My Games\Path of Exile 2\FilterStash
        /// </summary>
        public static string DefaultCachePath
            => Path.Combine(DefaultFiltersPath, "FilterStash");

        /// <summary>
        /// C:\Users\USER\Documents\My Games\Path of Exile 2\FilterStash\index.json
        /// </summary>
        public readonly static string DefaultIndexPath = Path.Combine(
                    DefaultCachePath,
                    "index.json"
                );

        public static void CreateDefaultConfig(string? path = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = DefaultIndexPath;

            try
            {
                // HACK this approach doesn't gel well with 
                Directory.CreateDirectory(Directory.GetParent(path)!.FullName);
                Dictionary<string, PackageIndex> config = [];
                config[nameof(PackageIndex)] = new();
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
                path = DefaultIndexPath;

            return File.Exists(path)
                && JsonSerializer.Deserialize<PackageIndex>(File.ReadAllText(path), _jsonOptions) is not null;
        }

        public static string FormatFileSize(long bytes)
        {
            // If file size is less than 1KB
            if (bytes < 1024)
                return bytes + " B";

            // If file size is less than 1MB
            else if (bytes < 1024 * 1024)
                return (bytes / 1024.0).ToString("0.00") + " KB";

            // If file size is less than 1GB
            else if (bytes < 1024 * 1024 * 1024)
                return (bytes / 1024.0 / 1024.0).ToString("0.00") + " MB";

            // If file size is 1GB or more
            else
                return (bytes / 1024.0 / 1024.0 / 1024.0).ToString("0.00") + " GB";
        }

    }
}
