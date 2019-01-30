using System.Collections.Generic;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Uinfo
{
    public class RecyclerViewAdapter : RecyclerView.Adapter, View.IOnClickListener
    {
        public delegate void OnClickEvent(string RoomNum);
        public event OnClickEvent OnClickEventHandler;
        private List<Show_Roomdate> data;
        private Context _context;
        public RecyclerViewAdapter(List<Show_Roomdate> list, Context context)
        {
            data = list;
            _context = context;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is MyViewHolder)
            {
                MyViewHolder myViewHolder = holder as MyViewHolder;
                myViewHolder.RoomNum.Text = data[position].RoomNum;
                myViewHolder.CourseName.Text = data[position].CourseName;
                myViewHolder.TeacherName.Text = data[position].TeacherName;
                myViewHolder.CourseTime.Text = data[position].CourseTime;
                myViewHolder.CourseNum.Text = data[position].CourseNum;
                myViewHolder.ItemView.SetOnClickListener(this);
            }
        }
        public void OnClick(View v)
        {
            OnClickEventHandler(v.FindViewById<TextView>(Resource.Id.RoomNum).Text);
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewItem)
        {
            View view = LayoutInflater.From(_context).Inflate(Resource.Layout.RoomListItem, parent, false);
            MyViewHolder holder = new MyViewHolder(view);
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
        public TextView RoomNum;
        public TextView CourseName;
        public TextView TeacherName;
        public TextView CourseTime;
        public TextView CourseNum;
        public MyViewHolder(View itemView) : base(itemView)
        {
            RoomNum = itemView.FindViewById<TextView>(Resource.Id.RoomNum);
            CourseName = itemView.FindViewById<TextView>(Resource.Id.CourseName);
            TeacherName = itemView.FindViewById<TextView>(Resource.Id.TeacherName);
            CourseTime = itemView.FindViewById<TextView>(Resource.Id.CourseTime);
            CourseNum = itemView.FindViewById<TextView>(Resource.Id.CourseNum);
        }
    }
    public abstract class EndlessRecyclerOnScrollListener : RecyclerView.OnScrollListener
    {
        /*
        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);
            LinearLayoutManager manager = (LinearLayoutManager)recyclerView.GetLayoutManager();
            //获取最后一个完全显示的itemPosition
            int lastItemPosition = manager.FindLastCompletelyVisibleItemPosition();
            int itemCount = manager.ItemCount;

            // 判断是否滑动到了最后一个item
            if (lastItemPosition == (itemCount - 1))
            {
                //加载更多
                onLoadMore();
            }

        }
        */
        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);
            LinearLayoutManager manager = (LinearLayoutManager)recyclerView.GetLayoutManager();
            //获取最后一个完全显示的itemPosition
            int lastItemPosition = manager.FindLastCompletelyVisibleItemPosition();
            int itemCount = manager.ItemCount;

            // 判断是否滑动到了最后一个item
            if (lastItemPosition == (itemCount - 3))
            {
                //加载更多
                onLoadMore();
            }
        }

        /**
         * 加载更多回调
         */
        public abstract void onLoadMore();
    }
}