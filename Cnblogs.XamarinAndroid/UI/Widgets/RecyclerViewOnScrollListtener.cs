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
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Cnblogs.XamarinAndroid;
using Cnblogs.ApiModel;

namespace Cnblogs.XamarinAndroid.UI.Widgets
{
    public class RecyclerViewOnScrollListtener : RecyclerView.OnScrollListener
    {
        private SwipeRefreshLayout _swipeRefreshLayout;
        private LinearLayoutManager _linearLayoutManager;//布局管理器
        private RecyclerView.Adapter  _adapter;//recyclerView 适配器
        private Android.OS.Handler handler;
        public delegate void InsertData();//添加更多数据的委托
        private InsertData _InsertDataEvent; //加载更多的事件
        private bool _IsLoadingMore;
        public RecyclerViewOnScrollListtener(SwipeRefreshLayout swipeRefreshLayout, LinearLayoutManager linearLayoutManager, RecyclerView.Adapter recyclerViewAdapter, InsertData InsertDataEvent)
        {
            _swipeRefreshLayout = swipeRefreshLayout;
            _linearLayoutManager = linearLayoutManager;
            _adapter = recyclerViewAdapter;
            _InsertDataEvent = InsertDataEvent;
        }
        //当RecyclerView的滑动状态改变时触发
        //滑动状态有3种，0：ScrollStateIdle手指离开屏幕1ScrollStateDragging：手指触碰屏幕2ScrollStateSetting
        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);
            System.Diagnostics.Debug.Write("test", "newState:" + newState);
            int state = recyclerView.ScrollState;
            if (state == RecyclerView.ScrollStateIdle)
            {
                int lastVisibleItemPosition = _linearLayoutManager.FindLastVisibleItemPosition();
                if (lastVisibleItemPosition + 1 == _adapter.ItemCount)
                {
                    System.Diagnostics.Debug.Write("test", "loadding已经完成"+state);
                   
                      _InsertDataEvent();
                }
            }
        }

        //当RecyclerView活动时触发
        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);
            System.Diagnostics.Debug.Write("正在滑动");
        }
    }
}