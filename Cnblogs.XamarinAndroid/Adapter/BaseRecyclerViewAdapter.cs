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
using Com.Nostra13.Universalimageloader.Core;

namespace Cnblogs.XamarinAndroid
{
    public class BaseHolder : RecyclerView.ViewHolder
    {
        private SparseArray<View> views;
        private Context context;
        private BaseHolder(Context context, View itemView, Action<int,string> listener) : base(itemView)
        {
            this.context = context;
            views = new SparseArray<View>(8);
            ItemView.Click += (s, e) => listener(base.Position,itemView.Tag.ToString());
        }
        public static BaseHolder GetRecyclerHolder(Context context, View itemView, Action<int,string> listener)
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
        public BaseHolder SetImageLoader(int viewId, DisplayImageOptions options)
        {
            ImageView iv = GetView<ImageView>(viewId);
            string url = iv.Tag.ToString();
            System.Diagnostics.Debug.Write("imgaeUrl",url);
            if (!url.Substring(url.Length - 4, 4).Contains(".png"))
                iv.SetImageResource(Resource.Drawable.noavatar);
            else
                ImageLoader.Instance.DisplayImage(url, iv, options);
            return this;
        }
        public BaseHolder SetTag(int viewId, string id)
        {
            LinearLayout tv = GetView<LinearLayout>(viewId);
            tv.Tag = id;
            return this;
        }
        public BaseHolder SetTagUrl(int viewId,string  url)
        {
            ImageView iv = GetView<ImageView>(viewId);
            iv.Tag = url;
            return this;
        }
    }

    public class BaseRecyclerViewAdapter<T> : RecyclerView.Adapter
    {
            private Context context; //上下文
            private List<T> list;//数据源
            private LayoutInflater inflater;//布局器
            private int itemLayoutId; // 布局ID
            private RecyclerView _recyclerView;
            public delegate void Convert(BaseHolder holder, int position);
            public event Convert OnConvertView;
            public Action<int,string> ItemClick; //单击事件

            public override int ItemCount
            {
                get
                {
                    return list.Count;
                }
            }

            //在RecyclerView提供数据的时候调用
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
                //myHolder.
                OnConvertView(myHolder, position);
            }
        }
}