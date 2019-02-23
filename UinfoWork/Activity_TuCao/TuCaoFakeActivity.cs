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
using Android.Graphics;

namespace Uinfo.Activity_TuCao
{
    [Activity(Label = "TuCaoFakeActivity")]
    public class TuCaoFakeActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TuCaoFake);
            #region 设置ToolBar
            SetToolBar();
            #endregion
            Button SendButton = FindViewById<Button>(Resource.Id.SendButton);
            SendButton.Click += (object sender, EventArgs e) =>
            {
                Toast ttoast = Toast.MakeText(this, "发送成功", ToastLength.Long);
                ttoast.Show();
                Finish();
            }; 
        }

        private void SetToolBar()
        {
            SupportActionBar.Title = "吐槽";
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }
        /// <summary>
        /// Toolbar的事件---返回
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}