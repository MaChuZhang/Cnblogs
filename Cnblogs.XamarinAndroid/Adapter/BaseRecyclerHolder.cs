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

namespace Cnblogs.XamarinAndroid.Adapter
{
    public class BaseRecyclerHolder : RecyclerView.ViewHolder
    {
        private SparseArray<View> views;
        private Context context;
        private BaseRecyclerHolder(Context context, View itemView,Action<int> listener) : base(itemView)
        {
            this.context = context;
            views = new SparseArray<View>(8);
            ItemView.Click += (s, e) => listener(base.Position);
        }
        public static BaseRecyclerHolder GetRecyclerHolder(Context context, View itemView,Action<int> listener)
        {
            return new Adapter.BaseRecyclerHolder(context, itemView,listener);
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
        public BaseRecyclerHolder SetText(int viewId, string text)
        {
            TextView tv = GetView<TextView>(viewId);
            tv.Text = text;
            return this;
        }
        public BaseRecyclerHolder SetImageResource(int viewId, int drawabledId)
        {
            ImageView iv = GetView<ImageView>(viewId);
            iv.SetImageResource(drawabledId);
            return this;
        }
    }

    public abstract class BaseRecyclerAdapter<T> : RecyclerView.Adapter
    {
        private Context context; //������
        private List<T> list;//����Դ
        private LayoutInflater inflater;//������
        private int itemLayoutId; // ����ID
        private bool isScrolling;//�Ƿ��ڹ���
        private RecyclerView _recyclerView;

        public Action<int> ItemClick; //�����¼�
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

        public BaseRecyclerAdapter(Context context,List<T> list,int itemLayoutId)
        {
            this.context = context;
            this.list = list;
            this.itemLayoutId = itemLayoutId;
            inflater = LayoutInflater.From(context);
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = inflater.Inflate(itemLayoutId,parent,false);
            //view.Click += AdapterItemClick;
            return BaseRecyclerHolder.GetRecyclerHolder(context,view, ItemClick);
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            BaseRecyclerHolder myHolder = holder as BaseRecyclerHolder;
            myHolder.ItemView.Tag = position;
            Convert(myHolder,list[position],position,isScrolling);
        }
        public abstract void Convert(BaseRecyclerHolder holder,T item,int position,bool  isScrolling);
    }     
}