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
    public class APKVersionCodeUtils
    {

        /// <summary>
        /// 获取当前本地apk的版本
        /// </summary>
        /// <param name="mContext">上下文</param>
        /// <returns></returns>
        public static int GetVersionCode(Context context)
        {
            int versionCode = 0;
            try
            {
                //获取软件版本号，对应AndroidManifest.xml下android:versionCode
                versionCode = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode;
            }
            catch (Exception e)
            {
                Toast toast = Toast.MakeText(context, e.Message, ToastLength.Short);
                toast.Show();
            }
            return versionCode;
        }

        /// <summary>
        /// 获取版本号名称
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public static String GetVerName(Context context)
        {
            String verName = "";
            try
            {
                verName = context.PackageManager.
                        GetPackageInfo(context.PackageName, 0).VersionName;
            }
            catch (Exception e)
            {
                Toast toast = Toast.MakeText(context, e.Message, ToastLength.Short);
                toast.Show();
            }
            return verName;
        }
    }
}