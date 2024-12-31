using Microsoft.Extensions.Logging;
using Octokit;

namespace PoE2FilterManager.Data
{
    public class SyncService
    {
        private readonly GitHubClient _client;
        private readonly ILogger<SyncService>? _log;

        public SyncService(ILogger<SyncService> log)
        {
            _client = new GitHubClient(new ProductHeaderValue("poe2-filter-manager"));
            _log = log;
        }


        public record PackageItem(string Name, string Url, string Sha, int Size);
        public async Task<PackageItem[]> GetFilterPackageAsync(string source)
        {
            try
            {
                // Parse the URL to get the repository owner and name
                Uri repoUri = new(source);
                string owner = repoUri.Segments[1].TrimEnd('/');
                string repoName = repoUri.Segments[2].TrimEnd('/');

                // Get the file content from the repository
                var contents = await _client.Repository.Content.GetAllContents(owner, repoName);
                return [.. contents.Select(s => new PackageItem(s.Name, s.Url, s.Sha, s.Size))];
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
