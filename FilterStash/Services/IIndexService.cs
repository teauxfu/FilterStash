using FilterStash;

namespace FilterStash.Services
{
    public interface IIndexService
    {
        PackageIndex ReadIndex();
        void SaveIndex(PackageIndex packageIndex);
    }
}