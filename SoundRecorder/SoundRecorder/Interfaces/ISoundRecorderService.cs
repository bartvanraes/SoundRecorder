using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundRecorder.Interfaces
{
    public interface ISoundRecorderService
    {
        Task StartRecordingAsync(Guid sessionId, RecordingStrategy strategy);
        void StopRecording(Guid sessionId);
    }

    public enum RecordingStrategy
    {
        AudioRecord,
        MediaRecorder
    }
}
