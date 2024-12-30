using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoE2FilterManager.Data
{
    public record Package(string Name, string Source);
    public class AppSettings
    {
        /// <summary>
        /// C:\Users\USER\Documents\My Games\Path of Exile 2
        /// </summary>
        private static string _defaultPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "My Games",
            "Path of Exile 2"
        );
        
        public string FiltersPath { get; set; } = _defaultPath;

        public List<Package> Packages { get; set; } = [];
    }
}
