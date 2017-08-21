using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using SoundRecorder.Interfaces;
using System.Threading;
using System.Diagnostics;
using SoundRecorder.Helpers;

namespace SoundRecorder
{
    public class MainPageViewModel : ViewModelBase
    {
        private float accelerometerX, accelerometerY, accelerometerZ, gyroscopeX, gyroscopeY, gyroscopeZ, magneticFieldX, magneticFieldY, magneticFieldZ;
        private bool isRecording;
        private static CancellationTokenSource CancellationToken { get; set; }

        //public event PropertyChangedEventHandler PropertyChanged;
        public ICommand StartRecordingCommand { get; private set; }
        public ICommand StopRecordingCommand { get; private set; }      
        
        public bool IsRecording
        {
            get
            {
                return isRecording;
            }
            private set {
                if(isRecording != value)
                {
                    isRecording = value;
                    OnPropertyChanged("IsRecording");
                }
            }
        }

        public float AccelerometerX
        {
            get
            {
                return accelerometerX;
            }
            set
            {
                if(accelerometerX != value)
                {
                    accelerometerX = value;
                    OnPropertyChanged("AccelerometerX");
                }
            }
        }

        public float AccelerometerY
        {
            get
            {
                return accelerometerY;
            }
            set
            {
                if (accelerometerY != value)
                {
                    accelerometerY = value;
                    OnPropertyChanged("AccelerometerY");
                }
            }
        }

        public float AccelerometerZ
        {
            get
            {
                return accelerometerZ;
            }
            set
            {
                if (accelerometerZ != value)
                {
                    accelerometerZ = value;
                    OnPropertyChanged("AccelerometerZ");
                }
            }
        }

        public float GyroscopeX
        {
            get
            {
                return gyroscopeX;
            }
            set
            {
                if (gyroscopeX != value)
                {
                    gyroscopeX = value;
                    OnPropertyChanged("GyroscopeX");
                }
            }
        }

        public float GyroscopeY
        {
            get
            {
                return gyroscopeY;
            }
            set
            {
                if (gyroscopeY != value)
                {
                    gyroscopeY = value;
                    OnPropertyChanged("GyroscopeY");
                }
            }
        }

        public float GyroscopeZ
        {
            get
            {
                return gyroscopeZ;
            }
            set
            {
                if (gyroscopeZ != value)
                {
                    gyroscopeZ = value;
                    OnPropertyChanged("GyroscopeZ");
                }
            }
        }

        public float MagneticFieldX
        {
            get
            {
                return magneticFieldX;
            }
            set
            {
                if (magneticFieldX != value)
                {
                    magneticFieldX = value;
                    OnPropertyChanged("MagneticFieldX");
                }
            }
        }

        public float MagneticFieldY
        {
            get
            {
                return magneticFieldY;
            }
            set
            {
                if (magneticFieldY != value)
                {
                    magneticFieldY = value;
                    OnPropertyChanged("MagneticFieldY");
                }
            }
        }

        public float MagneticFieldZ
        {
            get
            {
                return magneticFieldZ;
            }
            set
            {
                if (magneticFieldZ != value)
                {
                    magneticFieldZ = value;
                    OnPropertyChanged("MagneticFieldZ");
                }
            }
        }

        public MainPageViewModel()
        {
            IsRecording = false;
            StartRecordingCommand = new Command(StartRecording);
            StopRecordingCommand = new Command(StopRecording);
        }

        

        /*protected virtual void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }*/

        private void StartRecording()
        {
            if (!IsRecording)
            {
                var service = DependencyService.Get<IIMUService>();
                Task.Run(() => PollAccelerometer(service));                
            }            
        }

        private async void PollAccelerometer(IIMUService service)
        {
            IsRecording = true;

            CancellationToken = new CancellationTokenSource();

            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    CancellationToken.Token.ThrowIfCancellationRequested();

                    var readings = await service.GetReadings();
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        AccelerometerX = readings.AccelerometerX;
                        AccelerometerY = readings.AccelerometerY;
                        AccelerometerZ = readings.AccelerometerZ;
                        GyroscopeX = readings.GyroscopeX;
                        GyroscopeY = readings.GyroscopeY;
                        GyroscopeZ = readings.GyroscopeZ;
                        MagneticFieldX = readings.MagneticFieldX;
                        MagneticFieldY = readings.MagneticFieldY;
                        MagneticFieldZ = readings.MagneticFieldZ;
                    });

                    await Task.Delay(10, CancellationToken.Token);

                    /*await Task.Delay(100, CancellationToken.Token).ContinueWith(async (arg) =>
                    {
                        if (!CancellationToken.Token.IsCancellationRequested)
                        {
                            CancellationToken.Token.ThrowIfCancellationRequested();

                            
                        }
                    });*/
                    
                }
                catch(Exception ex)
                {
                    Debug.WriteLine("Exception: " + ex.Message);
                    IsRecording = false;
                }
            }
        }

        private void StopRecording()
        {
            if(CancellationToken != null && IsRecording)
            {
                CancellationToken.Cancel();
                IsRecording = false;
            }
        }

    }
}
