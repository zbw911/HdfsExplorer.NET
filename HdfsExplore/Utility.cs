using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HdfsExplore
{
    class Utility
    {
        public static DateTime JavaTicksToDatetime(long time)
        {

            var dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var tricks1970 = dt1970.Ticks;//1970年1月1日刻度
            var timeTricks = tricks1970 + time * 10000;//日志日期刻度
            var date = new DateTime(timeTricks, DateTimeKind.Utc);//转化为DateTime

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
}
