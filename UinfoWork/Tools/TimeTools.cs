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

namespace Uinfo.Tools
{
    public class TimeTools
    {
        public static DateTime 计算本学期夏开学时间(DateTime time)
        {
            DateTime datetime = new DateTime(time.Year, 9, 8);
            datetime =datetime.AddDays(1 - (Convert.ToInt16(datetime.DayOfWeek.ToString("D")) > 0 ?Convert.ToInt16(datetime.DayOfWeek.ToString("D")):7));
            if (time.Month >= 9 && time.Day >= datetime.Day)
                datetime = new DateTime(time.Year + 1, 2, 28);
            else
                datetime = new DateTime(time.Year, 2, 28);
            return datetime.AddDays(1 - (Convert.ToInt16(datetime.DayOfWeek.ToString("D")) > 0 ? Convert.ToInt16(datetime.DayOfWeek.ToString("D")) : 7));
        }
        public static DateTime 计算本学期冬开学时间(DateTime time)
        {
            DateTime datetime = new DateTime(time.Year, 9, 8);
            datetime=datetime.AddDays(1 - (Convert.ToInt16(datetime.DayOfWeek.ToString("D")) > 0 ? Convert.ToInt16(datetime.DayOfWeek.ToString("D")) : 7));
            if (time.Month >= 9 && time.Day >= datetime.Day)
                datetime = new DateTime(time.Year, 9, 8);
            else
                datetime = new DateTime(time.Year - 1, 9, 8);
            return datetime.AddDays(1 - (Convert.ToInt16(datetime.DayOfWeek.ToString("D")) > 0 ? Convert.ToInt16(datetime.DayOfWeek.ToString("D")) : 7));
        }
        public static int 计算冬的周(DateTime time)
        {
            DateTime 冬= 计算本学期冬开学时间(time);
            var t冬 = 冬.AddDays(1 - (Convert.ToInt16(冬.DayOfWeek.ToString("D")) > 0 ? Convert.ToInt16(冬.DayOfWeek.ToString("D")) : 7));
            float 冬Weeks = (time - t冬).Days / 7.0f;
            if (冬Weeks >= 0 && time> t冬) 冬Weeks += 1;
            return (int)冬Weeks;
        }
        public static int 计算夏的周(DateTime time)
        {
            DateTime 夏 = 计算本学期夏开学时间(time);
            var t夏 = 夏.AddDays(1 - (Convert.ToInt16(夏.DayOfWeek.ToString("D")) > 0 ? Convert.ToInt16(夏.DayOfWeek.ToString("D")) : 7));
            float 夏Weeks = (time - t夏).Days / 7.0f;
            if (夏Weeks >= 0 && time> t夏) 夏Weeks += 1;
            return (int)夏Weeks;
        }
        public static int 计算当前周(DateTime time)
        {
            var 冬Weeks = 计算冬的周(time);
            var 夏Weeks = 计算夏的周(time);
            if (夏Weeks > 0) return 夏Weeks;
            else return 冬Weeks;
        }
        public static float 计算当前节(DateTime time)
        {
            float 节 = 1;
            var Time = TimeSpan.Parse(time.Hour.ToString() + ":" + time.Minute.ToString());
            foreach (TimeSpan classtime in TimeNode)
            {
                if (Time > classtime) 节 += 0.5f;
            }
            return 节;
        }
        public static DateTime 计算节的开始时间(DateTime time)
        {
            var 节 = 计算当前节(time);
            节 = (((int)Math.Floor(节)) - 1) * 2;
            if (节 >= TimeNode.Length - 1) 节 = TimeNode.Length - 2;
            return new DateTime(2018,1,1, TimeNode[(int)节].Hours, TimeNode[(int)节].Minutes,0);
        }
        public static DateTime 计算节的结束时间(DateTime time)
        {
            var 节 = 计算当前节(time);
            节 = (((int)Math.Floor(节)) - 1) * 2;
            if (节 >= TimeNode.Length - 1) 节 = TimeNode.Length - 2;
            return new DateTime(2018, 1, 1, TimeNode[(int)节 + 1].Hours, TimeNode[(int)节 + 1].Minutes, 0);
        }
        private static TimeSpan[] TimeNode =
        {
            TimeSpan.Parse("08:00"),
            TimeSpan.Parse("08:45"),

            TimeSpan.Parse("08:55"),
            TimeSpan.Parse("09:40"),

            TimeSpan.Parse("10:00"),
            TimeSpan.Parse("10:45"),

            TimeSpan.Parse("10:55"),
            TimeSpan.Parse("11:40"),


            TimeSpan.Parse("14:00"),
            TimeSpan.Parse("14:45"),

            TimeSpan.Parse("14:55"),
            TimeSpan.Parse("15:40"),

            TimeSpan.Parse("16:00"),
            TimeSpan.Parse("16:45"),

            TimeSpan.Parse("16:55"),
            TimeSpan.Parse("17:40"),


            TimeSpan.Parse("19:00"),
            TimeSpan.Parse("19:45"),

            TimeSpan.Parse("19:55"),
            TimeSpan.Parse("20:40"),

            TimeSpan.Parse("20:50"),
            TimeSpan.Parse("21:35")
        };
    }
}