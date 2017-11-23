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
namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "@string/comment",Theme ="@style/AppTheme")]
    public  class ArticleCommentActivity : BaseActivity, SwipeRefreshLayout.IOnRefreshListener
    {
        
        protected override int LayoutResourceId
        {
            get
            {
                return Resource.Layout.articleComment;
            }
        }

        private RecyclerView _recyclerView;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private BaseRecyclerViewAdapter<ArticleCommentModel> adapter;
        public string   blogApp; //������
        private DisplayImageOptions options;
        private int pageIndex = 1; int postId;
        private EditText edit_content;
        private Button btn_submit;
        private List<ArticleCommentModel> commentList = new List<ArticleCommentModel>();
        internal static void Enter(Context context, string _blogApp,int postId)
        {
            Intent intent = new Intent(context, typeof(ArticleCommentActivity));
            intent.PutExtra("blogApp", _blogApp);
            intent.PutExtra("postId",postId);
            context.StartActivity(intent);
        }
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            blogApp = Intent.GetStringExtra("blogApp");
            postId = Intent.GetIntExtra("postId",0);
            SetToolBarNavBack();
            StatusBarUtil.SetColorStatusBars(this);
            SetToolBarTitle(Resources.GetString(Resource.String.comment));
            //��ʾͼƬ����
            options = new DisplayImageOptions.Builder()
                  .ShowImageForEmptyUri(Resource.Drawable.icon_user)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .ShowImageOnFail(Resource.Drawable.icon_user)
                  .ShowImageOnLoading(Resource.Drawable.icon_user)
                  .CacheOnDisk(true)
                  .Displayer(new DisplayerImageCircle(20))
                  .Build();
            edit_content = FindViewById<EditText>(Resource.Id.edit_content);
            btn_submit = FindViewById<Button>(Resource.Id.btn_submit);
            //btn_submit.enabvle
            btn_submit.Click += (s, e) =>
            {
                Add();
            };
            edit_content.TextChanged+=(s,e)=>
            {
                if (!string.IsNullOrEmpty(edit_content.Text))
                {
                    btn_submit.Enabled = true;
                }
                else
                    btn_submit.Enabled = false;
            };

            _swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            _swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            _swipeRefreshLayout.SetOnRefreshListener(this);
            _swipeRefreshLayout.Post(() =>
            {
                _swipeRefreshLayout.Refreshing = true;
            });
            _swipeRefreshLayout.PostDelayed(() =>
            {
                System.Diagnostics.Debug.Write("PostDelayed�����Ѿ����");
                _swipeRefreshLayout.Refreshing = false;
            }, 3000);
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(this));
            _recyclerView.AddItemDecoration(new RecyclerViewDecoration(this, (int)Orientation.Vertical));
            Token token = UserTokenUtil.GetToken(this);
            //if (token.IsExpire)
            //{
            //    ly_expire.Visibility = ViewStates.Visible;
            //    _swipeRefreshLayout.Visibility = ViewStates.Gone;
            //    tv_startLogin.Click += (s, e) =>
            //    {
            //        StartActivity(new Intent(this, typeof(loginactivity)));
            //    };
            //    return;
            //}
            //else
            //{
            //    ly_expire.Visibility = ViewStates.Gone;
            //    _swipeRefreshLayout.Visibility = ViewStates.Visible;
            //}
            try
            {
                commentList = await listArticleCommentServer();
                if (commentList != null)
                {
                    initRecycler();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.ToString());
            }
        }
        private async void Add()
        {
            ProgressDialog dialog = new ProgressDialog(this);
            dialog.SetTitle("����");
            dialog.SetMessage("�ύ������.....");
            string body = edit_content.Text.TrimEnd().TrimStart();
          
            if (string.IsNullOrEmpty(body))
            {
                AlertUtil.ToastShort(this, "����������");
                return;
            }
            dialog.Show();
            var  result= await  ArticleRequest.Add(UserTokenUtil.GetToken(this), blogApp, postId, body);
            if (result.Success)
            {
                dialog.Hide();
                AlertUtil.ToastShort(this, "���۳ɹ�");
                btn_submit.Enabled = false;
            }
            else
            {
                dialog.Hide();
                AlertUtil.ToastShort(this, result.Message);
                btn_submit.Enabled = true;
            }
        }
        private async void LoadMore()
        {
            pageIndex++;
            var tempList = await listArticleCommentServer();
            commentList.AddRange(tempList);
            if (tempList.Count == 0)
            {
                //adapter.SetFooterView(Resource.Layout.item_recyclerView_footer_empty);
                return;
            }
            else if (commentList != null)
            {
                //adapter.NotifyDataSetChanged();
                // adapter.NotifyItemChanged(commentList.Count + 1);
                //var ds= adapter.ItemCount;
                adapter.SetNewData(commentList);
                System.Diagnostics.Debug.Write("ҳ��:" + pageIndex + "������������" + commentList.Count);
            }
        }
         void initRecycler()
        {
            adapter = new BaseRecyclerViewAdapter<ArticleCommentModel>(this, commentList, Resource.Layout.item_recyclerview_statusComment, LoadMore);
            //  View  footerView = LayoutInflater.From(Activity).Inflate(Resource.Layout.item_recyclerView_footer_loading, null);
            _recyclerView.SetAdapter(adapter);
            //adapter.ItemClick += (position, tag) =>
            //{
            //    System.Diagnostics.Debug.Write(position, tag);
            //    AlertUtil.ToastShort(this, tag);
            //    var intent = new Intent(this, typeof(DetailArticleActivity));
            //    intent.PutExtra("id", int.Parse(tag));
            //    StartActivity(intent);
            //};
            adapter.ItemClick += (position, tag) =>
            {
                    System.Diagnostics.Debug.Write(position, tag);
                    var intent = new Intent(this, typeof(DetailArticleActivity));
                    intent.PutExtra("id", int.Parse(tag));
                    StartActivity(intent);
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
                var model = commentList[position];
                holder.SetText(Resource.Id.tv_commentUserName, model.Author);
                holder.SetText(Resource.Id.tv_commentDateAdded, model.DateAdded.ToCommonString());
                (holder.GetView<TextView>(Resource.Id.tv_commentContent)).SetText(HtmlUtil.GetHtml(model.Body), TextView.BufferType.Spannable);
                holder.SetText(Resource.Id.tv_floor, model.Floor+"¥");
                holder.GetView<LinearLayout>(Resource.Id.ly_item).Tag = model.Id.ToString();
                holder.SetImageLoader(Resource.Id.iv_commentUserIcon, options, model.FaceUrl);
            };
        }
        private async Task<List<ArticleCommentModel>> listArticleCommentServer()
        {
            try
            {
                var token = UserTokenUtil.GetToken(this);
                var result = await ArticleRequest.ListArticleComment(token, blogApp, postId, pageIndex);
                if (result.Success)
                {
                    _swipeRefreshLayout.Refreshing = false;
                    //commentList = result.Data;
                    try
                    {
                        await SQLiteUtil.UpdateArticleCommentList(result.Data);
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.ToString());
                return null;
            }
        }
        private async Task<List<ArticleCommentModel>> listArticleLocal()
        {
            commentList = await SQLiteUtil.SelectArticleCommentList(Constact.PageSize);
            return commentList;
        }

        public async void OnRefresh()
        {
            if (pageIndex > 1)
                pageIndex = 1;
            var tempList = await listArticleCommentServer();
            if (tempList != null)
            {
                commentList = tempList;
                _swipeRefreshLayout.Refreshing = false;
                adapter.SetNewData(tempList);
            }
        }
    }
}