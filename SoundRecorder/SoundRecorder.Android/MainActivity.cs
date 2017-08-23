using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using Android.Content;
using System.Threading.Tasks;
using SoundRecorder.Interfaces;
using SoundRecorder.Droid;
using System.IO;
using MimeKit;
using MailKit.Net.Smtp;
using System.Collections.Generic;
using SoundRecorder.Droid.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(MainActivity))]
namespace SoundRecorder.Droid
{
    [Activity(Label = "SoundRecorder", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, ISensorEventListener, IIMUService, IEmailService
    {
        static readonly object _syncLockAccelerometer = new object();
        static readonly object _syncLockGyroscope = new object();
        static readonly object _syncLockMagneticField = new object();
        static readonly object _syncLockTimeStamps = new object();
        private SensorManager _sensorManager;
        private static float _accelerometerX, _accelerometerY, _accelerometerZ, _gyroscopeX, _gyroscopeY, _gyroscopeZ, _magneticFieldX, _magneticFieldY, _magneticFieldZ,
            _prevAccelerometerX, _prevAccelerometerY, _prevAccelerometerZ, _prevGyroscopeX, _prevGyroscopeY, _prevGyroscopeZ, _prevMagneticFieldX, _prevMagneticFieldY, _prevMagneticFieldZ;
        private static DateTime _timeStamp, _interpolatedTimeStamp, _prevInterpolatedTimeStamp, _prevTimeStamp;

        private static int INTERVAL_MILLISECONDS = 20;

        public MainActivity()
        {

        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            
        }

        public void OnSensorChanged(SensorEvent e)
        {
            switch (e.Sensor.Type)
            {
                case SensorType.Accelerometer:

                    lock (_syncLockAccelerometer)
                    {
                        _accelerometerX = e.Values[0];
                        _accelerometerY = e.Values[1];
                        _accelerometerZ = e.Values[2];
                        //Console.WriteLine(String.Format("Time: {0:mm:ss.fff}, X: {1:0.00}, Y: {2:0.00}, Z: {3:0.00}", DateTime.Now, _x, _y, _z));
                    }
                    break;
                case SensorType.Gyroscope:
                    lock (_syncLockGyroscope)
                    {
                        _gyroscopeX = e.Values[0];
                        _gyroscopeY = e.Values[1];
                        _gyroscopeZ = e.Values[2];
                    }
                    break;
                case SensorType.MagneticField:
                    lock(_syncLockMagneticField)
                    {
                        _magneticFieldX = e.Values[0];
                        _magneticFieldY = e.Values[1];
                        _magneticFieldZ = e.Values[2];
                    }
                    break;
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            _sensorManager = (SensorManager)GetSystemService(Context.SensorService);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }

        protected override void OnResume()
        {
            base.OnResume();

            _prevTimeStamp = DateTime.MinValue;
            _prevInterpolatedTimeStamp = DateTime.MinValue;

            var accelerometerSensor = _sensorManager.GetDefaultSensor(SensorType.Accelerometer);
            var gyroscopeSensor = _sensorManager.GetDefaultSensor(SensorType.Gyroscope);
            var magneticFieldSensor = _sensorManager.GetDefaultSensor(SensorType.MagneticField);

            if (accelerometerSensor != null)
            {
                _sensorManager.RegisterListener(this, accelerometerSensor, SensorDelay.Fastest);
            }

            if (gyroscopeSensor != null)
            {
                _sensorManager.RegisterListener(this, gyroscopeSensor, SensorDelay.Fastest);
            }

            if (magneticFieldSensor != null)
            {
                _sensorManager.RegisterListener(this, magneticFieldSensor, SensorDelay.Fastest);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            _sensorManager.UnregisterListener(this);
        }

        public async Task<IList<IMUReadings>> GetReadings()
        {
            float accelerometerX, accelerometerY, accelerometerZ, gyroscopeX, gyroscopeY, gyroscopeZ, magneticFieldX, magneticFieldY, magneticFieldZ;
            IList<IMUReadings> readings = new List<IMUReadings>();

            _timeStamp = DateTime.Now;

            //In case the interpolated timestamps are lagging behind the previous timestamp, add extra interpolated results to catch up
            do
            {
                lock (_syncLockTimeStamps)
                {

                    if (_prevTimeStamp != DateTime.MinValue)
                    {
                        if (_prevInterpolatedTimeStamp != DateTime.MinValue)
                        {
                            _interpolatedTimeStamp = _prevInterpolatedTimeStamp.AddMilliseconds(INTERVAL_MILLISECONDS);
                        }
                        else
                        {
                            _interpolatedTimeStamp = _prevTimeStamp.AddMilliseconds(INTERVAL_MILLISECONDS);
                        }
                    }
                    else
                    {
                        _interpolatedTimeStamp = _timeStamp;
                    }
                }

                lock (_syncLockAccelerometer)
                {
                    accelerometerX = InterpolationHelper.LinearInterpolation(INTERVAL_MILLISECONDS, _prevInterpolatedTimeStamp, _timeStamp, _prevAccelerometerX, _accelerometerX);
                    accelerometerY = InterpolationHelper.LinearInterpolation(INTERVAL_MILLISECONDS, _prevInterpolatedTimeStamp, _timeStamp, _prevAccelerometerY, _accelerometerY);
                    accelerometerZ = InterpolationHelper.LinearInterpolation(INTERVAL_MILLISECONDS, _prevInterpolatedTimeStamp, _timeStamp, _prevAccelerometerZ, _accelerometerZ);
                    _prevAccelerometerX = accelerometerX;
                    _prevAccelerometerY = accelerometerY;
                    _prevAccelerometerZ = accelerometerZ;
                }

                lock (_syncLockGyroscope)
                {
                    gyroscopeX = InterpolationHelper.LinearInterpolation(INTERVAL_MILLISECONDS, _prevInterpolatedTimeStamp, _timeStamp, _prevGyroscopeX, _gyroscopeX);
                    gyroscopeY = InterpolationHelper.LinearInterpolation(INTERVAL_MILLISECONDS, _prevInterpolatedTimeStamp, _timeStamp, _prevGyroscopeY, _gyroscopeY);
                    gyroscopeZ = InterpolationHelper.LinearInterpolation(INTERVAL_MILLISECONDS, _prevInterpolatedTimeStamp, _timeStamp, _prevGyroscopeZ, _gyroscopeZ);
                    _prevGyroscopeX = gyroscopeX;
                    _prevGyroscopeY = gyroscopeY;
                    _prevGyroscopeZ = gyroscopeZ;
                }

                lock (_syncLockMagneticField)
                {
                    magneticFieldX = InterpolationHelper.LinearInterpolation(INTERVAL_MILLISECONDS, _prevInterpolatedTimeStamp, _timeStamp, _prevMagneticFieldX, _magneticFieldX);
                    magneticFieldY = InterpolationHelper.LinearInterpolation(INTERVAL_MILLISECONDS, _prevInterpolatedTimeStamp, _timeStamp, _prevMagneticFieldY, _magneticFieldY);
                    magneticFieldZ = InterpolationHelper.LinearInterpolation(INTERVAL_MILLISECONDS, _prevInterpolatedTimeStamp, _timeStamp, _prevMagneticFieldZ, _magneticFieldZ);
                    _prevMagneticFieldX = magneticFieldX;
                    _prevMagneticFieldY = magneticFieldY;
                    _prevMagneticFieldZ = magneticFieldZ;
                }



                //Console.WriteLine(String.Format("prevTimeStamp: {0}, actualTimeStamp: {1}, interpollatedTimeStamp: {2}", _prevTimeStamp, _timeStamp, interpolatedTimeStamp));

                lock (_syncLockTimeStamps)
                {
                    _prevTimeStamp = _timeStamp;
                    _prevInterpolatedTimeStamp = _interpolatedTimeStamp;
                }

                readings.Add(new IMUReadings
                {
                    AccelerometerX = accelerometerX,
                    AccelerometerY = accelerometerY,
                    AccelerometerZ = accelerometerZ,
                    GyroscopeX = gyroscopeX,
                    GyroscopeY = gyroscopeY,
                    GyroscopeZ = gyroscopeZ,
                    MagneticFieldX = magneticFieldX,
                    MagneticFieldY = magneticFieldY,
                    MagneticFieldZ = magneticFieldZ,
                    TimeStamp = _interpolatedTimeStamp
                });
            }
            while (_interpolatedTimeStamp < _prevTimeStamp);

            return readings;
        }

        public void SendEmailWithAttachment(string emailAddress, string text, string subject, string fileName)
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, fileName);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("SoundRecorder", "bart.vanraes@gmail.com"));
            message.To.Add(new MailboxAddress(emailAddress, emailAddress));
            message.Subject = subject;
            string body = "";
            //Read xml file instead of attachment
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        body += line;
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            finally
            {
                File.Delete(filePath);
            }

            /*var co = new ContentObject(File.OpenRead(filePath), ContentEncoding.Default);
            var cd = new ContentDisposition(ContentDisposition.Attachment);
            var ct = ContentEncoding.Base64;

            var attachment = new MimePart("xml")
            {
                ContentObject = new ContentObject(File.OpenRead(filePath), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = fileName
            };*/

            var multiPart = new Multipart("mixed");

            multiPart.Add(new TextPart("plain")
            {
                Text = body
            });
           // multiPart.Add(attachment);
            message.Body = multiPart;

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect("smtp.gmail.com", 587, false);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("bart.vanraes.test@gmail.com", "Graspop666");

                client.Send(message);
                client.Disconnect(true);
            }


            /*var email = new Intent(Android.Content.Intent.ActionSend);
            
            var file = new Java.IO.File(filePath);
            var uri = Android.Net.Uri.FromFile(file);

            file.SetReadable(true, true);
            email.PutExtra(Android.Content.Intent.ExtraEmail, new string[] { emailAddress });
            email.PutExtra(Android.Content.Intent.ExtraSubject, subject);
            email.PutExtra(Android.Content.Intent.ExtraStream, uri);
            email.SetType("message/rfc822");
            StartActivity(Android.Content.Intent.CreateChooser(email, "Send mail..."));*/
        }
    }
}

