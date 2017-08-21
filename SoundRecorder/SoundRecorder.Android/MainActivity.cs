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

[assembly: Xamarin.Forms.Dependency(typeof(MainActivity))]
namespace SoundRecorder.Droid
{
    [Activity(Label = "SoundRecorder", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, ISensorEventListener, IIMUService
    {
        static readonly object _syncLockAccelerometer = new object();
        static readonly object _syncLockGyroscope = new object();
        static readonly object _syncLockMagneticField = new object();
        private SensorManager _sensorManager;
        private static float _accelerometerX, _accelerometerY, _accelerometerZ, _gyroscopeX, _gyroscopeY, _gyroscopeZ, _magneticFieldX, _magneticFieldY, _magneticFieldZ;

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

        public async Task<IMUReadings> GetReadings()
        {
            float accelerometerX, accelerometerY, accelerometerZ, gyroscopeX, gyroscopeY, gyroscopeZ, magneticFieldX, magneticFieldY, magneticFieldZ;
            lock (_syncLockAccelerometer)
            {
                accelerometerX = _accelerometerX;
                accelerometerY = _accelerometerY;
                accelerometerZ = _accelerometerZ;
            }

            lock(_syncLockGyroscope)
            {
                gyroscopeX = _gyroscopeX;
                gyroscopeY = _gyroscopeY;
                gyroscopeZ = _gyroscopeZ;
            }

            lock(_syncLockMagneticField)
            {
                magneticFieldX = _magneticFieldX;
                magneticFieldY = _magneticFieldY;
                magneticFieldZ = _magneticFieldZ;
            }

            return new IMUReadings
            {
                AccelerometerX = accelerometerX,
                AccelerometerY = accelerometerY,
                AccelerometerZ = accelerometerZ,
                GyroscopeX = gyroscopeX,
                GyroscopeY = gyroscopeY,
                GyroscopeZ = gyroscopeZ,
                MagneticFieldX = magneticFieldX,
                MagneticFieldY = magneticFieldY,
                MagneticFieldZ = magneticFieldZ
            };
        }        
    }
}

