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
    public class TextAnalysis
    {
        /// <summary>
        /// 分析输入框包含的搜索参数
        /// </summary>
        /// <param name="SearchText"></param>
        static public SearchKey SearchText(string SearchText)
        {
            var searchkey = new SearchKey();
            List<string> Likes = new List<string>();
            #region RoomNum
            Likes.Add("6A-101");
            Likes.Add("6A_101");
            Likes.Add("6A 101");
            Likes.Add("6A101");
            Likes.Add("8-234");
            Likes.Add("101");
            Likes.Add("10");
            Likes.Add("6A");
            Likes.Add("1");
            searchkey.RoomNum = FormateAnalysis(Likes, SearchText);
            #endregion

            #region TeacherName
            Likes.Clear();
            Likes.Add("王剩剩剩");
            Likes.Add("王剩剩");
            Likes.Add("王剩");
            Likes.Add("王");
            searchkey.TeacherName = FormateAnalysis(Likes, SearchText);
            #endregion
            #region CourseTime

            //searchkey.CourseTime =
            #endregion
            #region CourseNum
            Likes.Clear();
            Likes.Add("12345678");
            Likes.Add("1234567");
            Likes.Add("123456");
            searchkey.CourseNum = FormateAnalysis(Likes, SearchText);
            #endregion
            #region CourseName
            Likes.Clear();
            Likes.Add("电");
            Likes.Add("电机");
            Likes.Add("电机学");
            Likes.Add("电机学学");
            Likes.Add("电机学学学");
            Likes.Add("电机学学学学");
            Likes.Add("马克思主义原理");
            Likes.Add("MA");
            Likes.Add("MAT");
            Likes.Add("MATL");
            Likes.Add("MATLA");
            Likes.Add("MATLAB");
            Likes.Add("MATLABB");
            Likes.Add("MATLABBB");
            searchkey.CourseName = FormateAnalysis(Likes, SearchText);
            #endregion
            return searchkey;
        }
        public static string GetCharType(char Value)
        {
            if (char.IsNumber(Value)) return "数字";
            if (char.IsPunctuation(Value)) return "标点";
            if (Value >= 0x4e00 && Value <= 0x9fbb) return "汉字";
            if (char.IsLetter(Value)) return "字母";
            return "";
        }

        private static string FormateAnalysis(List<string> Likes, string Text)
        {
            string Resoult = "";
            foreach (string Like in Likes)//遍历需要匹配的字符串
            {
                //识别一个字符串
                var LikeType = GetCharType(Like[0]);//获取第一个字符的类型
                int Position = 0;//text位置
                foreach (char TextSymbol in Text)//遍历text的每一个字符
                {
                    if (LikeType == GetCharType(TextSymbol)) //匹配到了第一个 开始识别
                    {
                        for (int TextP = Position, LikeP = 0; TextP < Text.Length && LikeP < Like.Length; TextP++, LikeP++)
                        {
                            if (GetCharType(Text[TextP]) != GetCharType(Like[LikeP])) break;
                            if (LikeP == Like.Length - 1)
                            {
                                if (TextP >= Text.Length - 1 || GetCharType(Text[TextP + 1]) != GetCharType(Like[LikeP]))
                                    Resoult = Text.Substring(Position, Like.Length);
                                else break;
                                return Resoult;
                            }
                        }
                    }
                    Position++;
                }
            }
            return Resoult;
        }
    }
}