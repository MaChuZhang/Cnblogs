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
using static Android.Views.View;
using Android.Support.V4.App;
using Newtonsoft.Json;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "@string/zzk", Theme = "@style/AppTheme")]
    public class SearchResultActivity : AppCompatActivity, IOnFocusChangeListener
    {

        private SwipeRefreshLayout swipeRefreshLayout;
        private RecyclerView _recyclerView;
        private BaseRecyclerViewAdapter<ZzkDocumentViewModel> adapter;
        private SearchView search_keyword;
        private string category;
        private string keyword;
        private int pageIndex = 1;
        private bool startQuery = false;
        private List<ZzkDocumentViewModel> searchList = new List<ZzkDocumentViewModel>();
        private TextView tv_question, tv_news, tv_kb, tv_blog;

        internal static void Enter(string category, Context context)
        {
            Intent intent = new Intent(context, typeof(SearchResultActivity));
            intent.PutExtra("category", category);
            context.StartActivity(intent);
        }

        internal static void Enter(string category, string keyword, bool startquery, Context context)
        {
            Intent intent = new Intent(context, typeof(SearchResultActivity));
            intent.PutExtra("category", category);
            intent.PutExtra("keyword", keyword);
            intent.PutExtra("startquery", startquery);
            context.StartActivity(intent);
        }
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.searchResult);
            var app = ((InitApp)ApplicationContext);
            if (app != null)
            {
                app.addActivity(this);
            }
            tv_question = FindViewById<TextView>(Resource.Id.tv_question);
            tv_news = FindViewById<TextView>(Resource.Id.tv_news);
            tv_kb = FindViewById<TextView>(Resource.Id.tv_kb);
            tv_blog = FindViewById<TextView>(Resource.Id.tv_blog);
            search_keyword = FindViewById<SearchView>(Resource.Id.search_keyword);
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorScheme(Resource.Color.primary);
            swipeRefreshLayout.Refreshing = false;
            StatusBarUtil.SetColorStatusBars(this);
            category = Intent.GetStringExtra("category");
            keyword = Intent.GetStringExtra("keyword");
            startQuery = Intent.GetBooleanExtra("startquery", false);
            
            
            initCategoryClick();//初始化category单击事件
            initCategoryByCategory(); //初始化category 选中的状态
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            toolbar.NavigationClick += (s, e) =>
            {
                ActivityCompat.FinishAfterTransition(this);
            };
            swipeRefreshLayout.Refresh += (s, e) =>
            {
                swipeRefreshLayout.PostDelayed(() => {
                    swipeRefreshLayout.Refreshing = false;
                }, 1000);
            };
            _recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(this));

            //search_keyword.SubmitButtonEnabled = true; //默认为false 不显示提交按钮，是否启用提交按钮
            search_keyword.OnActionViewExpanded(); //手动展开SearchView输入框，初始可以点击输入的状态
                                                   //search_keyword.SetIconifiedByDefault(false); //默认是为true 不显示输入框键盘,默认提交按钮不显示,该属性设置为false，则直接展开输入框弹出键盘
                                                   // search_keyword.RequestFocus(); //将控件设置成可获取焦点状态,默认是无法获取焦点的,只有设置成true,才能获取控件的点击事件
                                                   //search_keyword.Focusable = true;
            search_keyword.ClearFocus();
            search_keyword.Focusable = true;
            if (!string.IsNullOrEmpty(keyword))
            {
                search_keyword.QueryHint = keyword;
                
            }
            search_keyword.FocusChange += (s, e) =>
            {
                System.Diagnostics.Debug.Write(e.HasFocus);
                if (e.HasFocus)
                    SearchInputActivity.Enter(category, this);
            };

            search_keyword.SetOnQueryTextFocusChangeListener(this);
            search_keyword.QueryTextSubmit += async (s, e) =>
            {
                keyword = search_keyword.Query.Trim();
                if (keyword == null || keyword.Length == 0)
                    return;
                SearchHistoryShared.SetSearchHistory(keyword, this);
                swipeRefreshLayout.Refreshing = true;
                await listSearchService((list)=> {
                    swipeRefreshLayout.PostDelayed(() => {
                        swipeRefreshLayout.Refreshing = false;
                    }, 1000);
                    searchList = list;
                    initRecycler();
                });
            };

            if (startQuery)
            {
                initCategoryByCategory();
                SearchHistoryShared.SetSearchHistory(keyword, this);
                if (!string.IsNullOrEmpty(keyword))
                {
                    swipeRefreshLayout.Refreshing = true;
                    await listSearchService((list)=> {
                        swipeRefreshLayout.PostDelayed(()=> {
                            swipeRefreshLayout.Refreshing = false;
                        },1000);
                        searchList = list;
                        initRecycler();
                    });
                }
            }
        }

        void initCategoryClick()
        {
            tv_news.Click += delegate
            {
                initCategoryById(tv_news);
            };
            tv_kb.Click += delegate
            {
                initCategoryById(tv_kb);
            };
            tv_blog.Click += delegate
            {
                initCategoryById(tv_blog);
            };
            tv_question.Click += delegate
            {
                initCategoryById(tv_question);
            };
        }
        void setUnSelected()
        {
            tv_news.Selected = false;
            tv_blog.Selected = false;
            tv_question.Selected = false;
            tv_kb.Selected = false;
        }

        async void initCategoryById(View v)
        {
            setUnSelected();
            switch (v.Id)
            {
                case Resource.Id.tv_blog:
                    if (tv_blog.Selected)
                        return;
                    tv_blog.Selected = true;
                    if (string.IsNullOrEmpty(keyword))
                    {
                        search_keyword.QueryHint = "搜博客";
                    }
                    category = "Blog";
                    break;
                case Resource.Id.tv_news:
                    if (tv_news.Selected)
                        return;
                    tv_news.Selected = true;
                    if (string.IsNullOrEmpty(keyword))
                    {
                        search_keyword.QueryHint = "搜新闻";
                    }
                    category = "News";
                    break;
                case Resource.Id.tv_question:
                    if (tv_question.Selected)
                        return;
                    tv_question.Selected = true;
                    if (string.IsNullOrEmpty(keyword))
                    {
                        search_keyword.QueryHint = "搜问答";
                    }
                    category = "Question";
                    break;
                case Resource.Id.tv_kb:
                    if (tv_kb.Selected)
                        return;
                    tv_kb.Selected = true;
                    if (string.IsNullOrEmpty(keyword))
                    {
                        search_keyword.QueryHint = "搜知识库";
                    }
                    category = "Kb";
                    break;
                default:
                    if (tv_blog.Selected)
                        return;
                    tv_blog.Selected = true;
                    if (string.IsNullOrEmpty(keyword))
                    {
                        search_keyword.QueryHint = "搜博客";
                    }
                    category = "Blog";
                    break;
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                swipeRefreshLayout.Refreshing = true;
                await listSearchService((list) =>
                {
                    swipeRefreshLayout.PostDelayed(() =>
                    {
                        swipeRefreshLayout.Refreshing = false;
                    }, 1000);
                    searchList=list;
                    initRecycler();
                });
            }
        }
        void initCategoryByCategory()
        {
            setUnSelected();
            switch (category)
            {
                case "Blog":
                    tv_blog.Selected = true;
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        search_keyword.QueryHint=keyword;
                    }
                    category = "Blog";
                    break;
                case "News":
                    tv_news.Selected = true;
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        search_keyword.QueryHint = keyword;
                    }
                    category = "News";
                    break;
                case "Question":
                    tv_question.Selected = true;
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        search_keyword.QueryHint = keyword;
                    }
                    category = "Question";
                    break;
                case "Kb":
                    tv_kb.Selected = true;
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        search_keyword.QueryHint = keyword;
                    }
                    category = "Kb";
                    break;
                default:
                    tv_blog.Selected = true;
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        search_keyword.QueryHint = keyword;
                    }
                    category = "Blog";
                    break;
            }
        }
        //public override void OnBackPressed()
        //{
        //   // this.Finish();
        //    MainActivity.Enter(this);
        //    base.OnBackPressed();
        //}
        public void OnFocusChange(View v, bool hasFocus)
        {
            if (v.Id == Resource.Id.search_keyword&&hasFocus)
            {
                SearchInputActivity.Enter(category, this);
            }
        }
        private async Task listSearchService(Action<List<ZzkDocumentViewModel>> successCallBack)
        {
            var result = await ZzkService.List(UserTokenUtil.GetToken(this), pageIndex, category, keyword);
            if (result.Success)
            {
                try
                {
                    successCallBack(result.Data);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex.ToString());
                }
            }
        }
        private async void LoadMore()
        {
            pageIndex++;
             await listSearchService((list)=> {
                 searchList.AddRange(list);
                 adapter.SetNewData(searchList);
                 System.Diagnostics.Debug.Write("页数:" + pageIndex + "数据总条数：" + searchList.Count);
            });
        }
        async void initRecycler()
        {
            adapter = new BaseRecyclerViewAdapter<ZzkDocumentViewModel>(this, searchList, Resource.Layout.item_recyclerview_search, LoadMore);
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClick += (position, tag) =>
            {
                var tempModel = searchList[position];
                tempModel.Title = tempModel.Title.Replace("<strong>", "").Replace("</strong>", "");
                switch (category)
                {
                    case "Blog":
                        string tempstr= tempModel.Uri.Replace("http://www.cnblogs.com/", "");
                        string blogApp = tempstr.Substring(0,tempstr.IndexOf("/"));
                        Article article = new Article() { Id = int.Parse(tempModel.Id),Avatar="",BlogApp=blogApp, Title=tempModel.Title,Author=tempModel.UserName,Url=tempModel.Uri,PostDate=tempModel.PublishTime,CommentCount=tempModel.CommentTimes,ViewCount=tempModel.ViewTimes,Diggcount=tempModel.VoteTimes};
                        DetailBlogActivity.Enter(this, int.Parse(tag),article );
                        break;
                    case "News":
                        NewsViewModel news = new NewsViewModel() {Id=int.Parse(tempModel.Id),Title=tempModel.Title,ViewCount=tempModel.ViewTimes,CommentCount=tempModel.CommentTimes,DiggCount=tempModel.VoteTimes,DateAdded=tempModel.PublishTime};
                        DetailNewsActivity.Enter(this,int.Parse(tag),news);
                        break;
                    case "Question":
                        QuestionActivity.Enter(this,int.Parse(tag),true);
                        break;
                    case "Kb":
                        KbArticles kb = new KbArticles() { Id = int.Parse(tempModel.Id), Title = tempModel.Title, ViewCount = tempModel.ViewTimes, DateAdded = tempModel.PublishTime, Author = tempModel.UserName };
                        DetailKbArticlesActivity.Enter(this,int.Parse(tag),kb);
                        break;
                    default:
                        break;
                }
            };
            adapter.ItemLongClick += (tag, position) =>
            {
                AlertUtil.ToastShort(this, tag);
            };
            string read = Resources.GetString(Resource.String.read);
            string comment = Resources.GetString(Resource.String.comment);
            string digg = Resources.GetString(Resource.String.digg);

            adapter.OnConvertView += (holder, position) =>
            {
                var tempModel = searchList[position];
                if (tempModel.Id == null&&!string.IsNullOrEmpty(tempModel.Uri))//问答id返回的是null，自己正则获取
                {
                    Regex regex = new Regex("\\d");
                    tempModel.Id =regex.Match(tempModel.Uri).Value;
                }
                    //holder.SetText(Resource.Id.tv_dateAdded, tempModel.PublishTime.ToCommonString());
                    holder.SetText(Resource.Id.tv_viewCount, tempModel.ViewTimes + " " + read);
                holder.SetText(Resource.Id.tv_commentCount, tempModel.CommentTimes+" "+comment);
                holder.SetText(Resource.Id.tv_diggCount, tempModel.VoteTimes + " " + digg);
                holder.SetText(Resource.Id.tv_url, tempModel.Uri);
                holder.SetText(Resource.Id.tv_author, tempModel.UserName);
                
                holder.GetView<LinearLayout>(Resource.Id.ly_item).Tag = tempModel.Id.ToString();
                (holder.GetView<TextView>(Resource.Id.tv_title)).TextFormatted = Html.FromHtml(tempModel.Title.replaceStrongToFont());
                string tempstr = "<font color='#707070'>"+ tempModel.PublishTime.ToString("yyyy年MM月dd日 HH:mm") + "</font>  "+ tempModel.Content.replaceStrongToFont();
                (holder.GetView<TextView>(Resource.Id.tv_summary)).TextFormatted = Html.FromHtml(tempstr);
            };
        }

       
    }   
    }