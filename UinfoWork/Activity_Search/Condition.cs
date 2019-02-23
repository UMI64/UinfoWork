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
using Android.Util;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Uinfo.Tools;
namespace Uinfo
{
    public class Condition
    {
        Context context;
        SearchRoom searchRoom;
        RecyclerView.Adapter roomlist_adapter;
        public bool EmptyRoomFlag = true;
        public bool TimeLockFlag = true;
        public bool 中二病Flag = false;
        private int year = 2018;
        private int month = 2018;
        private int day = 2018;
        private int hour = 2018;
        private int minute = 2018;
        public DateTime Time
        {
            get
            {
                if (TimeLockFlag) return DateTime.Now;
                DateTime date;
                try
                {
                    date = new DateTime(year, month, day, hour, minute, 20);
                }
                catch
                {
                    date = new DateTime(year, month, 10);
                }
                return date;
            }
        }

        LinearLayout EmptyRoomSwitch;
        TextView EmptyRoomText;
        Button TimeLockButton;
        TextView WeekText;
        TextView TimeText;
        public Condition(Context context, SearchRoom searchRoom, RecyclerView.Adapter roomlist_adapter)
        {
            this.context = context;
            this.searchRoom = searchRoom;
            this.roomlist_adapter = roomlist_adapter;
            #region 中二病
            TextView 中二病 = ((Activity)context).FindViewById<TextView>(Resource.Id.中二病Text);
            ImageButton LeftMenuHead = ((Activity)context).FindViewById<ImageButton>(Resource.Id.imageButton1);
            中二病.Click += (sender, args) =>
            {
                if (中二病Flag)
                {
                    中二病.Text = "普通模式";
                    中二病Flag = false;
                    LeftMenuHead.SetImageDrawable(context.GetDrawable(Resource.Drawable.ic_launcher_foreground_HD));
                    SetTime();
                }
                else
                {
                    中二病.Text = "中二病模式";
                    中二病Flag = true;
                    LeftMenuHead.SetImageDrawable(context.GetDrawable(Resource.Drawable.APPButtonImage));
                    SetTime();
                }
            };
            #endregion
            #region 监听时间改变
            var intentFilter = new IntentFilter();
            intentFilter.AddAction(Intent.ActionTimeTick);//每分钟变化
            TimeChangeReceiver timeChangeReceiver = new TimeChangeReceiver();
            context.RegisterReceiver(timeChangeReceiver, intentFilter);
            timeChangeReceiver.condition = this;
            #endregion
            #region  空教室
            EmptyRoomSwitch = ((Activity)context).FindViewById<LinearLayout>(Resource.Id.EmptyRoomSwitch);
            EmptyRoomText = ((Activity)context).FindViewById<TextView>(Resource.Id.EmptyRoomText);
            EmptyRoomSwitch.Click += (sender, args) =>
            {
                if (EmptyRoomFlag)
                {
                    EmptyRoomFlag = false;
                    EmptyRoomText.Text = "全部教室";
                }
                else
                {
                    EmptyRoomFlag = true;
                    EmptyRoomText.Text = "空教室";
                }
                searchRoom.ResetShowDatas(this);//显示数据应用更改
                roomlist_adapter.NotifyDataSetChanged();//显示数据刷新
            };
            #endregion
            #region 时间
            TimeLockButton = ((Activity)context).FindViewById<Button>(Resource.Id.TimeLockButton);
            WeekText = ((Activity)context).FindViewById<TextView>(Resource.Id.WeekText);
            TimeText = ((Activity)context).FindViewById<TextView>(Resource.Id.TimeText);
            SetTimeToNow();//初始化时间
            SetTime();//应用更改
            WeekText.Click += (sender, args) =>
            {
                ChangeWeek();
            };//改变周
            TimeText.Click += (sender, args) =>
            {
                ChangeTime();
            };//改变时间
            #endregion
        }
        private void ChangeTime()
        {
            Android.Support.V7.App.AlertDialog.Builder customizeDialog = new Android.Support.V7.App.AlertDialog.Builder(context);
            View dialogView = LayoutInflater.From(context).Inflate(Resource.Layout.ChangeTime, null);
            customizeDialog.SetView(dialogView);
            var dig = customizeDialog.Create();
            dialogView.FindViewById<EditText>(Resource.Id.year).Text = year.ToString();
            dialogView.FindViewById<EditText>(Resource.Id.month).Text = month.ToString();
            dialogView.FindViewById<EditText>(Resource.Id.day).Text = day.ToString();
            dialogView.FindViewById<EditText>(Resource.Id.hour).Text = hour.ToString();
            dialogView.FindViewById<EditText>(Resource.Id.minute).Text = minute.ToString();
            dialogView.FindViewById<Button>(Resource.Id.OKButon).Click += (sender, args) =>
            {
                if (!TimeLockFlag)
                {
                    int year = int.Parse(dialogView.FindViewById<EditText>(Resource.Id.year).Text);
                    int month = int.Parse(dialogView.FindViewById<EditText>(Resource.Id.month).Text);
                    int day = int.Parse(dialogView.FindViewById<EditText>(Resource.Id.day).Text);
                    int hour = int.Parse(dialogView.FindViewById<EditText>(Resource.Id.hour).Text);
                    int minute = int.Parse(dialogView.FindViewById<EditText>(Resource.Id.minute).Text);
                    if (CheckTimeFormate(year, month, day, hour, minute))
                    {
                        this.year = year;
                        this.month = month;
                        this.day = day;
                        this.hour = hour;
                        this.minute = minute;
                        SetTime();//应用更改
                        dig.Cancel();
                    }
                    else
                    {
                        Toast ttoast = Toast.MakeText(context, "失败：时间格式错误", ToastLength.Long);
                        ttoast.Show();
                    }
                }
                else
                {
                    Toast toast = Toast.MakeText(context, "失败：当前时间跟随系统", ToastLength.Long);
                    toast.Show();
                }
            };
            TextView FollowingSystemText = dialogView.FindViewById<TextView>(Resource.Id.FollowingSystemText);
            FollowingSystemText.Text = TimeLockFlag? "时间跟随系统": "时间自由设定";
            FollowingSystemText.Click += (sender, args) =>
            {
                if (TimeLockFlag)//现在是被锁住了的
                {
                    TimeLockButton.Background = (BitmapDrawable)context.GetDrawable(Resource.Drawable.LockOpen);
                    FollowingSystemText.Text = "时间自由设定";
                    TimeLockFlag = false;//解锁
                }
                else
                {
                    TimeLockButton.Background = (BitmapDrawable)context.GetDrawable(Resource.Drawable.LockUp);
                    FollowingSystemText.Text = "时间跟随系统";
                    TimeLockFlag = true;//锁住
                    SetTimeToNow();//设定为系统时间
                    SetTime();//应用更改
                }
            };
            Window window = dig.Window;
            window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));
            dig.Show();

        }
        private void ChangeWeek()
        {
            Android.Support.V7.App.AlertDialog.Builder customizeDialog = new Android.Support.V7.App.AlertDialog.Builder(context);
            View dialogView = LayoutInflater.From(context).Inflate(Resource.Layout.ChangeWeek, null);
            customizeDialog.SetView(dialogView);
            var dig = customizeDialog.Create();
            dialogView.FindViewById<Button>(Resource.Id.OKButon).Click += (sender, args) =>
            {
                if (!TimeLockFlag)
                {
                    int weeks = int.Parse(dialogView.FindViewById<EditText>(Resource.Id.ChangeWeeksText).Text);
                    int week = int.Parse(dialogView.FindViewById<EditText>(Resource.Id.ChangeWeekText).Text);
                    DateTime time;
                    var 夏time = TimeTools.计算本学期夏开学时间(Time);
                    var 冬time = TimeTools.计算本学期冬开学时间(Time);
                    if (Time > 夏time) time = 夏time;
                    else time = 冬time;
                    time = time.AddDays((weeks - 1) * 7);
                    time = time.AddDays(week - Convert.ToInt16(time.DayOfWeek.ToString("D")));
                    int year = time.Year;
                    int month = time.Month;
                    int day = time.Day;
                    if (CheckTimeFormate(year, month, day, hour, minute))
                    {
                        this.year = year;
                        this.month = month;
                        this.day = day;
                        SetTime();//应用更改
                        dig.Cancel();
                    }
                    else
                    {
                        Toast ttoast = Toast.MakeText(context, "失败：时间格式错误", ToastLength.Long);
                        ttoast.Show();
                    }
                }
                else
                {
                    Toast toast = Toast.MakeText(context, "失败：当前时间跟随系统", ToastLength.Long);
                    toast.Show();
                }
            };
            TextView FollowingSystemText = dialogView.FindViewById<TextView>(Resource.Id.FollowingSystemText);
            FollowingSystemText.Text = TimeLockFlag ? "时间跟随系统" : "时间自由设定";
            FollowingSystemText.Click += (sender, args) =>
            {
                if (TimeLockFlag)//现在是被锁住了的
                {
                    TimeLockButton.Background = (BitmapDrawable)context.GetDrawable(Resource.Drawable.LockOpen);
                    FollowingSystemText.Text = "时间自由设定";
                    TimeLockFlag = false;//解锁
                }
                else
                {
                    TimeLockButton.Background = (BitmapDrawable)context.GetDrawable(Resource.Drawable.LockUp);
                    FollowingSystemText.Text = "时间跟随系统";
                    TimeLockFlag = true;//锁住
                    SetTimeToNow();//设定为系统时间
                    SetTime();//应用更改
                }
            };
            Window window = dig.Window;
            window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));
            dig.Show();
        }
        private bool CheckTimeFormate(int year,int month,int day,int hour,int minute)
        {
            try
            {
                DateTime time = new DateTime(year, month, day, hour, minute, 20);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void SetTimeToNow()
        {
            if (TimeLockFlag)
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
                day = DateTime.Now.Day;
                hour = DateTime.Now.Hour;
                minute = DateTime.Now.Minute;
            }
        }//把时间设定为系统时间
        public void SetTime()
        {
            DateTime time=new DateTime(year, month, day, hour, minute,20);
            TimeText.Text = year.ToString() + "年" + month.ToString() + "月" + day.ToString() + "日   " + hour.ToString() + ":" + minute.ToString();

            var 冬Weeks=TimeTools.计算冬的周(time);
            var 夏Weeks= TimeTools.计算夏的周(time);
            if (冬Weeks <= 20) WeekText.Text = "第" + 冬Weeks.ToString() + "周 " + NumberToWeekChinese(Convert.ToInt16(time.DayOfWeek.ToString("D")));
            else if (夏Weeks < 1) WeekText.Text = "距离开学还有" + (TimeTools.计算本学期夏开学时间(time) - time).Days / 7 + "周";
            else if (夏Weeks <= 20) WeekText.Text = "第" + 夏Weeks.ToString() + "周 " + NumberToWeekChinese(Convert.ToInt16(time.DayOfWeek.ToString("D")));
            else WeekText.Text = "距离开学还有" + (TimeTools.计算本学期冬开学时间(time).AddYears(1) - time).Days / 7 + "周";
            searchRoom.ResetShowDatas(this);//显示数据应用更改
            roomlist_adapter.NotifyDataSetChanged();//显示数据刷新
        }//应用更改的时间并更新显示的数据
        private string NumberToWeekChinese(int week)
        {
            if (中二病Flag)
                return new string[] { "日曜日", "月曜日", "火曜日", "水曜日", "木曜日", "金曜日", "土曜日", }[week];
            else
                return new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六", }[week];
        }
    }
    class TimeChangeReceiver : BroadcastReceiver
    {
        public Condition condition { get; set; }
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == Intent.ActionTimeTick)
            {
                condition.SetTimeToNow();//更改设定时间
                condition.SetTime();//应用更改
            }
        }
    }
}