using System.Xml;

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
    }
}