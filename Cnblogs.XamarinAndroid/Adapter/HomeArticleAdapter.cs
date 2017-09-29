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

namespace Cnblogs.XamarinAndroid
{
    public class RecyclerViewAdapter : RecyclerView.Adapter
    {
        private List<string> data;
        private Context _context;
        public RecyclerViewAdapter(List<string> list, Context context)
        {
            data = list;
            _context = context;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyViewHolder myViewHolder = holder as MyViewHolder;
            myViewHolder.tvTitle.Text = data[position];
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewItem)
        {
            View view = LayoutInflater.From(_context).Inflate(Resource.Layout.item_recyclerView, parent, false);
            MyViewHolder holder = new MyViewHolder(view);
            return holder;
        }
        public override int ItemCount
        {
            get
            {
                return data.Count;
            }
        }
        public override void OnViewRecycled(Java.Lang.Object holder)
        {
            base.OnViewRecycled(holder);
            MyViewHolder myViewHolder = holder as MyViewHolder;
        }
    }
    public class MyViewHolder : RecyclerView.ViewHolder
    {
        public TextView tvTitle;
        public MyViewHolder(View itemView) : base(itemView)
        {
            tvTitle = itemView.FindViewById<TextView>(Resource.Id.tv_num);
        }
    }
}