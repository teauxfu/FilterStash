using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace PoE2FilterManager.Data.Services
{
    /// <summary>
    /// Just something quick and dirty, with user-friendly encoding. 
    /// </summary>
    /// <remarks>consider not exposing the index object directly?</remarks>
    public class JsonIndexService : IIndexService
    {
        readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
        };
        private readonly string _indexPath;
        private readonly ILogger<JsonIndexService>? _log;

        public JsonIndexService(string indexPath, ILogger<JsonIndexService>? log = null)
        {
            _indexPath = indexPath;
            _log = log;

            if (!File.Exists(indexPath))
                File.WriteAllText(indexPath, JsonSerializer.Serialize(new PackageIndex(), jsonSerializerOptions));
        }


        public void SaveIndex(PackageIndex packageIndex)
        {
            string json = JsonSerializer.Serialize(packageIndex, jsonSerializerOptions);
            File.WriteAllText(_indexPath, json);
            _log?.LogInformation("saved index at {path}", _indexPath);
        }

        public PackageIndex ReadIndex()
        {
            
            if (JsonSerializer.Deserialize<PackageIndex>(File.ReadAllText(_indexPath)) is PackageIndex index)
                return index;

            var e = new Exception($"Could not read an app config from {_indexPath}");
            _log?.LogError(e, "An error occurred while reading the index");
            throw e;
        }
    }
}
