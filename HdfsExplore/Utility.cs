using System;
using System.Configuration;

namespace HdfsExplore
{
    internal class Utility
    {
        public static DateTime JavaTicksToDatetime(long time)
        {
            var dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var tricks1970 = dt1970.Ticks; //1970年1月1日刻度
            var timeTricks = tricks1970 + time*10000; //日志日期刻度
            var date = new DateTime(timeTricks, DateTimeKind.Utc); //转化为DateTime

            return TimeZone.CurrentTimeZone.ToLocalTime(date);
        }

        internal static DateTime JavaTicksToDatetime(string strtime)
        {
            long ticks;

            if (!long.TryParse(strtime, out ticks))
                return DateTime.MinValue;


            return JavaTicksToDatetime(ticks);
        }
    }


    /// <summary>
    ///     配置信息维护
    /// </summary>
    public class AppConfig
    {
        public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        /// <summary>
        ///     获取配置值
        /// </summary>
        /// <param name="key">配置标识</param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            var result = string.Empty;
            if (config.AppSettings.Settings[key] != null)
                result = config.AppSettings.Settings[key].Value;
            return result;
        }

        /// <summary>
        ///     修改或增加配置值
        /// </summary>
        /// <param name="key">配置标识</param>
        /// <param name="value">配置值</param>
        public static void SetValue(string key, string value)
        {
            if (config.AppSettings.Settings[key] != null)
                config.AppSettings.Settings[key].Value = value;
            else
                config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>
        ///     删除配置值
        /// </summary>
        /// <param name="key">配置标识</param>
        public static void DeleteValue(string key)
        {
            config.AppSettings.Settings.Remove(key);
        }
    }
}