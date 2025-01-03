namespace PoE2FilterManager.Data.Services
{
    public interface ISyncService
    {
        Task<Package?> GetFilterPackageAsync(string name, string source);
    }
}