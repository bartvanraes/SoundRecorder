using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundRecorder.Interfaces
{
    public interface IEmailService
    {
        void SendEmailWithAttachment(string emailAddress, string text, string subject, string fileName);
    }
}
