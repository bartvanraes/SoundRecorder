using System;
using System.Collections.Generic;
using System.Text;

namespace SoundRecorder.DTO.Files
{
    public class ImuDTO
    {
        public float AccelerometerX { get; set; }
        public float AccelerometerY { get; set; }
        public float AccelerometerZ { get; set; }
        public float GyroscopeX { get; set; }
        public float GyroscopeY { get; set; }
        public float GyroscopeZ { get; set; }
        public float MagneticFieldX { get; set; }
        public float MagneticFieldY { get; set; }
        public float MagneticFieldZ { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
