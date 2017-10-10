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
using Android.Support.V7.Widget;
using Android.Util;

namespace Cnblogs.XamarinAndroid
{
    public class BaseHolder : RecyclerView.ViewHolder
    {
        private SparseArray<View> views;
        private Context context;
        private BaseHolder(Context context, View itemView, Action<int> listener) : base(itemView)
        {
            this.context = context;
            views = new SparseArray<View>(8);
            ItemView.Click += (s, e) => listener(base.Position);
        }
        public static BaseHolder GetRecyclerHolder(Context context, View itemView, Action<int> listener)
        {
            return new BaseHolder(context, itemView, listener);
        }
        public SparseArray<View> GetViews()
        {
            return this.views;
        }
        public T GetView<T>(int viewId) where T : View
        {
            View view = views.Get(viewId);
            if (view == null)
            {
                view = ItemView.FindViewById(viewId);
                views.Put(viewId, view);
            }
            return (T)view;
        }
        public BaseHolder SetText(int viewId, string text)
        {
            TextView tv = GetView<TextView>(viewId);
            tv.Text = text;
            return this;
        }
        public BaseHolder SetImageResource(int viewId, int drawabledId)
        {
            ImageView iv = GetView<ImageView>(viewId);
            iv.SetImageResource(drawabledId);
            return this;
        }
    }

    public class BaseRecyclerViewAdapter<T> : RecyclerView.Adapter
    {
            private Context context; //������
            private List<T> list;//����Դ
            private LayoutInflater inflater;//������
            private int itemLayoutId; // ����ID
            private bool isScrolling;//�Ƿ��ڹ���
            private RecyclerView _recyclerView;
            public delegate void Convert(BaseHolder holder, int position);
            public event Convert OnConvertView;
            public Action<int> ItemClick; //�����¼�

            public override int ItemCount
            {
                get
                {
                    return list.Count;
                }
            }

            //��RecyclerView�ṩ���ݵ�ʱ�����
            public override void OnAttachedToRecyclerView(RecyclerView recyclerView)
            {
                base.OnAttachedToRecyclerView(recyclerView);
                this._recyclerView = recyclerView;
            }
            public override void OnDetachedFromRecyclerView(RecyclerView recyclerView)
            {
                base.OnDetachedFromRecyclerView(recyclerView);
                this._recyclerView = null;
            }

            public BaseRecyclerViewAdapter(Context context, List<T> list, int itemLayoutId)
            {
                this.context = context;
                this.list = list;
                this.itemLayoutId = itemLayoutId;
                inflater = LayoutInflater.From(context);
            }
            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View view = inflater.Inflate(itemLayoutId, parent, false);
                //view.Click += AdapterItemClick;
                return BaseHolder.GetRecyclerHolder(context, view, ItemClick);
            }
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
            BaseHolder myHolder = holder as BaseHolder;
                //myHolder.ItemView.Tag = position;
                OnConvertView(myHolder, position);
            }
        }
}