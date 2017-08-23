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
using SoundRecorder.DTO.Files;
using SoundRecorder.Shared.Extensions;

namespace SoundRecorder
{
    public class MainPageViewModel : ViewModelBase
    {
        private float accelerometerX, accelerometerY, accelerometerZ, gyroscopeX, gyroscopeY, gyroscopeZ, magneticFieldX, magneticFieldY, magneticFieldZ;
        private int readingsCount;
        private bool isRecording;
        private static CancellationTokenSource CancellationToken { get; set; }

        //public event PropertyChangedEventHandler PropertyChanged;
        public ICommand StartRecordingCommand { get; private set; }
        public ICommand StopRecordingCommand { get; private set; }

        private IList<ImuDTO> imuDTOList;
        
        public int ReadingsCount
        {
            get
            {
                return readingsCount;
            }
            set
            {
                readingsCount = value;
                OnPropertyChanged("ReadingsCount");
            }
        }

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
                imuDTOList = new List<ImuDTO>();
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
                        var lastReading = readings.Last();
                        AccelerometerX = lastReading.AccelerometerX;
                        AccelerometerY = lastReading.AccelerometerY;
                        AccelerometerZ = lastReading.AccelerometerZ;
                        GyroscopeX = lastReading.GyroscopeX;
                        GyroscopeY = lastReading.GyroscopeY;
                        GyroscopeZ = lastReading.GyroscopeZ;
                        MagneticFieldX = lastReading.MagneticFieldX;
                        MagneticFieldY = lastReading.MagneticFieldY;
                        MagneticFieldZ = lastReading.MagneticFieldZ;
                        ReadingsCount = readings.Count;
                    });

                    readings.ForEach(x =>
                    {
                        imuDTOList.Add(new ImuDTO
                        {
                            AccelerometerX = x.AccelerometerX,
                            AccelerometerY = x.AccelerometerY,
                            AccelerometerZ = x.AccelerometerZ,
                            GyroscopeX = x.GyroscopeX,
                            GyroscopeY = x.GyroscopeY,
                            GyroscopeZ = x.GyroscopeZ,
                            MagneticFieldX = x.MagneticFieldX,
                            MagneticFieldY = x.MagneticFieldY,
                            MagneticFieldZ = x.MagneticFieldZ,
                            TimeStamp = x.TimeStamp
                        });
                    });

                    await Task.Delay(20, CancellationToken.Token);

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

                var fileService = DependencyService.Get<IFileService>();
                var emailService = DependencyService.Get<IEmailService>();

                Task.Run(() => SaveFileAndSendEmail(fileService, emailService));
            }
        }

        private async void SaveFileAndSendEmail(IFileService fileService, IEmailService emailService)
        {
            var fileName = DateTime.Now.ToString("ddMMyyyyhhmmssfff") + ".xml";

            var filePath = fileService.WriteFile(fileName, imuDTOList);

            emailService.SendEmailWithAttachment("bart.vanraes@gmail.com", String.Format("Soundrecorder IMU file: {0}", fileName), String.Format("Soundrecorder IMU file: {0}", fileName), fileName);
        }

    }
}
