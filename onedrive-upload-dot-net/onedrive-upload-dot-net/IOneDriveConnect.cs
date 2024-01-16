using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onedrive_upload_dot_net 
{ 
    public interface IOneDriveConnect
    {
        Task<string> getUploadFileUrl(string filePath, string UploadFolderName);
    }
}
