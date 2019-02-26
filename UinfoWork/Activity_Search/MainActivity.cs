using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Support.V4.Widget;
using Android.Views.InputMethods;
using Android.Content;
using Android.Support.Design.Widget;
using Uinfo.Search;
using Uinfo.About;
using Uinfo.Activity_TuCao;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Android.Graphics;

namespace Uinfo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class MainActivity : AppCompatActivity
    {
        public Condition conditions;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainSearch);
            #region 界面下方的教室信息链表
            SearchRoom searchRoom = new SearchRoom(this);//创建一个搜索类
            RecyclerView RoomRecyclertView = FindViewById<RecyclerView>(Resource.Id.RoomRecyclertView);//绑定链表
            RecyclerView.Adapter roomlist_adapter = new RecyclerViewAdapter(searchRoom.Show_Datas, this);//创建适配器
            LoadMoreWrapper loadMoreWrapper = new LoadMoreWrapper(roomlist_adapter);//创建含有加载更多的适配器
            RoomRecyclertView.SetLayoutManager(new LinearLayoutManager(this));//
            RoomRecyclertView.SetAdapter(loadMoreWrapper);//设置链表的适配器
            RoomRecyclertView.AddOnScrollListener(new Loadmore(this, searchRoom, loadMoreWrapper));//设置滑动到底部时加载更多
            Detail DetailView = new Detail(this);//教室详细信息页面
            ((RecyclerViewAdapter)roomlist_adapter).OnClickEventHandler += (RoomNum) =>
            {//点击教室后展开详细页面 传入教室相关信息
                foreach (var room in searchRoom.RoomList)
                {
                    if (room.RoomNum.Equals(RoomNum))
                    {
                        DetailView.ShowDetail(room);
                        break;
                    }
                }
            };
            #endregion
            #region 左菜单
            /*左菜单的toolbar呼出按钮*/
            DrawerLayout drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerLayout);
            Button CalLeftMenu_button = FindViewById<Button>(Resource.Id.CalLeftMenu_button);
            CalLeftMenu_button.Click += (o, e) =>
            {
                drawerLayout.OpenDrawer((int)GravityFlags.Start);
            };
            //关于
            ImageView AboutimageView=FindViewById<ImageView>(Resource.Id.AboutimageView);
            TextView AboutText = FindViewById<TextView>(Resource.Id.AboutText);
            AboutimageView.Click += AboutimageView_Click;
            AboutText.Click += AboutimageView_Click;
            void AboutimageView_Click(object sender, EventArgs e)
            {
                Intent intent = new Intent(this, typeof(AboutActivity));
                //启动关于界面
                StartActivity(intent);
            }
            //吐槽
            ImageView TocaoimageView = FindViewById<ImageView>(Resource.Id.TocaoimageView);
            TextView TocaoText = FindViewById<TextView>(Resource.Id.TocaoText);
            TocaoimageView.Click += TocaoimageView_Click;
            TocaoText.Click += TocaoimageView_Click;
            void TocaoimageView_Click(object sender, EventArgs e)
            {
                Intent intent = new Intent(this, typeof(TuCaoFakeActivity));
                //启动假吐槽界面
                StartActivity(intent);
            }
            TocaoimageView.LongClick += TocaoimageView_LongClick;
            TocaoText.LongClick += TocaoimageView_LongClick;
            void TocaoimageView_LongClick(object sender, View.LongClickEventArgs e)
            {
                Intent intent = new Intent(this, typeof(TuCaoActivity));
                //启动吐槽界面
                StartActivity(intent);
            }
            /*左菜单的版本号*/
            TextView VersionCode = FindViewById<TextView>(Resource.Id.VersionCodeText);
            VersionCode.Text = "版本 " + APKVersionCodeUtils.GetVerName(this);
            #endregion
            #region 搜索条件
            conditions = new Condition(this, searchRoom, loadMoreWrapper);
            #endregion
            #region 搜索栏
            EditText SearchBox = FindViewById<EditText>(Resource.Id.edit_search);
            Button SearchButton = FindViewById<Button>(Resource.Id.SearchButton);
            /*搜索提示*/
            string Tips = GetString(Resource.String.Tip);
            JObject jObject = (JObject)JsonConvert.DeserializeObject(Tips);
            JArray jArray = (JArray)jObject["Tips"];
            Random rd = new Random();
            SearchBox.Hint = jArray[rd.Next(0, jArray.Count)].ToString();
            /*开始搜索教室*/
            /*第一次启动搜索一次防止说空*/
            loadMoreWrapper.LoadState = LoadMoreWrapper.LOADING;
            new LoadTask(this, searchRoom, loadMoreWrapper, conditions).Execute("101");
            SearchBox.EditorAction += (sender, args) =>
            {
                if (args.ActionId == ImeAction.Search)
                {
                    if (RoomRecyclertView.GetAdapter().ItemCount > 3)
                        RoomRecyclertView.ScrollToPosition(0);
                    searchRoom.Show_Datas.Clear();//清空显示数据
                    loadMoreWrapper.LoadState = LoadMoreWrapper.LOADING;
                    new LoadTask(this, searchRoom, loadMoreWrapper, conditions).Execute(SearchBox.Text);
                }
            };
            SearchButton.Click += (sender, args) =>
            {
                if (RoomRecyclertView.GetAdapter().ItemCount > 3)
                    RoomRecyclertView.ScrollToPosition(0);
                searchRoom.Show_Datas.Clear();//清空显示数据
                loadMoreWrapper.LoadState = LoadMoreWrapper.LOADING;
                new LoadTask(this, searchRoom, loadMoreWrapper, conditions).Execute(SearchBox.Text);
            };
            /*左右按钮换颜色*/
            AppBarLayout layout = FindViewById<AppBarLayout>(Resource.Id.appBarLayout1);
            layout.OffsetChanged += (object sender, AppBarLayout.OffsetChangedEventArgs e) =>
            {
                int gray = 255;
                if (-e.VerticalOffset+162 < 255)
                    gray = -e.VerticalOffset+162;
                Color filtercolor = Color.Rgb(255, 242, gray);
                CalLeftMenu_button.Background.SetColorFilter(filtercolor, PorterDuff.Mode.Multiply);
                SearchButton.Background.SetColorFilter(filtercolor, PorterDuff.Mode.Multiply);
            };
            #endregion
        }

        /// <summary>
        /// 加载更多逻辑
        /// </summary>
        private class Loadmore : EndlessRecyclerOnScrollListener
        {
            SearchRoom searchRoom;
            LoadMoreWrapper loadMoreWrapper;
            Context context;
            public Loadmore(Context context, SearchRoom searchRoom, LoadMoreWrapper loadMoreWrapper)
            {
                this.context = context;
                this.searchRoom = searchRoom;
                this.loadMoreWrapper = loadMoreWrapper;
            }
            public override void onLoadMore()
            {
                if (loadMoreWrapper.LoadState != LoadMoreWrapper.LOADING_END)
                {
                    loadMoreWrapper.LoadState = LoadMoreWrapper.LOADING;
                    new LoadmoreTask(context, searchRoom, loadMoreWrapper).Execute(5);
                }
            }
        }
        /// <summary>
        /// 初次加载任务
        /// </summary>
        public class LoadTask : AsyncTask<string, int, string>
        {
            SearchRoom searchRoom;
            LoadMoreWrapper loadMoreWrapper;
            Condition condition;
            Context context;
            public LoadTask(Context context, SearchRoom searchRoom, LoadMoreWrapper loadMoreWrapper, Condition condition)
            {
                this.context = context;
                this.searchRoom = searchRoom;
                this.loadMoreWrapper = loadMoreWrapper;
                this.condition = condition;
            }
            protected override string RunInBackground(string[] @params)
            {
                System.Threading.Thread.Sleep(200);
                return searchRoom.Start(@params[0], condition);
            }
            protected override void OnPostExecute(string result)
            {
                base.OnPostExecute(result);
                if (result.Contains("End"))
                {
                    loadMoreWrapper.LoadState = LoadMoreWrapper.LOADING_END;
                }
                else
                {
                    loadMoreWrapper.LoadState = LoadMoreWrapper.LOADING_COMPLETE;
                    Toast toast = Toast.MakeText(context, result, ToastLength.Long);
                    toast.Show();
                }

            }
        }
        /// <summary>
        /// 加载更多任务
        /// </summary>
        public class LoadmoreTask : AsyncTask<int, int, string>
        {
            SearchRoom searchRoom;
            LoadMoreWrapper loadMoreWrapper;
            Context context;
            public LoadmoreTask(Context context, SearchRoom searchRoom, LoadMoreWrapper loadMoreWrapper)
            {
                this.context = context;
                this.searchRoom = searchRoom;
                this.loadMoreWrapper = loadMoreWrapper;
            }
            protected override string RunInBackground(int[] @params)
            {
                System.Threading.Thread.Sleep(800);
                return searchRoom.GetinfoFromRoomNumber(@params[0]);
            }
            protected override void OnPostExecute(string result)
            {
                base.OnPostExecute(result);
                if (result.Contains("End"))
                {
                    loadMoreWrapper.LoadState = LoadMoreWrapper.LOADING_END;
                }
                else
                {
                    loadMoreWrapper.LoadState = LoadMoreWrapper.LOADING_COMPLETE;
                    Toast toast = Toast.MakeText(context, result, ToastLength.Long);
                    toast.Show();
                }
            }
        }
    }
}
