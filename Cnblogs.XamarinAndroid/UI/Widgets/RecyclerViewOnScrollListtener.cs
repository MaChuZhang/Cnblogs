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
        private BaseRecyclerViewAdapter<Article> _adapter;//recyclerView 适配器
        private Android.OS.Handler handler;
        public delegate void InsertData();//添加更多数据的委托
        private InsertData _InsertDataEvent; //加载更多的事件
        private bool _IsLoadingMore;
        public RecyclerViewOnScrollListtener(SwipeRefreshLayout swipeRefreshLayout, LinearLayoutManager linearLayoutManager, BaseRecyclerViewAdapter<Article> recyclerViewAdapter, InsertData InsertDataEvent, bool IsLoadingMore)
        {
            _swipeRefreshLayout = swipeRefreshLayout;
            _linearLayoutManager = linearLayoutManager;
            _adapter = recyclerViewAdapter;
            _InsertDataEvent = InsertDataEvent;
            //handler = handle;
            _IsLoadingMore = IsLoadingMore;
        }

        //当RecyclerView的滑动状态改变时触发
        //滑动状态有3种，0：ScrollStateIdle手指离开屏幕1ScrollStateDragging：手指触碰屏幕2ScrollStateSetting
        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);
            System.Diagnostics.Debug.Write("test", "newState:" + newState);
        }

        //当RecyclerView活动时触发
        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);
            System.Diagnostics.Debug.Write("正在滑动");
            int lastVisibleItemPosition = _linearLayoutManager.FindLastVisibleItemPosition();

            if (lastVisibleItemPosition == _adapter.ItemCount)
            {
                System.Diagnostics.Debug.Write("test", "loadding已经完成");
                bool isRefreshing = _swipeRefreshLayout.Refreshing;
                if (isRefreshing)
                { 
                    _adapter.NotifyItemRemoved(_adapter.ItemCount);
                    return;
                }
                if (!_IsLoadingMore)
                {
                   // _IsLoadingMore = true;
                    
                        _InsertDataEvent();
                        System.Diagnostics.Debug.Write("test", "加载more已经完成");
                        _IsLoadingMore = false;
                  
                }
            }
        }
    }
}