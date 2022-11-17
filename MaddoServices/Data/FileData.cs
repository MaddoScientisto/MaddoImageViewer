namespace MaddoServices.Data
{
    public class FileData
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FolderPath { get; set; }
        public string FullPath { get; set; }

        public DateTime Date { get; set; }
        public DateTime LastSeen { get; set; }

        public int ViewdCount { get; set; }
    }
}
