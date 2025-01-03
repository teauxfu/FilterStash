namespace PoE2FilterManager.Data.Services
{
    public interface IIndexService
    {
        PackageIndex ReadIndex();
        void SaveIndex(PackageIndex packageIndex);
    }
}