using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaddoServices.Services
{
    public interface IFolderPicker
    {
        Task<string> PickFolder();
    }
}
