using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TjdHelper.Tools
{
    public class TimeHelper
    {
        /// <summary>
        /// 时间戳->时间
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="isMilliSeconds"></param>
        /// <returns></returns>
        public static string ConvertToTimeByTimestamp(long timestamp, bool isMilliSeconds)
        {
            // 创建UTC时间
            DateTime utcTime;
            if (isMilliSeconds)
            {
                utcTime = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
            }
            else
            {
                utcTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
            }

            // 转换为北京时间
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
            DateTime beijingTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, cstZone);
            // 格式化为字符串
            return beijingTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 时间->时间戳
        /// </summary>
        /// <param name="timeStr">时间字符串（yyyy-dd-MM HH:mm:ss）</param>
        /// <returns></returns>
        public static long ConvertToTimestampByTime(string timeStr, bool isMilliSeconds)
        {
            // 将时间字符串解析为日期时间对象
            DateTime dateTime = DateTime.ParseExact(timeStr, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            // 转换为北京时间
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
            dateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, cstZone);

            // 计算时间戳
            long timestampResult;
            if (isMilliSeconds)
            {
                timestampResult = (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            }
            else
            {
                timestampResult = (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            }
            return timestampResult;
        }

        /// <summary>
        /// 获取n分钟前时间信息
        /// </summary>
        /// <param name="timeStr"></param>
        /// <param name="span"></param>
        /// <param name="isMilliSeconds"></param>
        /// <returns></returns>
        public static TimeResultObj GetTimeBefore(string timestampStr, TimeSpan span, bool isMilliSeconds)
        {
            DateTime dateTimeBefore;
            if (isMilliSeconds)
            {
                dateTimeBefore = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(timestampStr)).UtcDateTime;
            }
            else
            {
                dateTimeBefore = DateTimeOffset.FromUnixTimeSeconds(long.Parse(timestampStr)).UtcDateTime;
            }
            DateTime beforeTime = dateTimeBefore - span;
            long beforeTimestamp;
            if (isMilliSeconds)
            {
                beforeTimestamp = ((long)(beforeTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
            }
            else
            {
                beforeTimestamp = ((long)(beforeTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
            }
            string beforeTimeStr = ConvertToTimeByTimestamp(beforeTimestamp, isMilliSeconds);

            return new TimeResultObj() { TimeBefore = beforeTimeStr, TimestampBefore = beforeTimestamp };
        }

        /// <summary>
        /// 获取n分钟后时间数据
        /// </summary>
        /// <param name="timeStr"></param>
        /// <param name="span"></param>
        /// <param name="isMilliSeconds"></param>
        /// <returns></returns>
        public static TimeResultObj GetTimeAfter(string timestampStr, TimeSpan span, bool isMilliSeconds)
        {
            DateTime dateTimeAfter;
            if (isMilliSeconds)
            {
                dateTimeAfter = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(timestampStr)).UtcDateTime;
            }
            else
            {
                dateTimeAfter = DateTimeOffset.FromUnixTimeSeconds(long.Parse(timestampStr)).UtcDateTime;
            }
            DateTime afterTime = dateTimeAfter + span;
            long afterTimestamp;
            if (isMilliSeconds)
            {
                afterTimestamp = ((long)(afterTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
            }
            else
            {
                afterTimestamp = ((long)(afterTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
            }
            string afterTimeStr = ConvertToTimeByTimestamp(afterTimestamp, isMilliSeconds);

            return new TimeResultObj() { TimeAfter = afterTimeStr, TimestampAfter = afterTimestamp };
        }

        /// <summary>
        /// DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetTimeByDateTimeSec(DateTime dateTime)
        {
            // 创建一个带有偏移量的DateTimeOffset对象，表示UTC+8时间
            DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime, TimeSpan.FromHours(8));

            // 计算秒级时间戳
            long timestamp = dateTimeOffset.ToUnixTimeSeconds();

            return timestamp;
        }

        /// <summary>
        /// DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetTimeByDateTimeMilSec(DateTime dateTime)
        {
            // 创建一个带有偏移量的DateTimeOffset对象，表示UTC+8时间
            DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime, TimeSpan.FromHours(8));

            // 计算秒级时间戳
            long timestamp = dateTimeOffset.ToUnixTimeMilliseconds();

            return timestamp;
        }
    }

    /// <summary>
    /// 转换后的时间结果信息
    /// </summary>
    public class TimeResultObj
    {
        public long TimestampNow { get; set; }
        public string TimeNow { get; set; }

        public long TimestampBefore { get; set; }
        public string TimeBefore { get; set; }

        public long TimestampAfter { get; set; }
        public string TimeAfter { get; set; }
    }
}
