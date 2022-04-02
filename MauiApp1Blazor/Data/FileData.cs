using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1Blazor.Data
{
    public class FileData
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FolderPath { get; set; }
        public string FullPath { get; set; }

        public DateTime LastSeen { get; set; }

        public int ViewdCount { get; set; }
    }
}
