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
    [Activity(Label = "TuCaoActivity")]
    public class TuCaoActivity : AppCompatActivity
    {
        WebView webView;
        ProgressBar progressBar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TuCao);
            #region 设置ToolBar
            SetToolBar();
            #endregion
            progressBar= FindViewById<ProgressBar>(Resource.Id.progressBar);

            webView = FindViewById<WebView>(Resource.Id.webView);
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.LoadWithOverviewMode = true;
            webView.Settings.SetRenderPriority(WebSettings.RenderPriority.High);
            webView.SetWebViewClient(new MyWebViewClient(webView, progressBar));
            webView.SetWebChromeClient(new MyWebChromeClient(progressBar));
            webView.LoadUrl("https://github.com/UMI64/UinfoWork/issues");
        }
        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && webView.CanGoBack())
            {
                webView.GoBack();// 返回前一个页面
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }
        public class MyWebViewClient : WebViewClient
        {
            WebView webView;
            ProgressBar progressBar;
            public MyWebViewClient(WebView webView, ProgressBar progressBar)
            {
                this.webView = webView;
                this.progressBar = progressBar;
            }
            public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
            {
                webView.LoadUrl(request.Url.ToString());
                return true;
            }
            public override void OnPageStarted(WebView view, string url, Bitmap favicon)
            {
                progressBar.Visibility = ViewStates.Visible;
                base.OnPageStarted(view, url, favicon);
            }
            public override void OnPageFinished(WebView view, string url)
            {
                progressBar.Visibility = ViewStates.Gone;
                base.OnPageFinished(view, url);
            }
        }
        public class MyWebChromeClient : WebChromeClient
        {
            ProgressBar progressBar;
            public MyWebChromeClient(ProgressBar progressBar)
            {
                this.progressBar = progressBar;
            }
            public override void OnProgressChanged(WebView view, int newProgress)
            {
                progressBar.SetProgress(newProgress,true);
                base.OnProgressChanged(view, newProgress);
            }

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