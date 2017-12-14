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
using System.Threading;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "@string/bookmark_my", Theme ="@style/AppTheme")]
    public  class MyBookmarkActivity : BaseActivity, SwipeRefreshLayout.IOnRefreshListener
    {
        
        protected override int LayoutResourceId
        {
            get
            {
                return Resource.Layout.myBookmark;
            }
        }

        private RecyclerView _recyclerView;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private BaseRecyclerViewAdapter<BookmarksModel> adapter;
        private LinearLayout ly_expire;
        private TextView tv_startLogin;
        public string   blogApp; //博客名
        private int pageIndex = 1;
        private Token token;
        private List<BookmarksModel> bookMarkList = new List<BookmarksModel>();
        internal static void Enter(string  _blogApp,Context context)
        {
            Intent intent = new Intent(context, typeof(MyBlogActivity));
            intent.PutExtra("blogApp", _blogApp);
            context.StartActivity(intent);
        }
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetToolBarNavBack();
            StatusBarUtil.SetColorStatusBars(this);
            SetToolBarTitle(Resources.GetString(Resource.String.bookmark_my));
            _swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            _swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            _swipeRefreshLayout.SetOnRefreshListener(this);
            _swipeRefreshLayout.Post(() =>
            {
                _swipeRefreshLayout.Refreshing = true;

            });
            
            _swipeRefreshLayout.PostDelayed(() =>
            {
                System.Diagnostics.Debug.Write("PostDelayed方法已经完成");
                _swipeRefreshLayout.Refreshing = false;
            }, 3000);
            ly_expire = FindViewById<LinearLayout>(Resource.Id.ly_expire);
            tv_startLogin = FindViewById<TextView>(Resource.Id.tv_startLogin);
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(this));

             token = UserTokenUtil.GetToken(this);
            if (token.IsExpire)
            {
                ly_expire.Visibility = ViewStates.Visible;
                _swipeRefreshLayout.Visibility = ViewStates.Gone;
                tv_startLogin.Click += (s, e) =>
                {
                    StartActivity(new Intent(this, typeof(loginactivity)));
                };
                return;
            }
            else
            {
                ly_expire.Visibility = ViewStates.Gone;
                _swipeRefreshLayout.Visibility = ViewStates.Visible;
            }

                bookMarkList = await listBookmarkServer();
                if (bookMarkList != null&&bookMarkList.Count>0)
               {
                 initRecycler();
               }
        }

        private async void LoadMore()
        {
            pageIndex++;
            var tempList = await listBookmarkServer();
            bookMarkList.AddRange(tempList);
            if (tempList.Count == 0)
            {
                //adapter.SetFooterView(Resource.Layout.item_recyclerView_footer_empty);
                return;
            }
            else if (bookMarkList != null)
            {
                adapter.SetNewData(bookMarkList);
                System.Diagnostics.Debug.Write("页数:" + pageIndex + "数据总条数：" + bookMarkList.Count);
            }
        }
        async void initRecycler()
        {
            adapter = new BaseRecyclerViewAdapter<BookmarksModel>(this, bookMarkList, Resource.Layout.item_recyclerview_myBookmark, LoadMore);
            //  View  footerView = LayoutInflater.From(Activity).Inflate(Resource.Layout.item_recyclerView_footer_loading, null);
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClick+= (position, tag) =>
            {
             
                    System.Diagnostics.Debug.Write(position, tag);
                    AlertUtil.ToastShort(this, tag);
              //  var intent = new Intent(this, typeof(DetailArticleActivity));
                //intent.PutExtra("id", int.Parse(tag));
                //StartActivity(intent);
            };
            adapter.ItemLongClick += (tag, position) =>
            {
               
                AlertUtil.ToastShort(this,tag);
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetCancelable(true);
                string[] items = Resources.GetStringArray(Resource.Array.DialogMenu);

                ProgressDialog progressDialog = new ProgressDialog(this);
                progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                progressDialog.SetMessage("删除中....");
                var model = bookMarkList.Find(f=>f.WzLinkId==int.Parse(tag));
                builder.SetItems(items, (s, e) =>
                {
                    switch (e.Which)
                    {
                        case 0:
                            Delete(progressDialog,model);
                            break;
                        case 1:
                            AddBookmarkActivity.Enter(this,model.WzLinkId,model.LinkUrl,model.Title,model.Tag,model.Summary,"edit");
                            break;
                        case 2:
                            AlertUtil.ToastShort(this,"该功能暂时未推出，敬请期待");
                            break;
                    }
                });
                builder.Show();
            };
            string read = Resources.GetString(Resource.String.read);
            string comment = Resources.GetString(Resource.String.comment);
            string digg = Resources.GetString(Resource.String.digg);

            adapter.OnConvertView += (holder, position) =>
            {
                var model = bookMarkList[position];
                holder.SetText(Resource.Id.tv_dateAdded, model.DateAdded.ToString("yyyy-MM-dd HH:ss"));
                holder.SetText(Resource.Id.tv_title, model.Title);
                holder.SetText(Resource.Id.tv_summary, model.Summary);
                holder.SetText(Resource.Id.tv_url, model.LinkUrl);
                holder.GetView<CardView>(Resource.Id.ly_item).Tag = model.WzLinkId.ToString();
                TextView tv_tags = (holder.GetView<TextView>(Resource.Id.tv_tags));
                TextView tv_summary = (holder.GetView<TextView>(Resource.Id.tv_summary));                
                if (!string.IsNullOrEmpty(model.Tag))
                {
                    tv_tags.Visibility = ViewStates.Visible;
                    tv_tags.Text = model.Tag.Replace(","," ");
                    //holder.SetText(Resource.Id.tv_tags, model.Tag.Replace(",", " "));
                }
                else
                {
                    tv_tags.Visibility = ViewStates.Gone;
                }
                if (!string.IsNullOrEmpty(model.Summary))
                {
                    tv_summary.Visibility = ViewStates.Visible;
                    tv_summary.Text = model.Summary;
                }
                else
                {
                    tv_summary.Visibility = ViewStates.Gone;
                }
            };
        }
        private  void Delete(ProgressDialog progressDialog,BookmarksModel model)
        {
            progressDialog.Show();
             BookmarksRequest.Delete(token, model.WzLinkId,(response)=> {
                 if (response.IsSuccess)
                 {
                        progressDialog.Hide();
                         RunOnUiThread(() =>
                         {
                             //OnRefresh();
                             bookMarkList.Remove(model);
                             adapter.SetNewData(bookMarkList);
                             // OnRefresh();
                         });
                 }
                 else
                 {
                     progressDialog.Hide();
                     AlertUtil.ToastShort(this, response.Message);
                 }
             });
        }
        private async Task<List<BookmarksModel>> listBookmarkServer()
        {
            var result = await BookmarksRequest.List(UserTokenUtil.GetToken(this),Constact.PageSize,pageIndex);
            if (result.Success)
            {
                _swipeRefreshLayout.Refreshing = false;
                //bookMarkList = result.Data;
                try
                {
                    await SQLiteUtil.UpdateBookMarkList(result.Data);
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
        private async Task<List<BookmarksModel>> listArticleLocal()
        {
            bookMarkList = await SQLiteUtil.SelectBookMarkList(Constact.PageSize);
            return bookMarkList;
        }

        public async void OnRefresh()
        {
            try
            {
                if (pageIndex > 1)
                    pageIndex = 1;
                bookMarkList = await listBookmarkServer();
                if (bookMarkList != null)
                {
                    //bookMarkList = tempList;
                    _swipeRefreshLayout.Refreshing = false;
                    adapter.SetNewData(bookMarkList);
                }
            }
            catch (Exception ex)
            {
                AlertUtil.ToastShort(this,ex.ToString());
                System.Diagnostics.Debug.Write(ex.ToString());
            }
        }
        #region override method
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.add,menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnMenuItemClick(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.add)
            {
                AddBookmarkActivity.Enter(this,"add");
            }
            return base.OnMenuItemClick(item);
        }
        #endregion

    }
}