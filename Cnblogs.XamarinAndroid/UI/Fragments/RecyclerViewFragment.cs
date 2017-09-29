using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using Fragment = Android.Support.V4.App.Fragment;
using Cnblogs.XamarinAndroid;

namespace Cnblogs.XamarinAndroid
{
    public class RecyclerViewFragment : Fragment
    {
        private RecyclerView _recyclerView;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private BaseRecyclerViewAdapter<string> adapter;
        public int position;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            position = Arguments.GetInt("position");

            // Create your fragment here
        }
        public static RecyclerViewFragment Instance(int position)
        {
            RecyclerViewFragment rf = new RecyclerViewFragment();
            Bundle b = new Bundle();
            b.PutInt("position",position);
            rf.Arguments = b;
            return rf;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
             base.OnCreateView(inflater, container, savedInstanceState);
             return inflater.Inflate(Resource.Layout.fragment_recyclerview,container,false);
        }

        public override void OnViewCreated(View view ,Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            _swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            _swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);

            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _recyclerView.SetLayoutManager(new  LinearLayoutManager(this.Activity));
            List<string> list = new List<string>() { "896", "168", "149", "126", "147", "789", "456", "456", "123" };

            adapter = new BaseRecyclerViewAdapter<string>(this.Activity, list, Resource.Layout.item_recyclerView);
            adapter.ItemClick += (position) =>
            {
                AlertUtil.ToastShort(this.Activity,list[position].ToString());
            };
            adapter.OnConvertView += (holder, postion) =>
            {
                holder.SetText(Resource.Id.tv_num,list[position]);
            };
            _recyclerView.SetAdapter(adapter);
        }
        //void ConvertView(BaseHolder holder,int position)
        //{
        //    holder.SetText();
        //}
    }
}