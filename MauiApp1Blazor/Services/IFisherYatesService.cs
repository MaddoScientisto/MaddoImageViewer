using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1Blazor.Services
{
    public interface IFisherYatesService
    {
        string Shuffle(string input, int? seed = null);
        void Shuffle<T>(T[] array, int? seed = null);
    }
}
