using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Octokit;

namespace PoE2FilterManager.Data.Services
{
    public class GitHubSyncService(ILogger<GitHubSyncService>? log = null) : ISyncService
    {
        private readonly GitHubClient _client = new(new ProductHeaderValue("poe2-filter-manager"));

        public async Task<Package?> GetFilterPackageAsync(string name, string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;

            try
            {
                // Parse the URL to get the repository owner and name
                Uri repoUri = new(source);
                string owner = repoUri.Segments[1].TrimEnd('/');
                string repoName = repoUri.Segments[2].TrimEnd('/');

                // Get the latest commit
                var commits = await _client.Repository.Commit.GetAll(owner, repoName);
                var latestCommit = commits.Count > 0 ? commits[0] : null;
                if (latestCommit == null)
                {
                    log?.LogError("No commits found for the repository {source}", source);
                    return null;
                }

                // Get the file content from the repository
                var contents = await _client.Repository.Content.GetAllContents(owner, repoName);
                string html = await _client.Repository.Content.GetReadmeHtml(owner, repoName);

                List<PackageItem> items = [];
                foreach (var content in contents)
                {
                    items.Add(new PackageItem(content.Name, content.HtmlUrl, content.DownloadUrl, content.Sha, content.Size, content.EncodedContent));
                }
                return new Package(name, source)
                {
                    ReadMeHtml = html,
                    Items = items,
                    LastUpdated = DateTimeOffset.Now,
                    LastCommitSha = latestCommit.Sha,
                    LastCommitDate = latestCommit.Commit.Committer.Date
                };
            }
            catch (NotFoundException ex)
            {
                log?.LogError(ex, "Content for the given source {source} could not be found", source);
                return null;
            }
            catch (Exception ex)
            {
                log?.LogError(ex, "An unexpected error occurred while getting package contents for {source}", source);
                throw;
            }
        }
    }
}
