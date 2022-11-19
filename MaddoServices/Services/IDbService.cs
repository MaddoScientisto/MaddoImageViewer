using MaddoServices.Data;

namespace MaddoServices.Services
{
    public interface IDbService
    {
        List<FileData> GetCollection(string dbPath, string path);
        FileData GetFileData(string dbPath, string path);

        void Save(string dbPath, FileData data);
    }
}
