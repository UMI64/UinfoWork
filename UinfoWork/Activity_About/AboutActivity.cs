using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views; 
using Android.Widget;
using Android.Content;
namespace Uinfo.About
{
    [Activity(Label = "About")]
    public class AboutActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.About);
            #region 设置ToolBar
            SetToolBar();
            #endregion
            #region 获取更新信息
            string RawVersionInfo=httpRequest.GetGerResult("https://raw.githubusercontent.com/UMI64/UinfoWork/master/UinfoWork/Properties/AndroidManifest.xml");
            string RawVersionDiscriptionInfo = httpRequest.GetGerResult(" https://raw.githubusercontent.com/UMI64/UinfoWork/master/UinfoWork/Resources/values/Strings.xml");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(RawVersionInfo);
            XmlNode rootNode = xmlDoc.SelectSingleNode("manifest");
            string VersionCode=rootNode.Attributes["android:versionCode"].Value;
            string VersionName= rootNode.Attributes["android:versionName"].Value;
            xmlDoc.LoadXml(RawVersionDiscriptionInfo);
            rootNode = xmlDoc.SelectSingleNode("resources");
            string VersionDiscription = string.Empty;
            foreach (XmlNode node in rootNode)
            {
                try
                {
                    if (node.Attributes["name"].Value == "Discription")
                    {
                        VersionDiscription = node.InnerText;
                    }
                }
                catch
                {
                }
            }
            #endregion
        }

        private void SetToolBar()
        {
            SupportActionBar.Subtitle = "关于";
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
        }
        //Toolbar的事件---返回
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