using FilterStash;

namespace FilterStash.Services
{
    public interface ISyncService
    {
        Task<Package?> GetFilterPackageAsync(string name, string source);
    }
}