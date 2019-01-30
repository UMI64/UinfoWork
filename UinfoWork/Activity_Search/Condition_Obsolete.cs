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
using Android.Util;
namespace Uinfo
{
    public class Condition
    {
        public bool EmptyRoomOnly = false;
        public bool NowTime = true;
        public DateTime Time
        {
            get
            {
                DateTime date;
                try
                {
                    date = new DateTime(int.Parse(year.Value), int.Parse(month.Value), int.Parse(day.Value), int.Parse(hour.Value), int.Parse(minutes.Value), 20);
                }
                catch
                {
                    date = new DateTime(int.Parse(year.Value), int.Parse(month.Value), 10);
                }
                return date;
            }
        }
        YearTimeCondition year;
        MonthCondition month;
        DayTimeCondition day;
        HourTimeCondition hour;
        MinutesTimeCondition minutes;
        ToggleButton EmptyRoomOnlyButton;
        ToggleButton SetNowButton;
        public Condition(Context context)
        {
            #region 时间控件
            year = new YearTimeCondition(context);
            month = new MonthCondition(context);
            day = new DayTimeCondition(context, this);
            year.TimeChangeLisenter = day;
            month.TimeChangeLisenter = day;
            hour = new HourTimeCondition(context);
            minutes = new MinutesTimeCondition(context);
            #endregion
            #region 监听时间改变
            var intentFilter = new IntentFilter();
            intentFilter.AddAction(Intent.ActionTimeTick);//每分钟变化
            TimeChangeReceiver timeChangeReceiver = new TimeChangeReceiver();
            context.RegisterReceiver(timeChangeReceiver, intentFilter);
            timeChangeReceiver.condition = this;
            #endregion
            #region  空教室
            EmptyRoomOnlyButton = ((Activity)context).FindViewById<ToggleButton>(Resource.Id.EmptyRoomOnly);
            EmptyRoomOnlyButton.Checked = EmptyRoomOnly;
            EmptyRoomOnlyButton.CheckedChange += (sender, args) =>
            {
                EmptyRoomOnly = args.IsChecked;
            };
            #endregion
            #region 时间
            SetNowButton = ((Activity)context).FindViewById<ToggleButton>(Resource.Id.SetNow);
            SetNowButton.Checked = NowTime;
            SetScroll(!NowTime);
            SetNowButton.CheckedChange += (sender, args) =>
            {
                NowTime = args.IsChecked;
                if (NowTime) SetTimeToNow();
                SetScroll(!NowTime);
            };
            #endregion
        }
        public void SetScroll(bool EnableOrDisable)
        {
            year.EnableScroll = EnableOrDisable;
            minutes.EnableScroll = EnableOrDisable;
            hour.EnableScroll = EnableOrDisable;
            day.EnableScroll = EnableOrDisable;
            month.EnableScroll = EnableOrDisable;
        }
        public void SetTimeToNow()
        {
            if (NowTime)
            {
                minutes.SetToNow();
                hour.SetToNow();
                day.SetToNow();
                month.SetToNow();
                year.SetToNow();
            }
        }
    }
    class TimeChangeReceiver : BroadcastReceiver
    {
        public Condition condition { get; set; }
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == Intent.ActionTimeTick)
            {
                condition.SetTimeToNow();
            }
        }
    }
    public interface OnSelectionChangeLisenter
    {
        void OnSelectionChange(string Item);
    }
    public interface OnTimeChangeLisenter
    {
        void OnTimeChange();
    }
    public class TimeCondition : OnSelectionChangeLisenter
    {
        public string Value = "";
        protected TimeAdapter ValueAdapter;
        protected TimeListView ValueListView;
        protected List<string> ValueList = new List<string>();
        public virtual void OnSelectionChange(string Item)
        {
            Value = Item;
        }
        public bool EnableScroll
        {
            set
            {
                ValueListView.Enabled = value;
            }
        }
    }

    public class MinutesTimeCondition : TimeCondition
    {
        public void SetToNow()
        {
            ValueListView.SetSelection(((int.MaxValue / 2) - ((int.MaxValue / 2) % ValueList.Count)) + DateTime.Now.Minute - ValueListView.mVisibleItemCount / 2);
            Value = DateTime.Now.Minute.ToString();
        }
        public MinutesTimeCondition(Context Context)
        {
            ValueListView = ((Activity)Context).FindViewById<TimeListView>(Resource.Id.MinutesList);
            for (int Minutes = 0; Minutes < 61; Minutes++)
                ValueList.Add(Minutes.ToString());
            ValueAdapter = new TimeAdapter(Context, ValueList);
            ValueListView.SetVisibleItemCount(3);
            ValueListView.Adapter = ValueAdapter;
            SetToNow();
            ValueListView.SelectionChangeLisenter = this;
            ValueListView.Enabled = false;
        }
    }
    public class HourTimeCondition : TimeCondition
    {
        public void SetToNow()
        {
            ValueListView.SetSelection(((int.MaxValue / 2) - ((int.MaxValue / 2) % ValueList.Count)) + DateTime.Now.Hour - ValueListView.mVisibleItemCount / 2);
            Value = DateTime.Now.Hour.ToString();
        }
        public HourTimeCondition(Context Context)
        {
            ValueListView = ((Activity)Context).FindViewById<TimeListView>(Resource.Id.HourList);
            for (int Minutes = 0; Minutes < 25; Minutes++)
                ValueList.Add(Minutes.ToString());
            ValueAdapter = new TimeAdapter(Context, ValueList);
            ValueListView.SetVisibleItemCount(3);
            ValueListView.Adapter = ValueAdapter;
            SetToNow();
            ValueListView.SelectionChangeLisenter = this;
        }
    }
    public class YearTimeCondition : TimeCondition
    {
        public OnTimeChangeLisenter TimeChangeLisenter { get; set; }
        public override void OnSelectionChange(string Item)
        {
            Value = Item;
            TimeChangeLisenter.OnTimeChange();
        }
        public void SetToNow()
        {
            int position = 0;
            foreach (string year in ValueList)
            {
                if (year == DateTime.Now.Year.ToString())
                {
                    ValueListView.SetSelection(position - 1);
                }
                position++;
            }
            Value = DateTime.Now.Year.ToString();
        }
        public YearTimeCondition(Context Context)
        {
            ValueListView = ((Activity)Context).FindViewById<TimeListView>(Resource.Id.YearList);
            ValueList.Add(null);
            for (int ye = 2016; ye < 2030; ye++)
                ValueList.Add(ye.ToString());
            ValueList.Add(null);
            ValueAdapter = new TimeAdapter(Context, ValueList);
            ValueAdapter.Loop = false;
            ValueListView.Adapter = ValueAdapter;
            SetToNow();
            ValueListView.SelectionChangeLisenter = this;
        }
    }
    public class MonthCondition : TimeCondition
    {
        public OnTimeChangeLisenter TimeChangeLisenter { get; set; }
        public override void OnSelectionChange(string Item)
        {
            Value = Item;
            TimeChangeLisenter.OnTimeChange();
        }
        public void SetToNow()
        {
            ValueListView.SetSelection(((int.MaxValue / 2) - ((int.MaxValue / 2) % ValueList.Count)) + DateTime.Now.Month - 1 - ValueListView.mVisibleItemCount / 2);
            Value = DateTime.Now.Month.ToString();
        }
        public MonthCondition(Context Context)
        {
            ValueListView = ((Activity)Context).FindViewById<TimeListView>(Resource.Id.MonthList);
            for (int Month = 1; Month < 13; Month++)
                ValueList.Add(Month.ToString());
            ValueAdapter = new TimeAdapter(Context, ValueList);
            ValueListView.SetVisibleItemCount(3);
            ValueListView.Adapter = ValueAdapter;
            SetToNow();
            ValueListView.SelectionChangeLisenter = this;
        }
    }
    public class DayTimeCondition : TimeCondition, OnTimeChangeLisenter
    {
        Condition condition;
        public void SetToNow()
        {
            ValueListView.SetSelection(((int.MaxValue / 2) - ((int.MaxValue / 2) % ValueList.Count)) + DateTime.Now.Day-1 - ValueListView.mVisibleItemCount / 2);
            Value = DateTime.Now.Day.ToString();
        }
        public void OnTimeChange()
        {
            int days = DateTime.DaysInMonth(condition.Time.Year, condition.Time.Month);
            int Selectday = int.Parse(ValueListView.GetItemAtPosition(ValueListView.FirstVisiblePosition + 1).ToString());
            ValueList.Clear();
            for (int day = 1; day < days + 1; day++)
                ValueList.Add(day.ToString());
            ValueAdapter.NotifyDataSetChanged();
            Selectday = (Selectday > ValueList.Count) ? ValueList.Count : Selectday;
            ValueListView.SetSelection(((int.MaxValue / 2) - ((int.MaxValue / 2) % ValueList.Count)) + Selectday - 1 - ValueListView.mVisibleItemCount / 2);
            Value = Selectday.ToString();
        }
        public DayTimeCondition(Context Context, Condition condition)
        {
            this.condition = condition;
            ValueListView = ((Activity)Context).FindViewById<TimeListView>(Resource.Id.DayList);
            for (int day = 1; day < 32; day++)
                ValueList.Add(day.ToString());
            ValueAdapter = new TimeAdapter(Context, ValueList);
            ValueListView.SetVisibleItemCount(3);
            ValueListView.Adapter = ValueAdapter;
            SetToNow();
            ValueListView.SelectionChangeLisenter = this;
        }
    }

    public class TimeListView : ListView, AbsListView.IOnScrollListener
    {
        public OnSelectionChangeLisenter SelectionChangeLisenter { get; set; }
        /// <summary>
        /// 中点的Y坐标
        /// </summary>
        private float centerY = 0f;
        /// <summary>
        /// 可视的item数
        /// </summary>
        public int mVisibleItemCount = 3;
        /// <summary>
        /// 没调整之前每个item的高度
        /// </summary>
        private float olditemheight = 0;
        /// <summary>
        /// 调整过后的每个item的高度
        /// </summary>
        private float newitemheight = -1;
        /// <summary>
        /// 当前选中项的序号
        /// </summary>
        private int curPosition = -1;
        public TimeListView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            //设置一个滚动监听
            SetOnScrollListener(this);
        }
        /// <summary>
        /// 设置ListView的显示item数
        /// </summary>
        /// <param name="count">必须是奇数 如果为-1 则表示只是使用动画效果的普通ListView</param>
        /// <returns></returns>
        public bool SetVisibleItemCount(int count)
        {
            if (count % 2 == 0)
            {
                return false;
            }
            else
            {
                mVisibleItemCount = count;
                return true;
            }

        }
        /// <summary>
        /// 在这里第一次调整item高度
        /// </summary>
        public override void OnWindowFocusChanged(bool hasWindowFocus)
        {
            base.OnWindowFocusChanged(hasWindowFocus);
            if (mVisibleItemCount != -1)
            {
                getNewItemHeight();
                reSetItemHeight();
            }
        }
        /// <summary>
        /// 调整每个可视的item的高度 以及对内容进行缩放
        /// </summary>
        public void reSetItemHeight()
        {
            for (int i = 0; i < ChildCount; i++)
            {
                //获取item
                View temp_view = GetChildAt(i);
                //设置item的高度
                ViewGroup.LayoutParams lp = temp_view.LayoutParameters;
                lp.Height = (int)newitemheight;
                temp_view.LayoutParameters = lp;
                //缩放内容 我的item的内容用一个LinearLayout包了起来 所以直接缩放LinearLayout
                LinearLayout item_ll_value = (LinearLayout)temp_view.FindViewById(Resource.Id.TimeLayout);
                item_ll_value.ScaleY = ((newitemheight / olditemheight) < 0 ? 0 : (newitemheight / olditemheight));
                item_ll_value.ScaleX = ((newitemheight / olditemheight) < 0 ? 0 : (newitemheight / olditemheight));
            }
        }
        /// <summary>
        /// 计算在给定的可视item数目下  每个item应该设置的高度
        /// </summary>
        private void getNewItemHeight()
        {
            //先把旧的item存起来
            olditemheight = GetChildAt(0).Height;
            //计算新的高度
            newitemheight = Height / mVisibleItemCount;
            if ((Height / mVisibleItemCount) % newitemheight > 0)
            {
                //除不尽的情况下把余数分给各个item，暂时发现分一次余数就够了，如果效果不理想就做个递归多分几次
                float remainder = (Height / mVisibleItemCount) % newitemheight;
                newitemheight = remainder / mVisibleItemCount;
            }
        }
        public void OnScrollStateChanged(AbsListView view, ScrollState scrollState)
        {
            //滚动结束之后开始正常回滚item并记录最中间的item为选中项  (必须设置可视项，ListView才会改为选择器模式)
            if (scrollState == (int)ScrollState.Idle && mVisibleItemCount != -1)
            {
                
                centerY = Height / 2;
                var Items = new Dictionary<int, float>();
                for (int i = 0; i < LastVisiblePosition-FirstVisiblePosition; i++)
                {
                    //获取item
                    View temp_view = GetChildAt(i);
                    //计算item的中点Y坐标
                    float itemY = temp_view.Bottom - (temp_view.Height / 2);
                    //计算距离
                    Items.Add(i, Math.Abs(itemY - centerY));
                }
                var NearItem = Items.First();
                foreach (var Item in Items)
                {
                    if (Item.Value < NearItem.Value)
                        NearItem = Item;
                }
                var nowPosition = FirstVisiblePosition + NearItem.Key;
                //使离中间最近的item回滚到中点位置
                SetSelection(nowPosition - mVisibleItemCount / 2);
                //计算当前选中项的序号
                //把当前选中项的序号存起来并通过listener回调出去
                if (SelectionChangeLisenter != null && nowPosition != curPosition)
                {
                    curPosition = nowPosition;
                    SelectionChangeLisenter.OnSelectionChange((string)GetItemAtPosition(curPosition));
                }
            }
        }
        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
            //计算中点
            centerY = Height / 2;
            //判断中点的有效性
            if (centerY <= 0)
            {
                return;
            }
            //开始对当前显示的View进行缩放
            for (int i = 0; i < visibleItemCount; i++)
            {
                //获取item
                View temp_view = GetChildAt(i);
                //计算item的中点Y坐标
                float itemY = temp_view.Bottom - (temp_view.Height / 2);
                //计算离中点的距离
                float distance = centerY;
                if (itemY > centerY)
                {
                    distance = itemY - centerY;
                }
                else
                {
                    distance = centerY - itemY;
                }

                //根据距离进行缩放
                temp_view.ScaleY = (1.1f - (distance / centerY) < 0 ? 0 : 1.1f - (distance / centerY));
                temp_view.ScaleX = (1.1f - (distance / centerY) < 0 ? 0 : 1.1f - (distance / centerY));
                //根据距离改变透明度
                temp_view.Alpha = (1.1f - (distance / centerY) < 0 ? 0 : 1.1f - (distance / centerY));
            }
        }
    }
    public class TimeAdapter : BaseAdapter<string>
    {
        List<string> items;
        Context context;
        public bool Loop = true;
        public TimeAdapter(Context context, List<string> values) : base()
        {
            this.context = context;
            items = values;

        }

        public override string this[int position]
        {
            get { return items[position % items.Count]; }
        }

        public override int Count
        {
            get
            {
                return Loop ? int.MaxValue : items.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.TimeList, parent, false);
            var Value = (TextView)convertView.FindViewById(Resource.Id.Text);
            Value.Text = items[position % items.Count];
            return convertView;
        }
    }
}