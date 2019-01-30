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
    public class DetailListViewAdapter : BaseAdapter<Coursedata>
    {

        List<Coursedata> Course;
        Activity activity;

        public DetailListViewAdapter(Activity context, List<Coursedata> values) : base()
        {
            activity = context;
            Course = values;
        }

        public override Coursedata this[int position]
        {
            get { return Course[position]; }
        }

        public override int Count
        {
            get { return Course.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View v = convertView;
            if (v == null)
                v = activity.LayoutInflater.Inflate(Resource.Layout.Detail_ListItem, null);
            v.FindViewById<TextView>(Resource.Id.CourseTime).Text = Course[position].CourseTime;
            v.FindViewById<TextView>(Resource.Id.CourseName).Text = Course[position].CourseName;
            v.FindViewById<TextView>(Resource.Id.CourseNum).Text = Course[position].CourseNum;
            v.FindViewById<TextView>(Resource.Id.TeacherName).Text = Course[position].TeacherName;
            return v;
        }
    }
}