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
        private LinearLayoutManager _linearLayoutManager;//���ֹ�����
        private BaseRecyclerViewAdapter<Article> _adapter;//recyclerView ������
        private Android.OS.Handler handler;
        public delegate void InsertData();//��Ӹ������ݵ�ί��
        private InsertData _InsertDataEvent; //���ظ�����¼�
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

        //��RecyclerView�Ļ���״̬�ı�ʱ����
        //����״̬��3�֣�0��ScrollStateIdle��ָ�뿪��Ļ1ScrollStateDragging����ָ������Ļ2ScrollStateSetting
        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);
            System.Diagnostics.Debug.Write("test", "newState:" + newState);
        }

        //��RecyclerView�ʱ����
        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);
            System.Diagnostics.Debug.Write("���ڻ���");
            int lastVisibleItemPosition = _linearLayoutManager.FindLastVisibleItemPosition();

            if (lastVisibleItemPosition == _adapter.ItemCount)
            {
                System.Diagnostics.Debug.Write("test", "loadding�Ѿ����");
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
                        System.Diagnostics.Debug.Write("test", "����more�Ѿ����");
                        _IsLoadingMore = false;
                  
                }
            }
        }
    }
}