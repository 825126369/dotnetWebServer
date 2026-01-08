using System;

namespace GameLogic
{
    /// <summary>
    /// 对委托的一些操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class DelegateUtility
    {
        public static bool CheckFunIsExist<T>(Action<T> mEvent, Action<T> fun)
        {
            if (mEvent == null)
            {
                return false;
            }
            Delegate[] mList = mEvent.GetInvocationList();
            return Array.Exists<Delegate>(mList, (x) => x.Equals(fun));
        }
    }
    
    public static class TimeUtility
    {
        public static TimeSpan GetTimeSpanFromDateString(string timeStr)
        {
            string dateFormatStr = "g";
            TimeSpan mTimeSpan = TimeSpan.ParseExact(timeStr, dateFormatStr, System.Globalization.CultureInfo.InvariantCulture);
            return mTimeSpan;
        }

        public static DateTime GetLocalTimeFromDateString(string timeStr)
        {
            string dateFormatStr = "yyyy/MM/dd HH:mm:ss";
            DateTime beginTime = DateTime.ParseExact(timeStr, dateFormatStr, System.Globalization.CultureInfo.InvariantCulture);
            return beginTime;
        }

        public static UInt64 GetTimeStampFromLocalTime(DateTime nLocalTime)
        {
            System.DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(nLocalTime, TimeZoneInfo.Local);
            return GetTimeStampFromUTCTime(utcTime);
        }

        public static UInt64 GetTimeStampFromUTCTime(DateTime utcTime)
        {
            TimeSpan ts = utcTime - new DateTime(1970, 1, 1, 0, 0, 0);
            return (UInt64)ts.TotalSeconds;
        }

        public static DateTime GetUTCTimeFromTimeStamp(UInt64 nTimeStamp)
        {
            DateTime dateTimeStart = new DateTime(1970, 1, 1, 0, 0, 0);
            return dateTimeStart.AddSeconds(nTimeStamp);
        }

        public static DateTime GetLocalTimeFromTimeStamp(UInt64 mTimeStamp)
        {
            DateTime utcTime = GetUTCTimeFromTimeStamp(mTimeStamp);
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.Local);
        }
    }

    public static class RandomUtility
    {
        private static Random random = null;

        static RandomUtility()
        {
            random = new Random((int)TimeUtility.GetTimeStampFromLocalTime(DateTime.Now));
        }

        public static double Random()
        {
            return random.NextDouble();
        }

        public static int Random(int x, int y)
        {
            return random.Next(x, y + 1);
        }

        public static uint Random(uint x, uint y)
        {
            return (uint)random.Next((int)x, (int)y + 1);
        }
    }
}