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
        Context Context;
        public Detail(Context Context)
        {
            this.Context = Context;
        }
        public void ShowDetail(RoomData room)
        {
            Android.Support.V7.App.AlertDialog.Builder customizeDialog = new Android.Support.V7.App.AlertDialog.Builder(Context);
            View dialogView = LayoutInflater.From(Context).Inflate(Resource.Layout.Detail, null);
            dialogView.FindViewById<TextView>(Resource.Id.RoomNum).Text = room.RoomNum;
            ListView list = dialogView.FindViewById<ListView>(Resource.Id.Detail_ListView);
            list.Adapter = new DetailListViewAdapter((Activity)Context, room.Coursedates);
            //customizeDialog.SetTitle("我是一个自定义Dialog");
            customizeDialog.SetView(dialogView);
            customizeDialog.Show();
        }
    }
}