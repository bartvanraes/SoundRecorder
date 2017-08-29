using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundRecorder.Interfaces
{
    public interface IFTPService
    {
        Task<string> UploadFilesToFTPServer(string ftpUrl, Guid sessionId, string userName, string password, string uploadDirectory = "");
    }
}
