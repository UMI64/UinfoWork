using System.Xml;
using Android.Content;
using Android.Content.PM;
namespace Uinfo.Updata
{
    public class Verison
    {
        public string VersionCode;
        public string VersionName;
        public string VersionDiscription;
        public static Verison GetLatestVerison()
        {
            Verison verison = new Verison();
            try
            {
                string RawVersionInfo = httpRequest.GetGerResult("https://raw.githubusercontent.com/UMI64/UinfoWork/master/UinfoWork/Properties/AndroidManifest.xml");
                string RawVersionDiscriptionInfo = httpRequest.GetGerResult(" https://raw.githubusercontent.com/UMI64/UinfoWork/master/UinfoWork/Resources/values/Strings.xml");
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(RawVersionInfo);
                XmlNode rootNode = xmlDoc.SelectSingleNode("manifest");
                verison.VersionCode = rootNode.Attributes["android:versionCode"].Value;
                verison.VersionName = rootNode.Attributes["android:versionName"].Value;
                xmlDoc.LoadXml(RawVersionDiscriptionInfo);
                rootNode = xmlDoc.SelectSingleNode("resources");
                verison.VersionDiscription = string.Empty;
                foreach (XmlNode node in rootNode)
                {
                    if (node.Attributes["name"].Value == "Discription")
                    {
                        verison.VersionDiscription = node.InnerText;
                    }
                }
            }
            catch
            {

            }
            return verison;
        }
        public static Verison GetAPKVersion(string absPath, Context context)
        {
            Verison version = new Verison();
            PackageManager pm = context.PackageManager;
            PackageInfo pkgInfo = pm.GetPackageArchiveInfo(absPath, PackageInfoFlags.Activities);
            if (pkgInfo != null)
            {
                ApplicationInfo appInfo = pkgInfo.ApplicationInfo;
                /* 必须加这两句，不然下面icon获取是default icon而不是应用包的icon */
                appInfo.SourceDir = absPath;
                appInfo.PublicSourceDir = absPath;
                version.VersionCode = pkgInfo.VersionCode.ToString(); // 得到版本信息  
                version.VersionName = pkgInfo.VersionName;
            }
            else
            {
                version.VersionCode = "0";
                version.VersionName = "0";
            }
            version.VersionDiscription = string.Empty;
            return version;
        }
        public static Verison GetLocalVersion(Context context)
        {
            Verison version = new Verison();
            // 获取packagemanager的实例
            PackageManager packageManager = context.PackageManager;
            // getPackageName()是你当前类的包名
            PackageInfo packInfo = packageManager.GetPackageInfo(context.PackageName, 0);
            version.VersionCode = packInfo.VersionCode.ToString();
            version.VersionName = packInfo.VersionName;
            version.VersionDiscription = context.GetString(Resource.String.Discription);
            return version;
        }
        public static bool operator <(Verison v1, Verison v2)
        {
            int.TryParse(v1.VersionCode.ToString(), out int V1);
            int.TryParse(v2.VersionCode.ToString(), out int V2);
            return V1 < V2;
        }
        public static bool operator >(Verison v1, Verison v2)
        {
            int.TryParse(v1.VersionCode.ToString(), out int V1);
            int.TryParse(v2.VersionCode.ToString(), out int V2);
            return V1 > V2;
        }
    }
}