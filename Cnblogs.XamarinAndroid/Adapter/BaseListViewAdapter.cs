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
using Android.Graphics;

namespace Cnblogs.XamarinAndroid
{
    public class ViewHolder : Java.Lang.Object
    {
        private SparseArray<View> Views;
        View ConvertView;
        int mPosition;
        private ViewHolder(Context _context, ViewGroup parent, int itemLayoutId, int position)
        {
            this.mPosition = position;
            Views = new SparseArray<View>();
            ConvertView = LayoutInflater.From(_context).Inflate(itemLayoutId, null);
            ConvertView.Tag = this;
        }
        public static ViewHolder Get(Context context, View convertView, ViewGroup parent, int itemLayoutId, int position)
        {
            if (convertView == null)
            {
                return new ViewHolder(context, parent, itemLayoutId, position);
            }
            else
            {
                ViewHolder holder = (ViewHolder)convertView.Tag;
                holder.mPosition = position;
                return holder;
            }
        }
        public T GetView<T>(int viewId) where T : View
        {
            View view = Views.Get(viewId);
            if (view == null)
            {
                view = ConvertView.FindViewById<T>(viewId);
                Views.Put(viewId, view);
            }
            return (T)view;
        }
        public View GetConvertView()
        {
            return ConvertView;
        }
        /// <summary>
        /// 给TextView 设置文本
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public ViewHolder SetText(int viewId, string text)
        {
            TextView view = GetView<TextView>(viewId);
            view.Text = text;
            return this;
        }

        /// <summary>
        /// 给ImageView 设置图片
        /// </summary>
        public ViewHolder SetImageBitMap(int viewId, Bitmap bm)
        {
            ImageView view = GetView<ImageView>(viewId);
            view.SetImageBitmap(bm);
            return this;
        }
    }
    public abstract class BaseListViewAdapter<T> : BaseAdapter
    {
        Context mContext;
        List<T> mData;
        int mItemLayoutId;
        public BaseListViewAdapter(Context context, List<T> data, int itemLayoutId) : base()
        {
            this.mContext = context;
            mData = data;
            mItemLayoutId = itemLayoutId;
        }
        public override int Count
        {
            get
            {
                return mData.Count;
            }
        }
        public override Java.Lang.Object GetItem(int position)
        {
            return null;

        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = mData[position];
            ViewHolder viewHolder = ViewHolder.Get(mContext, convertView, parent, mItemLayoutId, position);
            convert(viewHolder, mData[position]);

            System.Diagnostics.Debug.Write("getView",position+""+mData[position]);
            return viewHolder.GetConvertView();
        }
        public abstract void convert(ViewHolder helper, T item);
    }
  
}