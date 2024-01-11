using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneDriveNuget
{
    public interface IOneDriveConnect
    {
        Task<string> getUploadFileUrl(string filePath, string UploadFolderName);
    }
}
