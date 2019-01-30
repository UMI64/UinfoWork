using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Content;
using Java.IO;
using Android.Database;
using Android.Support.V4.Content;
using Android.Provider;
using Android.Content.PM;
using Android.Net;

namespace Uinfo.Updata
{
    [Activity(Label = "UpdataActivity")]
    public class UpdataActivity : AppCompatActivity
    {
        long DownloadID = 0;
        readonly string APKname = "Uinfo.apk";
        Intent StartInstallintent;
        Verison NewVerison=new Verison();
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
            #region 下载更新按钮
            Button DownloadButton = FindViewById<Button>(Resource.Id.DownloadButton);
            DownloadButton.Click += (o, e) =>
            {
                DownLoadApk("Uinfo", "更新", "https://github.com/UMI64/UinfoWork/raw/master/UinfoWork.UinfoWork.apk");
            };
            #endregion
        }
        /// <summary>
        /// 设置ToolBar
        /// </summary>
        private void SetToolBar()
        {
            SupportActionBar.Subtitle = "更新";
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
        ///下载apk
        /// </summary>
        /// <param name="context">上下文对象</param>
        /// <param name="title">程序的名字</param>
        /// <param name="url">下载的url地址</param>
        /// <returns></returns>
        public void DownLoadApk(string title, string Description, string url)
        {
            var fileSavePath = Uri.FromFile(new File(GetExternalFilesDir(Environment.DirectoryDownloads).AbsolutePath)) + "/" + APKname;
            File file = new File(Uri.Parse(fileSavePath).Path);
            if (file.Exists())//如果文件存
            {
                Verison LocalVerison = Verison.GetLocalVersion(this);
                Verison APKVerison =Verison.GetAPKVersion(file.AbsolutePath, this);
                if (LocalVerison < APKVerison)//版本小于则安装
                {
                    OpenAPK(fileSavePath);
                    return;//退出下载
                }
                else
                    file.Delete();
                //大于等于则删除
                //继续下载
            }
            DownloadManager.Request request = new DownloadManager.Request(Uri.Parse(url));
            /*
            ConnectivityManager cm = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            if (cm.ActiveNetworkInfo.Type == ConnectivityType.Mobile)
                request.SetAllowedNetworkTypes(DownloadNetwork.Mobile);
            else
                request.SetAllowedNetworkTypes(DownloadNetwork.Wifi);
                */
            request.SetAllowedNetworkTypes(DownloadNetwork.Wifi | DownloadNetwork.Mobile);
            request.SetAllowedOverMetered(true);
            request.SetAllowedOverRoaming(false);
            request.SetVisibleInDownloadsUi(true);
            request.SetDestinationInExternalFilesDir(this, Environment.DirectoryDownloads, APKname);
            request.AllowScanningByMediaScanner();
            request.SetNotificationVisibility(DownloadVisibility.Visible);

            // 设置 Notification 信息
            request.SetTitle(title);
            //request.SetDescription(Description);
            request.SetMimeType("application/vnd.android.package-archive");
            //如果已经启动过下载
            if (DownloadID != 0)
            {
                DownloadManager.Query query = new DownloadManager.Query();
                query.SetFilterById(DownloadID);
                ICursor cursor = ((DownloadManager)GetSystemService(DownloadService)).InvokeQuery(query);
                if (cursor.MoveToFirst())
                {
                    var status = (DownloadStatus)cursor.GetInt(cursor.GetColumnIndex(DownloadManager.ColumnStatus));
                    if (status != DownloadStatus.Successful || status != DownloadStatus.Failed) return;//下载成功或者失败才能继续这次下载
                }
            }
            // 实例化DownloadManager 对象
            DownloadManager downloadManager = (DownloadManager)GetSystemService(DownloadService);
            DownloadID = downloadManager.Enqueue(request);//开始下载
            Setlistener(DownloadID);//设置下载监听
        }
        /// <summary>
        /// 注册广播监听系统的下载完成事件
        /// </summary>
        /// <param name="Id"></param>
        private void Setlistener(long Id)
        {
            IntentFilter intentFilter = new IntentFilter(DownloadManager.ActionDownloadComplete);
            DownloadReceiver broadcastReceiver = new DownloadReceiver(this, Id);
            RegisterReceiver(broadcastReceiver, intentFilter);
        }
        /// <summary>
        /// 监听下载完成类
        /// </summary>
        class DownloadReceiver : BroadcastReceiver
        {
            long Id;
            UpdataActivity updata;
            public DownloadReceiver(UpdataActivity updata, long Id)
            {
                this.updata = updata;
                this.Id = Id;
            }
            public override void OnReceive(Context context, Intent intent)
            {
                DownloadManager manager = (DownloadManager)context.GetSystemService(Context.DownloadService);
                // 这里是通过下面这个方法获取下载的id，
                long ID = intent.GetLongExtra(DownloadManager.ExtraDownloadId, -1);
                // 这里把传递的id和广播中获取的id进行对比是不是我们下载apk的那个id，如果是的话，就开始获取这个下载的路径
                if (ID == Id)
                {
                    DownloadManager.Query query = new DownloadManager.Query();
                    query.SetFilterById(Id);
                    ICursor cursor = manager.InvokeQuery(query);
                    if (cursor.MoveToFirst())
                    {
                        // 获取文件下载路径
                        string fileName = cursor.GetString(cursor.GetColumnIndex(DownloadManager.ColumnLocalUri));
                        // 如果文件名不为空，说明文件已存在,则进行自动安装apk
                        if (fileName != null)
                        {
                            updata.OpenAPK(fileName);
                        }
                    }
                    cursor.Close();
                }
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
            File file = new File(Uri.Parse(fileSavePath).Path);
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
            File file = new File(Uri.Parse(fileSavePath).Path);
            string filePath = file.AbsolutePath;
            StartInstallintent = new Intent(Intent.ActionView);
            StartInstallintent.SetFlags(ActivityFlags.NewTask);
            var provider = PackageName + ".fileprovider";
            Uri data = FileProvider.GetUriForFile(this, provider, new File(filePath));
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
            File file = new File(Uri.Parse(fileSavePath).Path);
            string filePath = file.AbsolutePath;
            StartInstallintent = new Intent(Intent.ActionView);
            StartInstallintent.SetFlags(ActivityFlags.NewTask);
            var provider = PackageName + ".fileprovider";
            Uri data = FileProvider.GetUriForFile(this, provider, new File(filePath));
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