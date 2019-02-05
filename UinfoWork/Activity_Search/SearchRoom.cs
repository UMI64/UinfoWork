using System.Text.RegularExpressions;
using Android.OS;
using Android.Views;
using Android.Text.Format;
using Android.Content;
using Java.Text;
using Java.Util;
using Android.Widget;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Uinfo
{
    public class SearchRoom
    {
        private Context context;
        Condition condition;
        private string RoomDetil = "http://106.15.191.168/api/RoomDetail";
        private string RoomURL = "http://106.15.191.168/api/RoomNum";
        public List<Show_Roomdate> Show_Datas = new List<Show_Roomdate>();//用于显示的数据
        public List<RoomData> RoomList = new List<RoomData>();//当前的教室列表
        public SearchRoom(Context context)
        {
            this.context = context;
        }
        /// <summary>
        /// 开始搜索，并展现10个教室的信息
        /// </summary>
        /// <param name="SearchText"></param>
        /// <param name="Condition"></param>
        public string Start(string SearchText, Condition Condition)
        {
            var res=GetRoomNumber(SearchText, Condition);
            if (res.Contains("Success")) return GetinfoFromRoomNumber(10);
            else return res;
        }
        /// <summary>
        /// 搜索教室号
        /// </summary>
        /// <param name="SearchText"></param>
        /// <param name="Condition"></param>
        public string GetRoomNumber(string SearchText, Condition Condition)
        {
            condition = Condition;
            //分析文本参数
            var searchkey = TextAnalysis.SearchText(SearchText);
            searchkey.CourseTime = condition.Time.ToString("g");
            #region HTTP查询
            //添加Post 参数
            var Key = new Dictionary<string, string>();
            Key.Add("RoomNum", searchkey.RoomNum);
            Key.Add("CourseName", searchkey.CourseName);
            Key.Add("TeacherName", searchkey.TeacherName);
            Key.Add("CourseTime", searchkey.CourseTime);
            Key.Add("CourseNum", searchkey.CourseNum);
            //获取结果
            var Result = httpRequest.PostGerResult(RoomURL, Key);
            #region 网络错误处理
            if (!Result.Contains("rooms_num"))
                return "远程服务器未响应";
            #endregion
            #endregion
            #region 清空原始数据
            RoomList.Clear();//清空教室号数据
            Show_Datas.Clear();//清空显示数据
            #endregion
            #region 处理Json数据并写入房间号链表
            JObject jObject = (JObject)JsonConvert.DeserializeObject(Result);
            JArray jArray = (JArray)jObject["rooms_num"];
            foreach (var roomnum in jArray)
            {
                RoomData room = new RoomData();
                room.RoomNum = roomnum.ToString();
                RoomList.Add(room);
            }
            #endregion
            return "Success";
        }
        /// <summary>
        /// 根据已有教室号下载教室的详细信息
        /// </summary>
        /// <param name="NumberOFrooms"></param>
        /// <returns></returns>
        public string GetinfoFromRoomNumber(int NumberOFrooms)
        {
            bool HaveEmptyInfoRoom = true;
            var TagRooms = new List<string>();//目标房间
            int count = 0;
            foreach (var room in RoomList)
            {
                if (room.InfoEmpty)
                {
                    TagRooms.Add(room.RoomNum);
                    if(++count>9) break;
                }
            }
            if (count < 10) HaveEmptyInfoRoom = false;
            GetRoomInfo(TagRooms);
            return HaveEmptyInfoRoom?"Success":"End";
        }
        /// <summary>
        /// 下载给定房间包含的课程并添加到显示数据链表
        /// </summary>
        /// <param name="SearchText">一串房间号码"6A-234/8-777"</param>
        private void GetRoomInfo(List<string> RoomNumList)
        {
            #region HTTP下载
            //添加Post 参数
            var Key = new Dictionary<string, List<string>>();
            Key.Add("RoomNum", RoomNumList);
            //获取结果
            var Result = httpRequest.PostGerResult(RoomDetil, Key);
            #region 网络错误处理
            if (!Result.Contains("rooms"))
            {
                Toast toast = Toast.MakeText(context, "远程服务器未响应", ToastLength.Short);
                toast.Show();
                return;
            }
            #endregion
            //处理Json数据
            var roomdates = Serialize(Result);//临时房间课程信息链表
            #endregion
            #region 把找到的数据存进本地缓存
            foreach (var Troom in roomdates)
            {
                for (int count = 0; count < RoomList.Count; count++)
                {
                    if (RoomList[count].RoomNum == Troom.RoomNum)
                    {
                        Troom.InfoEmpty = false;
                        RoomList[count] = Troom;
                        break;
                    }
                }
                var ShowData = Troom.GetShowData(condition.Time);//给房间设定时间
                if (!condition.EmptyRoomFlag || ShowData.CourseName == "空教室")
                    Show_Datas.Add(ShowData);//加入显示链表
            }
            #endregion
        }
        /// <summary>
        /// JSON房间信息处理
        /// </summary>
        static private List<RoomData> Serialize(string JsonRoomsData)
        {
            /*反序列化出ROOMS的数组*/
            JObject jObject = (JObject)JsonConvert.DeserializeObject(JsonRoomsData);
            JArray jArray = (JArray)jObject["rooms"];

            List<RoomData> rooms = new List<RoomData>();
            foreach (var item in jArray)
            {
                /*反序列化出course课程*/
                JObject jOcourse = (JObject)JsonConvert.DeserializeObject(item.ToString());
                JArray jAcourse = (JArray)jOcourse["courses"];

                List<Coursedata> courselist = new List<Coursedata>();
                foreach (var Courseitem in jAcourse)
                {
                    Coursedata course = new Coursedata
                    {
                        TeacherName = Courseitem["courseTeacher"].ToString(),
                        CourseName = Courseitem["courseName"].ToString(),
                        CourseTime = Courseitem["courseTime"].ToString(),
                        CourseNum = Courseitem["courseNum"].ToString(),
                        CourseChange = Courseitem["courseChange"].ToString()

                    };
                    courselist.Add(course);
                }
                RoomData room = new RoomData
                {
                    RoomNum = (((JObject)item)["number"]).ToString(),
                    Coursedates = courselist
                };
                rooms.Add(room);
            }
            return rooms;
        }
        /// <summary>
        /// 清空显示信息
        /// </summary>
        public void CleanShowDatas()
        {
            Show_Datas.Clear();
        }
        /// <summary>
        /// 条件变动后刷新显示的数据
        /// </summary>
        public void ResetShowDatas(Condition condition)
        {
            Show_Datas.Clear();
            foreach (var room in RoomList)
            {
                if (room.InfoEmpty) continue;
                Show_Roomdate ShowData = room.GetShowData(condition.Time);
                if (!condition.EmptyRoomFlag || ShowData.CourseName == "空教室")
                    Show_Datas.Add(ShowData);
            }
        }
    }
}