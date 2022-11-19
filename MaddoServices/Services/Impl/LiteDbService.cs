using LiteDB;

using MaddoServices.Data;
using System.Diagnostics;

namespace MaddoServices.Services.Impl
{
    public class LiteDbService : IDbService
    {       

        public FileData GetFileData(string dbPath, string path)
        {
            using var db = new LiteDatabase(dbPath);
            var col = db.GetCollection<FileData>("Images");
            col.EnsureIndex(x => x.FullPath);

            var data = col.Query()
                .Where(x => x.FullPath == path).FirstOrDefault();

            return data;
        }

        public List<FileData> GetCollection(string dbPath, string path)
        {
            using var db = new LiteDatabase(dbPath);
            var col = db.GetCollection<FileData>("Images");
            col.EnsureIndex(x => x.FullPath);

            var data = col.Query()
                .Where(x => x.FullPath == path)
                .OrderBy(x => x.FileName)
                //.Select(x => new )
                .ToList();

            return data;
        }

        public void RecordData(string dbPath, IEnumerable<FileData> data)
        {
            using var db = new LiteDatabase(dbPath);
            var col = db.GetCollection<FileData>("Images");
            col.EnsureIndex(x => x.FullPath);

            

            var retrievedData = col.Query()
                .Where(x => x.FullPath == data.First().FullPath)
                .OrderBy(x => x.FileName)                
                .ToList();


            var removedFiles = retrievedData.Where(x => !data.Any(x2 => x2.FullPath == x.FullPath));

            foreach (var file in removedFiles)
            {
                col.Delete(file.FullPath);
            }



        }

        public void Save(string dbPath, FileData data)
        {
            using var db = new LiteDatabase(dbPath);
            var col = db.GetCollection<FileData>("Images");
            col.EnsureIndex(x => x.FullPath);

            var succ = col.Upsert(data);

            if (succ)
            {
                Debug.WriteLine($"Saved {data.FullPath}");
            }
            else
            {
                Debug.WriteLine($"Problem with {data.FullPath}");
            }
        }

        
    }
}
