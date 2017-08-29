using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using SoundRecorder.Interfaces;
using SoundRecorder.Droid.Services;

[assembly: Xamarin.Forms.Dependency(typeof(FTPService))]

namespace SoundRecorder.Droid.Services
{
    public class FTPService : IFTPService
    {
        private static string localDirectory;
        public FTPService()
        {
            localDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        }

        public async Task<string> UploadFilesToFTPServer(string ftpUrl, Guid sessionId, string userName, string password, string uploadDirectory = "")
        {
            string xmlFilePath = "";
            string audioFileName = "";
            string audioFilePath = "";
            try
            {
                string xmlFileName = String.Format("{0}.xml", sessionId.ToString());
                audioFileName = String.Format("{0}.mp4", sessionId.ToString());
                xmlFilePath = Path.Combine(localDirectory, xmlFileName);
                audioFilePath = Path.Combine(localDirectory, audioFileName);


                //Upload XML
                string xmlUploadUrl = String.Format("{0}{1}/{2}", ftpUrl, uploadDirectory, xmlFileName);
                FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(xmlUploadUrl);
                req.Proxy = null;
                req.Method = WebRequestMethods.Ftp.UploadFile;
                req.Credentials = new NetworkCredential(userName, password);
                req.UseBinary = true;
                req.UsePassive = true;
                byte[] data = File.ReadAllBytes(xmlFilePath);
                req.ContentLength = data.Length;
                Stream stream = req.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();
                FtpWebResponse res = (FtpWebResponse)req.GetResponse();

                string audioUploadUrl = String.Format("{0}{1}/{2}", ftpUrl, uploadDirectory, audioFileName);
                req = (FtpWebRequest)FtpWebRequest.Create(audioUploadUrl);
                req.Proxy = null;
                req.Method = WebRequestMethods.Ftp.UploadFile;
                req.Credentials = new NetworkCredential(userName, password);
                req.UseBinary = true;
                req.UsePassive = true;
                data = File.ReadAllBytes(audioFilePath);
                req.ContentLength = data.Length;
                stream = req.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();
                res = (FtpWebResponse)req.GetResponse();

                return res.StatusDescription;

            }
            catch (Exception err)
            {
                return err.ToString();
            }
            finally
            {
                File.Delete(xmlFilePath);
                File.Delete(audioFilePath);

            }
        }
    }
}