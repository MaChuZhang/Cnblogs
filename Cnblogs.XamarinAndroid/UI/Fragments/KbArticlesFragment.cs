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
using Android.Support.V4.View;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Support.Design.Widget;
using Cnblogs.ApiModel;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Cnblogs.XamarinAndroid.UI;
using Cnblogs.HttpClient;
using System.Threading.Tasks;
using Cnblogs.XamarinAndroid.UI.Widgets;

namespace Cnblogs.XamarinAndroid
{
    public class KbArticlesFragment : Fragment, SwipeRefreshLayout.IOnRefreshListener
    {
        private RecyclerView _recyclerView;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private BaseRecyclerViewAdapter<KbArticles> adapter;
        private List<KbArticles> kbArticlesList = new List<KbArticles>();
        private int pageIndex=1;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu=true;
            //Android.Support.V7.Widget.Toolbar toolbar = Activity.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //toolbar.Title = Resources.GetString(Resource.String.kbArticles);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
                base.OnCreateView(inflater, container, savedInstanceState);
               return inflater.Inflate(Resource.Layout.fragment_kbArticles, container, false);
       
        }

        public override async void OnViewCreated(View view,Bundle  savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            _swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            _swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            _swipeRefreshLayout.SetOnRefreshListener(this);
            _swipeRefreshLayout.Post(() =>
            {
                _swipeRefreshLayout.Refreshing = true;

            });
            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(this.Activity));
            _recyclerView.AddItemDecoration(new RecyclerViewDecoration(this.Activity, (int)Orientation.Vertical));
            kbArticlesList = await listKbArticlesServer(pageIndex);
            if (kbArticlesList != null)
            {
                initRecycler();
            }
           // RecyclerView.OnScrollListener scroll = new RecyclerViewOnScrollListtener(_swipeRefreshLayout, (Android.Support.V7.Widget.LinearLayoutManager)_recyclerView.GetLayoutManager(), adapter, LoadMore);
            //_recyclerView.AddOnScrollListener(scroll);
        }

        private async Task<List<KbArticles>> listKbArticlesServer(int _pageIndex)
        {
            pageIndex = _pageIndex;
            var result = await KbArticlesRequest.GetKbArticlesList(AccessTokenUtil.GetToken(this.Activity), pageIndex);
            if (result.Success)
            {
                _swipeRefreshLayout.Refreshing = false;
                try
                {
                   
                    await SQLiteUtil.UpdateKbArticlesList(result.Data);
                    return result.Data;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex.ToString());
                    return null;
                }
            }
            return null;
        }
        async void initRecycler()
        {
            adapter = new BaseRecyclerViewAdapter<KbArticles>(this.Activity, kbArticlesList, Resource.Layout.item_recyclerview_kbarticles);
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClick += (position, tag) =>
            {
                System.Diagnostics.Debug.Write(position, tag);
                AlertUtil.ToastShort(this.Activity, tag);
                DetailKbArticlesActivity.Enter(Activity,int.Parse(tag));
            };
            string read = Resources.GetString(Resource.String.read);
            string digg = Resources.GetString(Resource.String.digg);

            adapter.OnConvertView += (holder, position) =>
            {
                holder.SetText(Resource.Id.tv_dateAdded, kbArticlesList[position].DateAdded.ToCommonString());
                holder.SetText(Resource.Id.tv_viewCount, kbArticlesList[position].ViewCount + " " + read);
                holder.SetText(Resource.Id.tv_summary, kbArticlesList[position].Summary);
                holder.SetText(Resource.Id.tv_diggCount, kbArticlesList[position].Diggcount + " " + digg);
                holder.SetText(Resource.Id.tv_title, kbArticlesList[position].Title);
                holder.SetText(Resource.Id.tv_author,kbArticlesList[position].Author);
                holder.SetTag(Resource.Id.ly_item, kbArticlesList[position].Id.ToString());
            };
            RecyclerView.OnScrollListener scroll = new RecyclerViewOnScrollListtener(_swipeRefreshLayout, (Android.Support.V7.Widget.LinearLayoutManager)_recyclerView.GetLayoutManager(), adapter, LoadMore);
            _recyclerView.AddOnScrollListener(scroll);
        }

        private async void LoadMore()
        {
            pageIndex++;
            var tempList = await listKbArticlesServer(pageIndex);
            kbArticlesList.AddRange(tempList);
            if (tempList.Count == 0)
            {
                return;
            }
            else if (kbArticlesList != null)
            {
                adapter.SetNewData(kbArticlesList);
                System.Diagnostics.Debug.Write("页数:" + pageIndex + "数据总条数：" + kbArticlesList.Count);
            }
        }

        public async void OnRefresh()
        {
            if (pageIndex > 1)
                pageIndex = 1;
            var tempList = await listKbArticlesServer(pageIndex);
            if (tempList != null)
            {
                kbArticlesList = tempList;
                _swipeRefreshLayout.Refreshing = false;
                adapter.SetNewData(tempList);
            }
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            Activity.MenuInflater.Inflate(Resource.Menu.setting, menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }
    }
}