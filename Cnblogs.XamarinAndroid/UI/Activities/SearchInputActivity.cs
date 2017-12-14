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
using Com.Nostra13.Universalimageloader.Core;
using Android.Support.V7.Widget;
using Cnblogs.ApiModel;
using Android.Support.V4.Widget;
using Android.Graphics;
using System.Threading.Tasks;
using Cnblogs.HttpClient;
using Cnblogs.XamarinAndroid.UI.Widgets;
using Android.Support.V7.App;
using Android.Text;
using System.Text.RegularExpressions;
using SearchView = Android.Support.V7.Widget.SearchView;
using Android.Support.V4.App;
using Android.Util;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "@string/zzk",Theme ="@style/AppTheme")]
    public  class SearchInputActivity : AppCompatActivity
    {
        private ListView lv_searchHistory;
        private SearchView search_keyword;
        private string category,keyword;
        private ImageButton imgbtn_delete;
        private TextView tv_deleteAll;
        private SearchHistoryAdapter<string> adapter;
        List<string> list;

        internal static void Enter(string category,Context context)
        {
            Intent intent = new Intent(context, typeof(SearchInputActivity));
            intent.PutExtra("category", category);
            //intent.PutExtra("keyword", keyword);
            context.StartActivity(intent);
        }

        void initSearchHistory()
        {
            lv_searchHistory = FindViewById<ListView>(Resource.Id.lv_searchHistory);
            imgbtn_delete = FindViewById<ImageButton>(Resource.Id.imgbtn_delete);
            tv_deleteAll = FindViewById<TextView>(Resource.Id.tv_deleteAll);

            tv_deleteAll.Click += (s, e) =>
            {
                bool result = SearchHistoryShared.DeleteSearchHistory(this);
                if (result)
                {
                    lv_searchHistory.Visibility = ViewStates.Gone;
                    tv_deleteAll.Text ="û��������¼";
                    tv_deleteAll.SetTextColor(Resources.GetColor(Resource.Color.lightBlack));
                }
            };
            Dictionary<string,DateTime> dict = SearchHistoryShared.GetSearchHistory(this);
            if (dict != null && dict.Count > 0)
            {
                tv_deleteAll.Visibility = ViewStates.Visible;
                list = dict.Select(s => s.Key).ToList();
                adapter = new SearchHistoryAdapter<string>(this, list, Resource.Layout.item_listview_searchhistory);
                adapter.ActionDelte +=(keyword)=>
                {
                    Delete(keyword);
                };
                lv_searchHistory.Adapter = adapter;
                fixedListView(list.Count);
                lv_searchHistory.ItemClick += (s, e) =>
                {
                    keyword = list[e.Position];
                    SearchResultActivity.Enter(category, keyword, true, this);
                };
            }
            else
            {
                tv_deleteAll.Visibility = ViewStates.Visible;
                tv_deleteAll.Text = "û��������¼";
                tv_deleteAll.SetTextColor(Resources.GetColor(Resource.Color.lightBlack));
            }
        }
        /// <summary>
        /// �̶�ListView�߶ȷ�ֹGetView�ظ�ִ��
        /// </summary>
        /// <param name="listview"></param>
        /// <param name="itemCount"></param>
        void fixedListView(int itemCount)
        {
            ViewGroup.LayoutParams _params= lv_searchHistory.LayoutParameters;
            var  heightdp = Resources.GetDimension(Resource.Dimension.dp50)+ Resources.GetDimension(Resource.Dimension.dp1);
            _params.Height = (int)heightdp * itemCount;
            lv_searchHistory.LayoutParameters = _params;
        }
        private void Delete(string keyword){
            var result= SearchHistoryShared.DeleteSearchHistory(keyword,this);
            if (result)
            {
                list.Remove(keyword);
                adapter.NotifyDataSetChanged();
                fixedListView(list.Count);//��������listview�ĸ߶�
            }
        }
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.searchInput);
            var app = ((InitApp)ApplicationContext);
            if (app != null)
            {
                app.addActivity(this);
            }
            StatusBarUtil.SetColorStatusBars(this);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            toolbar.NavigationClick += (s, e) =>
            {
                //ActivityCompat.FinishAfterTransition(this);
                ActivityCompat.FinishAfterTransition(this);
                this.Finish();
            };

            initSearchHistory();
            category = Intent.GetStringExtra("category");
            keyword = Intent.GetStringExtra("keyword");
            search_keyword = FindViewById<SearchView>(Resource.Id.search_keyword);
            search_keyword.QueryTextSubmit+= (h, _keyword) =>
            {
                keyword = _keyword.Query.Trim();
                if (keyword == null || keyword.Length == 0)
                    return;
                SearchResultActivity.Enter(category, keyword, true, this);
            };
            //search_keyword.SetIconifiedByDefault(false); //����չ��ͼ�����ʽ
            search_keyword.OnActionViewExpanded(); //��ʼ���Ե�������״̬
            search_keyword.RequestFocus(); //���ؼ����óɿɻ�ȡ����״̬,Ĭ�����޷���ȡ�����,ֻ�����ó�true,���ܻ�ȡ�ؼ��ĵ���¼�
            search_keyword.Focusable = true;
        }
        public override void OnBackPressed()
        {
            this.Finish();
            base.OnBackPressed();
        }
    }   
 }