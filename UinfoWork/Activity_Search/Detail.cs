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

namespace Uinfo
{
    public class Detail
    {
        readonly Context Context;
        View dialogView;
        int Week=2;
        public Detail(Context Context)
        {
            this.Context = Context;
        }
        public void ShowDetail(RoomData room)
        {
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
                LinearLayout.LayoutParams p = new LinearLayout.LayoutParams(PxToDp(19), PxToDp(45));
                view.LayoutParameters = p;
                text = view.FindViewById<TextView>(Resource.Id.text_view);
                text.Text= i.ToString();
                classNumberLayout.AddView(view);
            }
        }
        private void CreateView(RoomData Room)
        {
            RelativeLayout day;
            foreach (var Course in Room.Coursedates)
            {
                if (Coursedata.是否在课程周数内(Course.NewCourseTime, Week))
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
                        default: day = (RelativeLayout)dialogView.FindViewById(Resource.Id.weekday);break;
                    }
                    View view = LayoutInflater.From(Context).Inflate(Resource.Layout.Detail_CourseCard, null); //加载单个课程布局
                    view.SetY(PxToDp(45) * (Course.NewCourseTime.节[0] - 1)); //设置开始高度,即第几节课开始
                    LinearLayout.LayoutParams p = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, (Course.NewCourseTime.节.Last() - Course.NewCourseTime.节.First() + 1) * PxToDp(45)-2); //设置布局高度,即跨多少节课
                    view.LayoutParameters = p;
                    TextView text = view.FindViewById<TextView>(Resource.Id.text_view);
                    text.Text = Course.CourseName + "\n" + Course.TeacherName; //显示课程名
                    day.AddView(view);
                }
            }
        }
        private int PxToDp(int Px)
        {
            return (int)(Px * Context.Resources.DisplayMetrics.Density);
        }
    }
}