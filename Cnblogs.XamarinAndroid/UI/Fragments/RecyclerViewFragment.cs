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
using Cnblogs.HttpClient;
using Cnblogs.ApiModel;
using Cnblogs.XamarinAndroid.UI;
using Com.Nostra13.Universalimageloader.Core;
using Android.Graphics;

namespace Cnblogs.XamarinAndroid
{
    public class RecyclerViewFragment : Fragment
    {
        private RecyclerView _recyclerView;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private BaseRecyclerViewAdapter<Article> adapter;
        private LinearLayout ly_menu;
        public int position;
        private DisplayImageOptions options;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            position = Arguments.GetInt("position");
            //œ‘ æÕº∆¨≈‰÷√
            options = new DisplayImageOptions.Builder()
                  .ShowImageOnFail(Resource.Drawable.Icon)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .ShowImageOnFail(Resource.Drawable.icon_user)
                  .ShowImageOnLoading(Resource.Drawable.icon_user)
                  .CacheOnDisk(true)
                  .Displayer(new DisplayerImageCircle(20))
                  .Build();
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
            ly_menu = Activity.FindViewById<LinearLayout>(Resource.Id.ly_menu);
            return inflater.Inflate(Resource.Layout.fragment_recyclerview,container,false);
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            _swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            _swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);

            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _recyclerView.SetLayoutManager(new LinearLayoutManager(this.Activity));
            var recyclerviewScroll = new RecyclerviewScroll();
            recyclerviewScroll.OnHide += delegate
            {
                
                DisplayMetrics dm = Activity.Resources.DisplayMetrics;
                int height = dm.HeightPixels;
                int width = dm.WidthPixels;
                ly_menu.Animate().TranslationY(height - ly_menu.Height).SetDuration(500).Start();
            };
            recyclerviewScroll.OnShow += delegate
            {
                ly_menu = Activity.FindViewById<LinearLayout>(Resource.Id.ly_menu);
                ly_menu.Animate().TranslationY(0).SetDuration(500).Start();
            };
            _recyclerView.AddOnScrollListener(recyclerviewScroll);
            var result = await ArticleRequest.GetArticleList(SharedDataUtil.GetToken(this.Activity));
            if (result.Success)
            {
                var articleList = result.Data;
                try
                {
                    await SQLiteUtil.UpdateArticleList(articleList);
                    var model = SQLiteUtil.SelectArticle(1);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex.ToString());
                }
                adapter = new BaseRecyclerViewAdapter<Article>(this.Activity, articleList, Resource.Layout.item_fragment_article);
                _recyclerView.SetAdapter(adapter);
                adapter.ItemClick += (position,tag) =>
                {
                    System.Diagnostics.Debug.Write(position,tag);
                    AlertUtil.ToastShort(this.Activity, tag);
                    var intent = new Intent(Activity, typeof(DetailArticleActivity));
                    intent.PutExtra("id",int.Parse(tag));
                    StartActivity(intent);
                };
                string read = Resources.GetString(Resource.String.read);
                string comment = Resources.GetString(Resource.String.comment);
                string digg = Resources.GetString(Resource.String.digg);
                adapter.OnConvertView += (holder, position) =>
                {
                    holder.SetText(Resource.Id.tv_author, articleList[position].Author);
                    holder.SetText(Resource.Id.tv_postDate, articleList[position].PostDate.ToCommonString());
                    holder.SetText(Resource.Id.tv_viewCount, articleList[position].ViewCount + " " + read);
                    holder.SetText(Resource.Id.tv_commentCount, articleList[position].CommentCount + " " + comment);
                    holder.SetText(Resource.Id.tv_description, articleList[position].Description);
                    holder.SetText(Resource.Id.tv_diggCount, articleList[position].Diggcount + " " + digg);
                    holder.SetText(Resource.Id.tv_title, articleList[position].Title);
                    holder.SetTag(Resource.Id.ly_item, articleList[position].Id.ToString());
                    holder.SetTagUrl(Resource.Id.iv_avatar,articleList[position].Avatar);
                    //ImageLoader.Instance.DisplayImage(articleList[position].Avatar, Resource.Id.iv_avatar);
                    holder.SetImageLoader(Resource.Id.iv_avatar,options);
                };
            }
            else {
                AlertUtil.ToastShort(this.Activity,result.Message);
            }
            _recyclerView.AddItemDecoration(new RecyclerViewDecoration(this.Activity, (int)Orientation.Vertical));
             
            //_recyclerView.Post(async() =>
            //{
            //    await ArticleRequest.GetArticleList(SharedDataUtil.GetToken(this.Activity),articleList=> {
            //        adapter = new BaseRecyclerViewAdapter<string>(this.Activity, articleList.Select(p=>p.BlogApp).ToList(), Resource.Layout.item_recyclerView);
            //        _recyclerView.SetAdapter(adapter);
            //        adapter.ItemClick += (position) =>
            //    {
            //       AlertUtil.ToastShort(this.Activity,list[position].ToString());
            //           };
            //adapter.OnConvertView += (holder, postion) =>
            //{
            //    holder.SetText(Resource.Id.tv_num,list[position]);
            //};
            //    },error=> {
            //        AlertUtil.ToastShort(this.Activity,error);
            //    });
            //});
        }
        //void ConvertView(BaseHolder holder,int position)
        //{
        //    holder.SetText();
        //}
    }
}