namespace PoE2FilterManager.Data
{
    public record Package(string Name, string Source);
    public class AppSettings
    {
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
        public static string DefaultCachePath => Path.Combine(DefaultCachePath, "FilterStash");

        public string FiltersPath { get; set; } = DefaultFiltersPath;

        public List<Package> Packages { get; set; } = [];
    }
}
