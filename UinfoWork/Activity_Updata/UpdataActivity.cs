using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Support.V4.Content;
using Android.Provider;
using Android.Content.PM;
using Android.Net;
using System.IO;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Uinfo.Updata
{
    [Activity(Label = "UpdataActivity")]
    public class UpdataActivity : AppCompatActivity
    {
        readonly static string APKname = "Uinfo.apk";
        Intent StartInstallintent;
        Verison NewVerison = new Verison();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Updata);
            NewVerison.VersionCode = Intent.GetCharSequenceExtra("VersionCode");
            NewVerison.VersionName = Intent.GetCharSequenceExtra("VersionName");
            NewVerison.VersionDiscription = Intent.GetCharSequenceExtra("VersionDiscription");
            #region 设置ToolBar
            SetToolBar();
            #endregion
            TextView VerisonName = FindViewById<TextView>(Resource.Id.VerisonName);
            VerisonName.Text = NewVerison.VersionName;

            TextView VerisonDiscription = FindViewById<TextView>(Resource.Id.VerisonDiscription);
            try
            {
                JObject jObject = (JObject)JsonConvert.DeserializeObject(NewVerison.VersionDiscription);
                JArray jArray = (JArray)jObject["VerisonDiscription"];
                foreach (var item in jArray)
                {
                    VerisonDiscription.Text += item.ToString() + "\r\n";
                }
            }
            catch
            {
                VerisonDiscription.Text = "获取更新描述失败";
            }
            #region 下载更新按钮
            Button DownloadButton = FindViewById<Button>(Resource.Id.DownloadButton); 
            DownloadButton.Click += (o, e) =>
            {
                DownloadTask downloadTask = new DownloadTask(this);
                downloadTask.Execute("https://github.com/UMI64/UinfoWork/raw/master/UinfoWork.UinfoWork.apk");
            };
            #endregion
        }
        public class DownloadTask : AsyncTask<string, int, string>
        {
            static readonly string Success = "Success";
            static readonly string Failure = "Failure";
            string fileSavePath;
            string filePath;
            UpdataActivity updataActivity;
            bool IsCancel = false;
            ProgressBar progressbar;
            TextView Downloads;
            TextView TimeLeft;
            Dialog dialog;
            int count = 0;
            int Lastcount = 0;
            long length = 0;
            int second=0;
            public DownloadTask(UpdataActivity updataActivity)
            {
                this.updataActivity = updataActivity;
                fileSavePath = (string)updataActivity.GetExternalFilesDir(Environment.DirectoryDownloads);
                filePath = Path.Combine(fileSavePath, APKname);
            }
            protected override void OnPreExecute()
            {
                base.OnPreExecute();
                Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(updataActivity);
                builder.SetTitle("下载中");
                View view = LayoutInflater.From(updataActivity).Inflate(Resource.Layout.UpdataDownloadDialog, null);
                progressbar = (ProgressBar)view.FindViewById(Resource.Id.progressbar);
                Downloads= (TextView)view.FindViewById(Resource.Id.Downloads);
                TimeLeft = (TextView)view.FindViewById(Resource.Id.TimeLeft);
                builder.SetView(view);
                builder.SetNegativeButton("取消",new DownloadDialogClickListener(updataActivity,this));
                dialog = builder.Create();
                dialog.SetCancelable(false);
                dialog.Show();
            }
            private class DownloadDialogClickListener : Java.Lang.Object, IDialogInterfaceOnClickListener
            {
                readonly Context mcontext;
                DownloadTask task;
                public DownloadDialogClickListener(Context context, DownloadTask task)
                {
                    mcontext = context;
                    this.task = task;
                }
                public void OnClick(IDialogInterface dialog, int which)
                {
                    dialog.Cancel();
                    task.IsCancel = true;
                    task.Cancel(true);
                }
            }
            protected override void OnProgressUpdate(params int[] values)
            {
                base.OnProgressUpdate(values);
                progressbar.SetProgress(values[0], true);
                Downloads.Text = (values[1] / 1024).ToString() + "Kb/" + (values[2] / 1024).ToString() + "Kb";
                TimeLeft.Text = "剩余" + second.ToString() + "秒";
            }
            protected override void OnCancelled()
            {
                base.OnCancelled();
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            protected override string RunInBackground(string[] @params)
            {
                Timer aTimer= new Timer(1000);
                string Url = @params[0];
                int[] OutPutDatas =new int[3];
                try
                {
                    if (!Directory.Exists(fileSavePath))
                        Directory.CreateDirectory(fileSavePath);
                    if (File.Exists(filePath))
                    {
                        Verison LocalVerison = Verison.GetLocalVersion(updataActivity);
                        Verison APKVerison = Verison.GetAPKVersion(filePath, updataActivity);
                        if (LocalVerison < APKVerison)//版本小于则安装
                            return Success;
                        else
                        {
                            File.Delete(filePath);
                        }
                        //大于等于则删除
                        //继续下载
                    }
                    System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(Url);

                    System.Net.WebResponse response = request.GetResponse();
                    Stream inputStream = response.GetResponseStream();
                    length = inputStream.Length;
                    byte[] buffer = new byte[1024];
                    Stream outStream = File.Create(filePath);
                    Stream inStream = response.GetResponseStream();
                    aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                    aTimer.AutoReset = true;
                    aTimer.Enabled = true;
                    while (!IsCancel)
                    {
                        int ReadLength = 0;
                        ReadLength = inStream.Read(buffer, 0, buffer.Length);
                        if (ReadLength > 0)
                            outStream.Write(buffer, 0, ReadLength);
                        else
                            break;
                        count += ReadLength;
                        int Progress = (int)(((float)count / length) * 100);
                        // 更新进度条
                        OutPutDatas[0] = Progress;
                        OutPutDatas[1] = count;
                        OutPutDatas[2] = (int)length;
                        PublishProgress(OutPutDatas);
                    }
                    aTimer.Close();
                    outStream.Close();
                    inStream.Close();
                }
                catch (System.Exception e)
                {
                    return e.Message;
                }
                if (IsCancel) return Failure;
                return Success;
            }
            private void OnTimedEvent(object source, ElapsedEventArgs e)
            {
                int different = count - Lastcount;
                second = (int)((length - count)/ different);
                Lastcount = count;
            }
            protected override void OnPostExecute(string result)
            {
                base.OnPostExecute(result);
                if (result == Success)
                    updataActivity.OpenAPK(filePath);
                dialog.Cancel();
            }
        }
        /// <summary>
        /// 设置ToolBar
        /// </summary>
        private void SetToolBar()
        {
            SupportActionBar.Title = "更新";
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
        }
        /// <summary>
        /// 的事件---返回
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
        /// <summary>
        /// //监听权限设置
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="resultCode"></param>
        /// <param name="data"></param>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 1)
            {
                if (resultCode == Result.Ok)
                    ChickPermission();
            }
        }
        /// <summary>
        /// 分安卓版本打开apk
        /// </summary>
        /// <param name="fileSavePath"></param>
        public void OpenAPK(string fileSavePath)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                InstallPackgeAPI28(fileSavePath);
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                //判断版本大于等于7.0
                InstallPackgeAPI26(fileSavePath);
            }
            else
            {
                InstallPackge(fileSavePath);
            }
        }
        /// <summary>
        /// 低安卓版本安装APK
        /// </summary>
        /// <param name="fileSavePath"></param>
        private void InstallPackge(string fileSavePath)
        {
            Java.IO.File file = new Java.IO.File(Uri.Parse(fileSavePath).Path);
            StartInstallintent = new Intent(Intent.ActionView);
            StartInstallintent.SetFlags(ActivityFlags.NewTask);
            var data = Uri.FromFile(file);
            StartInstallintent.SetDataAndType(data, "application/vnd.android.package-archive");
            StartActivity(StartInstallintent);
        }
        /// <summary>
        /// 中安装版本安装APK
        /// </summary>
        /// <param name="fileSavePath"></param>
        private void InstallPackgeAPI26(string fileSavePath)
        {
            Java.IO.File file = new Java.IO.File(Uri.Parse(fileSavePath).Path);
            string filePath = file.AbsolutePath;
            StartInstallintent = new Intent(Intent.ActionView);
            StartInstallintent.SetFlags(ActivityFlags.NewTask);
            var provider = PackageName + ".fileprovider";
            Uri data = FileProvider.GetUriForFile(this, provider, new Java.IO.File(filePath));
            StartInstallintent.SetFlags(ActivityFlags.GrantReadUriPermission);// 给目标应用一个临时授权
            StartInstallintent.SetDataAndType(data, "application/vnd.android.package-archive");
            StartActivity(StartInstallintent);
        }
        /// <summary>
        /// 高安卓版本安装APK
        /// </summary>
        /// <param name="fileSavePath"></param>
        private void InstallPackgeAPI28(string fileSavePath)
        {
            Java.IO.File file = new Java.IO.File(Uri.Parse(fileSavePath).Path);
            string filePath = file.AbsolutePath;
            StartInstallintent = new Intent(Intent.ActionView);
            StartInstallintent.SetFlags(ActivityFlags.NewTask);
            var provider = PackageName + ".fileprovider";
            Uri data = FileProvider.GetUriForFile(this, provider, new Java.IO.File(filePath));
            StartInstallintent.SetFlags(ActivityFlags.GrantReadUriPermission);// 给目标应用一个临时授权
            StartInstallintent.SetDataAndType(data, "application/vnd.android.package-archive");
            ChickPermission();
        }
        /// <summary>
        /// 检查权限并安装
        /// </summary>
        public void ChickPermission()
        {
            if (!PackageManager.CanRequestPackageInstalls())//是否有8.0安装权限
            {
                new Android.App.AlertDialog.Builder(this)
                        .SetTitle("权限申请")
                        .SetMessage("亲，没有权限我会崩溃，请把权限赐予我吧！")
                        .SetPositiveButton("赏给你", new GetPremessionDialog(this, true))
                        .SetNegativeButton("取消", new GetPremessionDialog(this, false))
                        .Show();
            }
            else StartActivity(StartInstallintent);//有权限直接安装
        }
        /// <summary>
        /// 获取权限的对话框 跳转到设置
        /// </summary>
        public class GetPremessionDialog : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            Context mcontext;
            bool what;
            public GetPremessionDialog(Context context, bool what)
            {
                mcontext = context;
                this.what = what;
            }
            public void OnClick(IDialogInterface dialog, int which)
            {
                dialog.Cancel();
                if (what)
                {
                    Uri packageURI = Uri.Parse("package:" + mcontext.PackageName);
                    Intent intent = new Intent(Settings.ActionManageUnknownAppSources, packageURI);
                    ((Activity)mcontext).StartActivityForResult(intent, 1);
                }
            }
        }
    }
}