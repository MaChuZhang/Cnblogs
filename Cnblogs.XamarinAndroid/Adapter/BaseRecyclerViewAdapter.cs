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
using Android.Content.Res;

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
        public BaseHolder(View view) : base(view)
        {
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
        public BaseHolder SetImageLoader(int viewId, DisplayImageOptions options,string  url)
        {
            ImageView iv = GetView<ImageView>(viewId);
            //string url = iv.Tag.ToString();
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

    public class FooterViewHolder : RecyclerView.ViewHolder
    {
        public FooterViewHolder(View view) : base(view)
        {
        }
    }
    public class EmptyViewHolder : RecyclerView.ViewHolder
    {
        public EmptyViewHolder(View view) : base(view)
        {
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
            private const int VIEW_ITEM = 0;
            private const int VIEW_FOOTER = 1;
            private const int VIEW_NULL = 2;
            private View footerView, emptyView;
            private bool isBottom = false;
        public override int ItemCount
            {
                get
                {
                  if (list.Count == 0)
                   {
                      return EmptyViewCount();
                   }
                   return list.Count+FooterViewCount()+ EmptyViewCount();
                }
            }
        private int FooterViewCount()
        {
            if (footerView==null)
            {
                return 0;
            }
            return 1;
        }
        private int EmptyViewCount()
        {
            if (emptyView == null)
            {
                return 0;
            }
            return 1;
        }
        public override int GetItemViewType(int position)
        {
            if (ItemCount == 0)
            {
                return VIEW_NULL;
            }
            if (position  == ItemCount-1)
            {
                Activity activity = (Activity)context;
                int childCount = _recyclerView.GetLayoutManager().ChildCount - 1;
                if (childCount == 0)
                    childCount = 1;
                View lastChildView = _recyclerView.GetLayoutManager().GetChildAt(childCount - 1);
                int recyclerViewPb = _recyclerView.PaddingBottom;
                int lastChildBottom = lastChildView.Bottom;
                int recyclerViewBottom = _recyclerView.Bottom - _recyclerView.PaddingBottom;

                if (activity is StatusesCommentActivity)
                {
                    LinearLayout headerView = activity.FindViewById<LinearLayout>(Resource.Id.ly_headerView);
                    int headerTop = headerView.Top+headerView.Height;
                    LinearLayout  ly_item = activity.FindViewById<LinearLayout>(Resource.Id.ly_item);
                    int rHeight = 0;
                    if (ly_item != null)
                    {
                        rHeight= ly_item.Height * (ItemCount - 1);
                    }
                    Resources res = context.Resources;
                    DisplayMetrics dm = res.DisplayMetrics;
                    int windowHeight = dm.HeightPixels;
                    if(!isBottom)
                    {
                        isBottom = true;
                        return VIEW_ITEM;
                    }
                    if (windowHeight > headerTop + rHeight) //判断recyclerView是否超出屏幕
                        return VIEW_NULL;
                    return VIEW_FOOTER;
                }
                   return VIEW_FOOTER;
            }
            return VIEW_ITEM;
        }

        public void SetNewData(List<T> list)
        {
            this.list = list == null ? new List<T>() : list;
            NotifyDataSetChanged();
        }
        //public int GetHeaderViewCount()
        //{
        //    if()
        //}
        public void SetItemCount(List<T> list)
        {
           //this.list
        }

        public void SetFooterView(int resId)
        {
            this.footerView = inflater.Inflate(resId,null);
            NotifyDataSetChanged();
        }

        public void SetEmptyView(View emptyView)
        {
           // if(emp)
        }
        protected RecyclerView.ViewHolder CreateBaseViewHolder(View view)
        {
            return new BaseHolder(view);
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
            if (viewType == VIEW_FOOTER)
            {
                footerView = inflater.Inflate(Resource.Layout.item_recyclerView_footer_loading,parent,false);
                return new  FooterViewHolder(footerView);
            }

            if (viewType == VIEW_NULL)
            {
                emptyView = inflater.Inflate(Resource.Layout.item_recyclerView_footer_empty, parent, false);
                return new EmptyViewHolder(emptyView);
            }
            View view = inflater.Inflate(itemLayoutId, parent, false);
            return BaseHolder.GetRecyclerHolder(context, view, ItemClick);
        }
         public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
         {
              if(holder is BaseHolder)
              {
                //myHolder.ItemView.Tag = position;
                //myHolder.
                BaseHolder myHolder = holder as BaseHolder;
                OnConvertView(myHolder, position);
              }
          }
     }
}