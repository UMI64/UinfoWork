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

        TextView EmptyRoomSwitch;
        Button TimeLockButton;
        TextView WeekText;
        TextView TimeText;
        public Condition(Context context, SearchRoom searchRoom, RecyclerView.Adapter roomlist_adapter)
        {
            this.context = context;
            this.searchRoom = searchRoom;
            this.roomlist_adapter = roomlist_adapter;
            #region 中二病
            TextView 中二病 = EmptyRoomSwitch = ((Activity)context).FindViewById<TextView>(Resource.Id.中二病Text);
            中二病.Click += (sender, args) =>
            {
                if (中二病Flag)
                {
                    中二病.Text = "普通模式";
                    中二病Flag = false;
                    SetTime();
                }
                else
                {
                    中二病.Text = "中二病模式";
                    中二病Flag = true;
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
            EmptyRoomSwitch = ((Activity)context).FindViewById<TextView>(Resource.Id.EmptyRoomSwitch);
            EmptyRoomSwitch.Click += (sender, args) =>
            {
                if (EmptyRoomFlag)
                {
                    EmptyRoomFlag = false;
                    EmptyRoomSwitch.Text = "全部教室";
                }
                else
                {
                    EmptyRoomFlag = true;
                    EmptyRoomSwitch.Text = "空教室";
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
            TimeLockButton.Click += (sender, args) =>//锁定时间按钮
            {
                if (TimeLockFlag)//现在是被锁住了的
                {
                    TimeLockButton.Background = (BitmapDrawable)context.GetDrawable(Resource.Drawable.LockOpen);
                    TimeLockFlag = false;//解锁
                }
                else
                {
                    TimeLockButton.Background = (BitmapDrawable)context.GetDrawable(Resource.Drawable.LockUp);
                    TimeLockFlag = true;//锁住
                    SetTimeToNow();//设定为系统时间
                    SetTime();//应用更改
                }
            };
            WeekText.Click += (sender, args) =>
            {
                if (!TimeLockFlag)
                {
                    ChangeWeek();
                }
            };//改变周
            TimeText.Click += (sender, args) =>
            {
                if (!TimeLockFlag)
                {
                    ChangeTime();
                }
            };//改变时间
            #endregion
        }
        private void ChangeWeek()
        {
            Android.Support.V7.App.AlertDialog.Builder customizeDialog = new Android.Support.V7.App.AlertDialog.Builder(context);
            View dialogView = LayoutInflater.From(context).Inflate(Resource.Layout.ChangeWeek, null);
            dialogView.FindViewById<Button>(Resource.Id.OKButon).Click += (sender, args) =>
            {
                int weeks = int.Parse(dialogView.FindViewById<EditText>(Resource.Id.ChangeWeeksText).Text);
                int week = int.Parse(dialogView.FindViewById<EditText>(Resource.Id.ChangeWeekText).Text);
                DateTime time;
                if (month >= 9)
                    time=new DateTime(year, 9, 1);
                else
                    time = new DateTime(year, 2, 21);
                time=time.AddDays((weeks-1) * 7);
                time=time.AddDays(week-Convert.ToInt16(time.DayOfWeek.ToString("D")));
                year = time.Year;
                month = time.Month;
                day = time.Day;
                SetTime();//应用更改
            };
            customizeDialog.SetView(dialogView);
            customizeDialog.Show();
        }
        private void ChangeTime()
        {
            Android.Support.V7.App.AlertDialog.Builder customizeDialog = new Android.Support.V7.App.AlertDialog.Builder(context);
            View dialogView = LayoutInflater.From(context).Inflate(Resource.Layout.ChangeTime, null);
            dialogView.FindViewById<EditText>(Resource.Id.year).Text=year.ToString();
            dialogView.FindViewById<EditText>(Resource.Id.month).Text = month.ToString(); ;
            dialogView.FindViewById<EditText>(Resource.Id.day).Text = day.ToString(); ;
            dialogView.FindViewById<EditText>(Resource.Id.hour).Text = hour.ToString(); ;
            dialogView.FindViewById<EditText>(Resource.Id.minute).Text = minute.ToString(); ;
            dialogView.FindViewById<Button>(Resource.Id.OKButon).Click += (sender, args) =>
            {
                year = int.Parse(dialogView.FindViewById<EditText>(Resource.Id.year).Text);
                month = int.Parse(dialogView.FindViewById<EditText>(Resource.Id.month).Text);
                day = int.Parse(dialogView.FindViewById<EditText>(Resource.Id.day).Text);
                hour = int.Parse(dialogView.FindViewById<EditText>(Resource.Id.hour).Text);
                minute = int.Parse(dialogView.FindViewById<EditText>(Resource.Id.minute).Text);
                SetTime();
            };
            customizeDialog.SetView(dialogView);
            customizeDialog.Show();
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
            int Weeks;
            if (month >= 9)
            {
                DateTime 冬 = new DateTime(year, 9, 1);
                冬 = 冬.AddDays(1 - Convert.ToInt16(冬.DayOfWeek.ToString("D")));
                Weeks = (time.DayOfYear - 冬.DayOfYear) / 7 + 1;
            }
            else
            {
                DateTime 夏 = new DateTime(year, 2, 21);
                夏 = 夏.AddDays(1 - Convert.ToInt16(夏.DayOfWeek.ToString("D")));
                Weeks = (time.DayOfYear - 夏.DayOfYear) / 7 +1;
            }
            if (Weeks <= 0) WeekText.Text = "学习使我快乐";
            else WeekText.Text = "第" + Weeks.ToString() + "周 " + NumberToWeekChinese(Convert.ToInt16(time.DayOfWeek.ToString("D")));
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