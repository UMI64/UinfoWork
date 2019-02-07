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
using Android.Support.V7.Widget;
using Uinfo.Tools;
namespace Uinfo
{
    public class Detail
    {
        private static int CardHeight = 45;
        private static int NumberWidth = 19;
        readonly Context Context;
        RoomData NowRoom;
        View dialogView;
        int Week;
        public Detail(Context Context)
        {
            this.Context = Context; 
        }
        public void ShowDetail(RoomData room)
        {
            NowRoom = room;
            Android.Support.V7.App.AlertDialog.Builder customizeDialog = new Android.Support.V7.App.AlertDialog.Builder(Context);
            dialogView = LayoutInflater.From(Context).Inflate(Resource.Layout.Detail, null);
            dialogView.FindViewById<TextView>(Resource.Id.RoomNum).Text = room.RoomNum;
            CreateLeftView();
            CreateView(room);
            customizeDialog.SetView(dialogView);
            customizeDialog.Show();
        }
        private void CreateLeftView()
        {
            //动态生成课程表左侧的节数视图
            LinearLayout classNumberLayout = dialogView.FindViewById<LinearLayout>(Resource.Id.class_number_layout);
            View view;
            TextView text;
            for (int i = 1; i <= 11; i++)
            {
                view = LayoutInflater.From(Context).Inflate(Resource.Layout.Detail_ClassNumber, null);
                LinearLayout.LayoutParams p = new LinearLayout.LayoutParams(DpToPx(NumberWidth), DpToPx(CardHeight));
                view.LayoutParameters = p;
                text = view.FindViewById<TextView>(Resource.Id.text_view);
                text.Text= i.ToString();
                classNumberLayout.AddView(view);
            }
        }
        private void CreateView(RoomData Room)
        {
            Week = TimeTools.计算当前周(((MainActivity)Context).conditions.Time);
            RelativeLayout day;
            int CourseCount = 0;
            foreach (var Course in Room.Coursedates)
            {
                switch (Course.NewCourseTime.星期)
                {
                    case DayOfWeek.Monday: day = (RelativeLayout)dialogView.FindViewById(Resource.Id.monday); break;
                    case DayOfWeek.Tuesday: day = (RelativeLayout)dialogView.FindViewById(Resource.Id.tuesday); break;
                    case DayOfWeek.Wednesday: day = (RelativeLayout)dialogView.FindViewById(Resource.Id.wednesday); break;
                    case DayOfWeek.Thursday: day = (RelativeLayout)dialogView.FindViewById(Resource.Id.thursday); break;
                    case DayOfWeek.Friday: day = (RelativeLayout)dialogView.FindViewById(Resource.Id.friday); break;
                    case DayOfWeek.Saturday: day = (RelativeLayout)dialogView.FindViewById(Resource.Id.saturday); break;
                    case DayOfWeek.Sunday: day = (RelativeLayout)dialogView.FindViewById(Resource.Id.weekday); break;
                    default: day = (RelativeLayout)dialogView.FindViewById(Resource.Id.weekday); break;
                }
                for (var count = 0; count < day.ChildCount; count++)
                {
                    var V = day.GetChildAt(count);
                    var 节 = (PxToDp((int)V.GetY()) + 10) / CardHeight + 1;
                    if (节 == Course.NewCourseTime.节[0])//重合
                    {
                        #region 把这节课添加到这个卡片里面
                        V.Tag += "/" + CourseCount;
                        #endregion
                        #region 设定显示的卡片
                        #endregion
                        goto cc;
                    }
                }
                CardView view = (CardView)LayoutInflater.From(Context).Inflate(Resource.Layout.Detail_CourseCard, null); //加载单个课程布局
                view.SetY(DpToPx(CardHeight) * (Course.NewCourseTime.节[0] - 1)); //设置开始高度,即第几节课开始
                LinearLayout.LayoutParams p = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, (Course.NewCourseTime.节.Last() - Course.NewCourseTime.节.First() + 1) * DpToPx(CardHeight) - 2); //设置布局高度,即跨多少节课
                view.LayoutParameters = p;
                TextView text = view.FindViewById<TextView>(Resource.Id.text_view);
                text.Text = Course.CourseName + "\n" + Course.TeacherName; //显示课程名
                if (Coursedata.是否在课程周数内(Course.NewCourseTime, Week))
                {
                    view.SetCardBackgroundColor(Android.Graphics.Color.ParseColor("#6495ED"));
                }
                view.Tag = CourseCount.ToString();
                view.Click += (object sender, EventArgs e) =>
                {
                    string[] ClassTag = ((CardView)sender).Tag.ToString().Split("/");
                    if (ClassTag.Length > 1)
                    {
                        var Tag = int.Parse(ClassTag[1]);
                        TextView T = view.FindViewById<TextView>(Resource.Id.text_view);
                        T.Text = NowRoom.Coursedates[Tag].CourseName + "\n" + NowRoom.Coursedates[Tag].TeacherName+"\n"+ NowRoom.Coursedates[Tag].CourseTime;
                        var FirstTag = ClassTag[0];
                        for (var count = 0; count < ClassTag.Length - 1; count++)
                        {
                            ClassTag[count] = ClassTag[count + 1];
                        }
                        ClassTag[ClassTag.Length - 1] = FirstTag;
                        string NewTag = string.Empty;
                        foreach (var t in ClassTag)
                        {
                            NewTag += t + "/";
                        }
                        ((CardView)sender).Tag = NewTag.Substring(0, NewTag.Length - 1);
                    }
                };
                day.AddView(view);
                cc: CourseCount++;
            }
        }

        private int DpToPx(int Dp)
        {
            return (int)(Dp * Context.Resources.DisplayMetrics.Density);
        }
        private int PxToDp(int Px)
        {
            return (int)(Px / Context.Resources.DisplayMetrics.Density);
        }
    }
}