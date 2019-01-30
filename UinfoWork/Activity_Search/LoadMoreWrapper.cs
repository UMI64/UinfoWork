using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace Uinfo.Search
{
    public class LoadMoreWrapper : RecyclerView.Adapter
    {

        private RecyclerView.Adapter adapter;

        // 普通布局
        private const int TYPE_ITEM = 1;
        // 脚布局
        private const int TYPE_FOOTER = 2;
        // 当前加载状态，默认为加载完成
        private int loadState = 2;
        // 正在加载
        public const int LOADING = 1;
        // 加载完成
        public const int LOADING_COMPLETE = 2;
        // 加载到底
        public const int LOADING_END = 3;

        public LoadMoreWrapper(RecyclerView.Adapter adapter)
        {
            this.adapter = adapter;
        }
        public override int GetItemViewType(int position)
        {
            // 最后一个item设置为FooterView
            if (position + 1 == ItemCount)
            {
                return TYPE_FOOTER;
            }
            else
            {
                return TYPE_ITEM;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //进行判断显示类型，来创建返回不同的View
            if (viewType == TYPE_FOOTER)
            {
                View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.RefreshFoot, parent, false);
                return new FootViewHolder(view);
            }
            else
            {
                return adapter.OnCreateViewHolder(parent, viewType);
            }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is FootViewHolder)
            {
                FootViewHolder footViewHolder = (FootViewHolder)holder;
                switch (LoadState)
                {
                    case LOADING: // 正在加载
                        footViewHolder.pbLoading.Visibility = ViewStates.Visible; 
                        footViewHolder.tvLoading.Visibility = ViewStates.Visible;
                        footViewHolder.llEnd.Visibility = ViewStates.Gone;
                        break;

                    case LOADING_COMPLETE: // 加载完成
                        footViewHolder.pbLoading.Visibility = ViewStates.Invisible;
                        footViewHolder.tvLoading.Visibility = ViewStates.Invisible;
                        footViewHolder.llEnd.Visibility = ViewStates.Gone;
                        break;

                    case LOADING_END: // 加载到底
                        footViewHolder.pbLoading.Visibility = ViewStates.Gone;
                        footViewHolder.tvLoading.Visibility = ViewStates.Gone;
                        footViewHolder.llEnd.Visibility = ViewStates.Visible;
                        break;

                    default:
                        break;
                }
            }
            else
            {
                adapter.OnBindViewHolder(holder, position);
            }
        }
        public override int ItemCount
        {
            get
            {
                return adapter.ItemCount + 1;
            }
        }
        public override void OnAttachedToRecyclerView(RecyclerView recyclerView)
        {
            base.OnAttachedToRecyclerView(recyclerView);
            RecyclerView.LayoutManager manager = recyclerView.GetLayoutManager();
            if (manager is GridLayoutManager)
            {
                GridLayoutManager GridManager = ((GridLayoutManager)manager);
                GridManager.SetSpanSizeLookup(new MySpanSizeLookup(GridManager,this));
            }
        }
        public class MySpanSizeLookup :GridLayoutManager.SpanSizeLookup
        {
            GridLayoutManager GridManager;
            LoadMoreWrapper loadMoreWrapper;
            public MySpanSizeLookup(GridLayoutManager GridManager, LoadMoreWrapper loadMoreWrapper)
            {
                this.loadMoreWrapper = loadMoreWrapper;
                this.GridManager = GridManager;
            }
            public override int GetSpanSize(int position)
            {
                // 如果当前是footer的位置，那么该item占据2个单元格，正常情况下占据1个单元格
                return loadMoreWrapper.GetItemViewType(position) == TYPE_FOOTER ? GridManager.SpanCount : 1;
            }
        }
        public class FootViewHolder : RecyclerView.ViewHolder
        {

            public ProgressBar pbLoading;
            public TextView tvLoading;
            public RelativeLayout llEnd;

            public FootViewHolder(View itemView) : base(itemView)
            {
                pbLoading = (ProgressBar)itemView.FindViewById(Resource.Id.pb_loading);
                tvLoading = (TextView)itemView.FindViewById(Resource.Id.tv_loading);
                llEnd = (RelativeLayout)itemView.FindViewById(Resource.Id.ll_end);
            }
        }

        /**
         * 设置上拉加载状态
         *
         * @param loadState 0.正在加载 1.加载完成 2.加载到底
         */
        public int LoadState
        {
            get
            {
                return loadState;
            }
            set
            {
                loadState = value;
                NotifyDataSetChanged();
            }
        }
    }
}