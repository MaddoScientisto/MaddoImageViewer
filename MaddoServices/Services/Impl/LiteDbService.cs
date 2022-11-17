using LiteDB;

using MaddoServices.Data;

namespace MaddoServices.Services.Impl
{
    public class LiteDbService : IDbService
    {
        public List<FileData> GetCollection(string path)
        {
            using var db = new LiteDatabase("images.db");
            var col = db.GetCollection<FileData>("Images");
            col.EnsureIndex(x => x.FolderPath);

            var data = col.Query()
                .Where(x => x.FolderPath == path)
                .OrderBy(x => x.FileName)
                //.Select(x => new )
                .ToList();

            return data;
        }

        public void RecordData(IEnumerable<FileData> data)
        {
            using var db = new LiteDatabase("images.db");
            var col = db.GetCollection<FileData>("Images");
            col.EnsureIndex(x => x.FolderPath);

            

            var retrievedData = col.Query()
                .Where(x => x.FolderPath == data.First().FolderPath)
                .OrderBy(x => x.FileName)                
                .ToList();


            var removedFiles = retrievedData.Where(x => !data.Any(x2 => x2.FullPath == x.FullPath));

            foreach (var file in removedFiles)
            {
                col.Delete(file.Id);
            }



        }

        
    }
}
