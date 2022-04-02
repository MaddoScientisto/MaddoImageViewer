using MauiApp1Blazor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1Blazor.Services
{
    public interface IDbService
    {
        List<FileData> GetCollection(string path);
    }
}
