using System;
using System.Collections.Generic;
using System.Text;

namespace MarkThree.Guardian.Forms
{
    class VolumeHelper
    {
        public static decimal ProjectedVolume(DateTime now, decimal averageDailyVolume)
        {
            const int nineThirtyAm = (9 * 60) + 30;
            const int fourPm = 16 * 60;
            const int minutesInTradingDay = fourPm - nineThirtyAm;
            // get the current time of day in seconds
            int nowMinutes = (now.Hour * 60) + now.Minute;
            ///////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////
            /// The following code is a placeholder until the real formula can be 
            /// developed. For now the trading day is divided into 78 5 minute periods.
            /// each one of these periods has the percent up to that point calulated.
            /// The value are based on the guassian curve, but in reality the function by 
            /// more dynamic and break the day into smaller intervals.
            double[] percentToInterval = {
                        2.345, 4.605, 6.782, 8.878, 10.895, 12.836, 14.703, 16.498, 18.224, 19.882,
                        21.475, 23.006, 24.476, 25.888, 27.244, 28.546, 29.796, 30.997, 32.151, 33.260, 
                        34.326, 35.352, 36.340, 37.292, 38.210, 39.097, 39.955, 40.786, 41.592, 42.376, 
                        43.139, 43.884, 44.614, 45.330, 46.035, 46.731, 47.420, 48.105, 48.788, 49.471, 
                        50.156, 50.845, 51.541, 52.246, 52.962, 53.692, 54.437, 55.200, 55.984, 56.790, 
                        57.621, 58.479, 59.366, 60.284, 61.236, 62.224, 63.250, 64.316, 65.425, 66.579, 
                        67.780, 69.030, 70.332, 71.688, 73.100, 74.570, 76.101, 77.694, 79.352, 81.078, 
                        82.873, 84.740, 86.681, 88.698, 90.794, 92.971, 95.231, 97.576 };

            int intervalCount = 78;
            // compute which 5 minute interval we are in
            int currentInterval = (int)(((double)(nowMinutes - nineThirtyAm) / (double)minutesInTradingDay) * (double)intervalCount);
            // make sure we are in range
            if (currentInterval < 0)
                return 0;
            if (currentInterval >= intervalCount)
                return averageDailyVolume;
            // since we now we are inside the trading day take the average volume and multiple it by the expected volume percent
            decimal projectVolume = (decimal)((double)averageDailyVolume * (percentToInterval[currentInterval] / 100.0));
            return projectVolume;
        }

    }
}
