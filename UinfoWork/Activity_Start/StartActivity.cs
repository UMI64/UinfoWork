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
    [Activity(Label = "Uinfo", Theme = "@style/StartTheme", MainLauncher = true)]
    public class StartActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Intent intent = new Intent(this, typeof(MainActivity));
            //启动主界面
            StartActivity(intent);
            Finish();
        }
    }
}