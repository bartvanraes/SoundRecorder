using SoundRecorder.DTO.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundRecorder.Interfaces
{
    public interface IFileService
    {
        string WriteFile(Guid sessionId, IList<ImuDTO> imuList);
    }
}
