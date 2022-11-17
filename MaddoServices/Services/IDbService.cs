using MaddoServices.Data;

namespace MaddoServices.Services
{
    public interface IDbService
    {
        List<FileData> GetCollection(string path);
    }
}
