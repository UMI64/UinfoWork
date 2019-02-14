using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Webkit;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Uinfo.Activity_TuCao
{
    [Activity(Label = "TuCaoActivity")]
    public class TuCaoActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TuCao);
            #region 设置ToolBar
            SetToolBar();
            #endregion
            WebView webView= FindViewById<WebView>(Resource.Id.webView);
            webView.LoadUrl("https://github.com/UMI64/UinfoWork/issues");
        }
        private void SetToolBar()
        {
            SupportActionBar.Title = "吐槽";
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }
    }
}