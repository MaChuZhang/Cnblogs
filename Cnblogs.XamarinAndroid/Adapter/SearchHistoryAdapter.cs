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

namespace Cnblogs.XamarinAndroid
{
    public class SearchHistoryAdapter<T>:BaseListViewAdapter<T>
    {
        public  Action<string> ActionDelte;//É¾³ýÊÂ¼þ
        public SearchHistoryAdapter(Context context, List<T> data, int resId) : base(context,data,resId)
        {

        }
        public override void convert(ViewHolder holder, T item)
        {
            holder.SetText(Resource.Id.tv_keyword,item.ToString());
            holder.GetView<ImageButton>(Resource.Id.imgbtn_delete).Click += (s, e) =>
            {
                ActionDelte(item.ToString());
            };
        }
    }
}