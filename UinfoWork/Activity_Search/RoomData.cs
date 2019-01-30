using System.Text.RegularExpressions;
using Java.Util;
using System;
using System.Collections.Generic;
namespace Uinfo
{
    /// <summary>
    /// 搜索关键词
    /// </summary>
    public class SearchKey
    {
        public string RoomNum { get; set; }//教室编号
        public string CourseName { get; set; }//课程名称
        public string TeacherName { get; set; }//教师名字
        public string CourseTime { get; set; }//上课时间
        public string CourseNum { get; set; }//课程代码
    }
    /// <summary>
    /// 用于显示的数据
    /// </summary>
    public class Show_Roomdate
    {
        public string RoomNum { get; set; }//教室编号
        public string CourseName { get; set; }//课程名称
        public string TeacherName { get; set; }//教师名字
        public string CourseTime { get; set; }//上课时间
        public string CourseNum { get; set; }//课程代码
    }
    /// <summary>
    /// 一节课程的信息
    /// </summary>
    public class Coursedata
    {
        public string CourseName { get; set; }//课程名称
        public string TeacherName { get; set; }//教师名字
        public string CourseTime { get; set; }//上课时间
        public string CourseNum { get; set; }//课程代码
        public string CourseChange { get; set; }//课程变化信息
    }
    public class CourseTimeFormate
    {
        public int 开始周;
        public int 结束周;
        public string 单双周;
        public DayOfWeek 星期;
        public List<int> 节=new List<int> ();
        public static CourseTimeFormate Parse(string OldTimeFormate)
        {
            CourseTimeFormate temp = new CourseTimeFormate();
            #region 转化星期
            switch (OldTimeFormate.Substring(1, 1))
            {
                case "一": temp.星期 = DayOfWeek.Monday; break;
                case "二": temp.星期 = DayOfWeek.Tuesday; break;
                case "三": temp.星期 = DayOfWeek.Wednesday; break;
                case "四": temp.星期 = DayOfWeek.Thursday; break;
                case "五": temp.星期 = DayOfWeek.Friday; break;
                case "六": temp.星期 = DayOfWeek.Saturday; break;
                case "日": temp.星期 = DayOfWeek.Sunday; break;
            }
            #endregion
            #region 转化周
            //截取周数信息
            var 周数的起始位置 = OldTimeFormate.IndexOf("{") + 1;
            var 周数的结束位置 = OldTimeFormate.IndexOf("}");
            var 周数信息 = OldTimeFormate.Substring(周数的起始位置, 周数的结束位置 - 周数的起始位置);
            var 周数 = Regex.Replace(周数信息, @"[^0-9-]+", "");
            //得到开始和结束
            temp.开始周 = int.Parse(周数.Substring(0, 周数.IndexOf("-") - 0));
            temp.结束周 = int.Parse(周数.Substring(周数.IndexOf("-") + 1, 周数.Length - 周数.IndexOf("-") - 1));
            //得到单双周
            if (OldTimeFormate.Contains("|"))
            {
                temp.单双周 = OldTimeFormate.Substring(OldTimeFormate.IndexOf("|") + 1, 1);
            }
            #endregion
            #region 转化节
            string[] 节数 = OldTimeFormate.Substring(OldTimeFormate.IndexOf("第") + 1, OldTimeFormate.IndexOf("节") - OldTimeFormate.IndexOf("第") - 1).Split(new char[] { ',' });
            foreach (string 节 in 节数)
            {
                temp.节.Add(int.Parse(节));
            }
            #endregion
            return temp;
        }
    }
    /// <summary>
    /// 一个房间信息
    /// </summary>
    public class RoomData
    {
        public bool InfoEmpty = true;//用于判断是否已经下载了信息
        public string RoomNum { get; set; }//教室编号
        public List<Coursedata> Coursedates;//教室包含的所有课程
        public Show_Roomdate GetShowData(DateTime Time)
        {
            var ReRoom = new Show_Roomdate();
            if (Coursedates != null)
            {
                foreach (Coursedata course in Coursedates)
                {
                    CourseTimeFormate courseTime = CourseTimeFormate.Parse(course.CourseTime);
                    if (EqualsTime(courseTime, Time))
                    {//非空教室显示信息
                        ReRoom.CourseName = course.CourseName;
                        ReRoom.CourseNum = course.CourseNum;
                        ReRoom.CourseTime = 转化为显示的时间(Time);
                        ReRoom.TeacherName = course.TeacherName;
                        ReRoom.RoomNum = RoomNum;
                        return ReRoom;
                    }
                }
            }
            //空教室显示信息
            ReRoom.CourseName = "空教室";
            ReRoom.RoomNum = RoomNum;
            ReRoom.CourseTime = 转化为显示的时间(Time);
            return ReRoom;
        }
        private static bool EqualsTime(CourseTimeFormate CourseTime, DateTime SetTime)
        {
            if (是否在课程周数内(CourseTime, SetTime) && 星期是否一样(CourseTime, SetTime) && 是否在课程节数内(CourseTime, SetTime))//判断周数符合//判断星期符合//判断节符合
                return true;
            else
                return false;
        }
        private static bool 是否在课程周数内(CourseTimeFormate CourseTime, DateTime SetTime)
        {
            int 目标周数;
            if (SetTime.Month >= 9)
            {
                DateTime 冬 = new DateTime(SetTime.Year, 9, 1);
                目标周数 = (SetTime.DayOfYear - 冬.DayOfYear)/ 7;
                if ((SetTime.DayOfYear - 冬.DayOfYear) % 7 > 0)
                    目标周数 += 1;
             }
            else
            {
                DateTime 夏 = new DateTime(SetTime.Year, 2, 21);
                目标周数 = (SetTime.DayOfYear - 夏.DayOfYear)/7;
                if ((SetTime.DayOfYear - 夏.DayOfYear) % 7 > 0)
                    目标周数 += 1;
            }
            if (目标周数 >= CourseTime.开始周 && 目标周数 <= CourseTime.结束周) return true;
            return false;
        }
        private static bool 星期是否一样(CourseTimeFormate CourseTime, DateTime SetTime)
        {
            if (SetTime.DayOfWeek == CourseTime.星期) return true;
            else return false;
        }
        private static bool 是否在课程节数内(CourseTimeFormate CourseTime, DateTime SetTime)
        {
            float 目标节=1;
            var TargetTime =TimeSpan.Parse(SetTime.Hour.ToString() + ":" + SetTime.Minute.ToString());
            foreach (TimeSpan time in TimeNode)
            {
                if (TargetTime > time) 目标节 += 0.5f;
            }
            foreach (int 节 in CourseTime.节)
            {
                if (目标节 >= 节 && 目标节 <= 节 + 1) return true;
            }
            return false;
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
        private static string 转化为显示的时间(DateTime OldTimeFormate)
        {
            float 目标节 = 1;
            var TargetTime = TimeSpan.Parse(OldTimeFormate.Hour.ToString() + ":" + OldTimeFormate.Minute.ToString());
            foreach (TimeSpan time in TimeNode)
            {
                if (TargetTime > time) 目标节 += 0.5f;
            }
            目标节 = (((int)Math.Floor(目标节)) - 1) * 2;
            if (目标节 >= TimeNode.Length - 1) 目标节 = TimeNode.Length - 2;
            string 起始 = TimeNode[(int)目标节].Hours.ToString()+":"+ TimeNode[(int)目标节].Minutes.ToString();
            string 结束 = TimeNode[(int)目标节 + 1].Hours.ToString() + ":" + TimeNode[(int)目标节+1].Minutes.ToString();
            return 起始 + "~"+ 结束;
        }
    }
}