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
using SoundRecorder.DTO.Files;
using SoundRecorder.Interfaces;
using SoundRecorder.Droid.Services;
using System.IO;
using System.Xml.Serialization;

[assembly: Xamarin.Forms.Dependency(typeof(FileService))]
namespace SoundRecorder.Droid.Services
{
    public class FileService : IFileService
    {
        public string WriteFile(string fileName, IList<ImuDTO> imuList)
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, fileName);

            XmlSerializer serializer = new XmlSerializer(imuList.GetType());
            using (StreamWriter outputFile = new StreamWriter(filePath, true))
            {
                //outputFile.WriteAsync(serializer.s)
                serializer.Serialize(outputFile, imuList);
            }

            return filePath;

            // TEST: Send an email
            

            /*var email = new Intent(Android.Content.Intent.ActionSend);
            email.PutExtra(Android.Content.Intent.ExtraEmail, new string[] { "bart.vanraes@gmail.com" });
            email.PutExtra(Android.Content.Intent.ExtraSubject, String.Format("Soundrecorder IMU file: {0}", fileName));*/


        }
    }
}