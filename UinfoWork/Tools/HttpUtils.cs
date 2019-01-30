using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Uinfo
{
    public class httpRequest
    {
        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name ="url">请求后台地址</param>
        /// <param name ="dic">参数</param>
        public static string PostGerResult(string url, Dictionary<string, List<string>> pairs)
        {
            try
            {
                string result = "";
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/json";
                #region 添加Post 参数
                string jsonParam = JsonConvert.SerializeObject(pairs);
                var byteData = Encoding.UTF8.GetBytes(jsonParam);
                req.ContentLength = byteData.Length;
                var writer = req.GetRequestStream();
                writer.Write(byteData, 0, byteData.Length);
                writer.Close();
                #endregion
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }
        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name ="url">请求后台地址</param>
        /// <param name ="dic">参数</param>
        public static string PostGerResult(string url, Dictionary<string, string> pairs)
        {
            try
            {
                string result = "";
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/json";
                #region 添加Post 参数
                string jsonParam = JsonConvert.SerializeObject(pairs);
                var byteData = Encoding.UTF8.GetBytes(jsonParam);
                req.ContentLength = byteData.Length;
                var writer = req.GetRequestStream();
                writer.Write(byteData, 0, byteData.Length);
                writer.Close();
                #endregion
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }
        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <returns></returns>
        public static string PostGerResult(string url)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
    }
}