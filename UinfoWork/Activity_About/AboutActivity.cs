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
using Uinfo.Updata;
namespace Uinfo.About
{
    [Activity(Label = "About")]
    public class AboutActivity : AppCompatActivity
    {
        List<AboutItem> AboutMenu = new List<AboutItem>();
        RecyclerView.Adapter Aboutlist_adapter;
        Verison NewVerison=new Verison();
        Verison LocalVerison;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.About);
            #region 设置ToolBar
            SetToolBar();
            #endregion
            #region 获取更新信息
            new LoadVerisonTask(this).Execute(5);
            #endregion
            LocalVerison = Verison.GetLocalVersion(this);
            AboutMenu.Add(new AboutItem("版本", "版本名 " + LocalVerison.VersionName + "\r\n" + "版本号 " + LocalVerison.VersionCode));
            AboutMenu.Add(new AboutItem("分割",""));
            AboutMenu.Add(new AboutItem("团队成员", ""));
            RecyclerView AboutRecyclertView = FindViewById<RecyclerView>(Resource.Id.AboutRecyclertView);//绑定链表
            Aboutlist_adapter = new AboutRecyclerViewAdapter(AboutMenu, this);//创建适配器
            AboutRecyclertView.SetLayoutManager(new LinearLayoutManager(this));//
            AboutRecyclertView.SetAdapter(Aboutlist_adapter);//设置链表的适配器
            //点击选项后的操作
            ((AboutRecyclerViewAdapter)Aboutlist_adapter).OnClickEventHandler += (Title) =>
            {
                if (Title == "版本" && AboutMenu[0].RedPointVisibility)
                {
                    Intent intent = new Intent(this, typeof(UpdataActivity));
                    //启动
                    intent.PutExtra("VersionCode", NewVerison.VersionCode.ToString());
                    intent.PutExtra("VersionName", NewVerison.VersionName.ToString());
                    intent.PutExtra("VersionDiscription", NewVerison.VersionDiscription.ToString());
                    StartActivity(intent);
                }
            };
        }
        private void SetToolBar()
        {
            SupportActionBar.Title = "关于";
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }
        /// <summary>
        /// Toolbar的事件---返回
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
        /// 获取最新APK任务
        /// </summary>
        public class LoadVerisonTask : AsyncTask<string, int, Verison>
        {
            AboutActivity About;
            public LoadVerisonTask(AboutActivity About)
            {
                this.About = About;
            }
            protected override Verison RunInBackground(string[] @params)
            {
                return Verison.GetLatestVerison();
            }
            protected override void OnPostExecute(Verison result)
            {
                base.OnPostExecute(result);
                About.NewVerison.VersionCode = result.VersionCode;
                About.NewVerison.VersionName = result.VersionName;
                About.NewVerison.VersionDiscription = result.VersionDiscription;
                About.AboutMenu[0].RedPointVisibility = result > Verison.GetLocalVersion(About);
                About.Aboutlist_adapter.NotifyDataSetChanged();
            }
        }
    }
    public class AboutItem
    {
        public string Title = string.Empty;
        public string Text = string.Empty;
        public bool RedPointVisibility = false;
        public AboutItem(string Title, string Text)
        {
            this.Title = Title;
            this.Text = Text;
        }
    }
    public class AboutRecyclerViewAdapter : RecyclerView.Adapter, View.IOnClickListener
    {
        public delegate void OnClickEvent(string Title);
        public event OnClickEvent OnClickEventHandler;
        private List<AboutItem> data;
        private Context _context;
        public AboutRecyclerViewAdapter(List<AboutItem> list, Context context)
        {
            data = list;
            _context = context;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder.ItemViewType==0)
            {
                MyViewHolder myViewHolder = holder as MyViewHolder;
                myViewHolder.AboutTitle.Text = data[position].Title;
                myViewHolder.AboutText.Text = data[position].Text;
                myViewHolder.RedPoint.Visibility = data[position].RedPointVisibility ? ViewStates.Visible : ViewStates.Invisible;
                myViewHolder.ItemView.SetOnClickListener(this);
            }
        }
        public void OnClick(View v)
        {
            OnClickEventHandler(v.FindViewById<TextView>(Resource.Id.AboutTitle).Text);
        }
        public override int GetItemViewType(int position)
        {
            if (data[position].Title == "分割")
                return 1;
            else
                return 0;
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewItem)
        {
            View view;
            if (viewItem == 0)
            {
                view = LayoutInflater.From(_context).Inflate(Resource.Layout.AboutListItem, parent, false);
            }
            else
            { view = LayoutInflater.From(_context).Inflate(Resource.Layout.AboutListItemDiv, parent, false);

            }
            MyViewHolder holder = new MyViewHolder(view, viewItem);
            return holder;
        }
        public override int ItemCount
        {
            get
            {
                return data.Count;
            }
        }
        public override void OnViewRecycled(Java.Lang.Object holder)
        {
            base.OnViewRecycled(holder);
            MyViewHolder myViewHolder = holder as MyViewHolder;
        }
    }
    public class MyViewHolder : RecyclerView.ViewHolder
    {
        public int Type = 0;
        public TextView AboutTitle;
        public TextView AboutText;
        public ImageView RedPoint;
        public MyViewHolder(View itemView,int type) : base(itemView)
        {
            Type = type;
            AboutTitle = itemView.FindViewById<TextView>(Resource.Id.AboutTitle);
            AboutText = itemView.FindViewById<TextView>(Resource.Id.AboutText);
            RedPoint = itemView.FindViewById<ImageView>(Resource.Id.RedPoint);
        }
    }
}