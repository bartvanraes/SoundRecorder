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

namespace SoundRecorder.Droid.Helpers
{
    public static class InterpolationHelper
    {
        public static float LinearInterpolation(float expectedInterVal, DateTime prevTimeStamp, DateTime actualTimeStamp, float prevVal, float newVal)
        {
            //var actualMilliseconds = actualTimeStamp.Ticks / 10000;
            //var prevMilliseconds = prevTimeStamp.Ticks / 10000;

            if(prevTimeStamp.Ticks == 0 || prevVal == newVal)
            {
                return newVal;
            }

            var expectedTicks = expectedInterVal * 10000;

            var diff = actualTimeStamp.Ticks - prevTimeStamp.Ticks;

            if (diff == 0)
            {
                return (prevVal + newVal) / 2;
            }

            //return prevVal + (diff * (newVal - prevVal)) / diff;

            //return y0 * (x - x1) / (x0 - x1) + y1 * (x - x0) / (x1 - x0);

            //return prevVal * (expectedTicks - diff) / (0 - diff) + newVal * (expectedTicks - 0) / (diff - 0);

            //See https://en.wikipedia.org/wiki/Linear_interpolation
            return (prevVal * (diff - expectedTicks) + newVal * (expectedTicks - 0)) / (diff - 0);

        }
    }
}