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
using System.Threading.Tasks;
using System.Threading;
using Cnblogs.XamarinAndroid.UI.Widgets;

namespace Cnblogs.XamarinAndroid
{
    public class QuestionTabFragment : Fragment,SwipeRefreshLayout.IOnRefreshListener
    {
        private RecyclerView _recyclerView;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private BaseRecyclerViewAdapter<QuestionModel> adapter;
        private int position;
        private DisplayImageOptions options;
        private int pageIndex = 1;
        private bool isMy; 
        private List<QuestionModel> listQuestion = new List<QuestionModel>();
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            position = Arguments.GetInt("position");
            isMy = Arguments.GetBoolean("isMy");
            //显示图片配置
            options = new DisplayImageOptions.Builder()
                  .ShowImageOnFail(Resource.Drawable.Icon)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .ShowImageOnFail(Resource.Drawable.icon_user)
                  .ShowImageOnLoading(Resource.Drawable.icon_user)
                  .CacheOnDisk(true)
                  .Displayer(new DisplayerImageCircle(20))
                  .Build();
        }
        public static QuestionTabFragment Instance(int position,bool isMy)
        {
            QuestionTabFragment rf = new QuestionTabFragment();
            Bundle b = new Bundle();
            b.PutInt("position",position);
            b.PutBoolean("isMy",isMy);
            rf.Arguments = b;
            return rf;
        }

        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
             base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.fragment_recyclerview,container,false);
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            _swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            _swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            _swipeRefreshLayout.SetOnRefreshListener(this);
            _swipeRefreshLayout.Post(() =>
            {
                _swipeRefreshLayout.Refreshing = true;
            });
            _swipeRefreshLayout.PostDelayed(() =>
            {
                System.Diagnostics.Debug.Write("PostDelayed刷新已经完成");
                _swipeRefreshLayout.Refreshing = false;
            }, 4000);
            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(this.Activity));
            _recyclerView.AddItemDecoration(new RecyclerViewDecoration(this.Activity, (int)Orientation.Vertical));

            listQuestion = await listStatusLocal();
            if (listQuestion.Count > 0)
            {
                initRecycler();
            }
            else
            {
                listQuestion = await listQuestionServer(pageIndex);
                if(listQuestion.Count>0)
                {
                  initRecycler();
                }
            }
            listQuestion = await listQuestionServer(pageIndex);
            if (listQuestion.Count > 0)
            {
                initRecycler();
            }

            RecyclerView.OnScrollListener scroll = new RecyclerViewOnScrollListtener(_swipeRefreshLayout,(Android.Support.V7.Widget.LinearLayoutManager)_recyclerView.GetLayoutManager(), adapter, LoadMore);
            _recyclerView.AddOnScrollListener(scroll);
        }
        private async void LoadMore()
        {
            pageIndex++;
            var tempList = await listQuestionServer(pageIndex);
            listQuestion.AddRange(tempList);
            if (tempList.Count==0)
             {
                return;
            }
            else if (listQuestion != null)
            {
               adapter.SetNewData(listQuestion);
                System.Diagnostics.Debug.Write("页数:"+pageIndex+"数据总条数："+listQuestion.Count);
            }
        }
        async void initRecycler()
        {
            adapter = new BaseRecyclerViewAdapter<QuestionModel>(this.Activity, listQuestion, Resource.Layout.item_recyclerview_question);
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClick += (position, tag) =>
            {
                System.Diagnostics.Debug.Write(position, tag);
                AlertUtil.ToastShort(this.Activity, tag);
                QuestionActivity.Enter(Activity,int.Parse(tag));
            };
                string read = Resources.GetString(Resource.String.read);
                string answer = Resources.GetString(Resource.String.answer);
                adapter.OnConvertView += (holder, position) =>
                {
                    var model = listQuestion[position];
                    holder.SetText(Resource.Id.tv_dateAdded,model.DateAdded.ToCommonString());
                    holder.SetText(Resource.Id.tv_summary, model.Summary);
                    holder.SetText(Resource.Id.tv_title, model.Title);
                    holder.SetText(Resource.Id.tv_summary, model.Summary);
                    holder.SetText(Resource.Id.tv_viewCount, read+" "+model.ViewCount.ToString());
                    holder.SetText(Resource.Id.tv_answerCount, answer +" "+ model.AnswerCount.ToString());
                    holder.SetText(Resource.Id.tv_awardCount, model.Award.ToString());
                    TextView tv_tags= (holder.GetView<TextView>(Resource.Id.tv_tags));
                    if (!string.IsNullOrEmpty(model.Tags))
                    {
                        tv_tags.Visibility = ViewStates.Visible;
                        tv_tags.Text = model.Tags.Replace(',', ' ');
                    }
                    else
                    {
                        tv_tags.Visibility = ViewStates.Gone;
                    }
                    if (model.QuestionUserInfo != null && model.QuestionUserInfo.UserID > 0)
                    {
                        holder.SetText(Resource.Id.tv_userName, model.QuestionUserInfo.UserName);
                        holder.SetImageLoader(Resource.Id.iv_avatar, options, Constact.CnblogsPic+model.QuestionUserInfo.IconName );
                    }
                    
                    holder.SetTag(Resource.Id.ly_item, model.Qid.ToString());
                };
        }
        private async Task<List<QuestionModel>> listQuestionServer(int _pageIndex)
        {
            pageIndex = _pageIndex;
            var result = new ApiResult<List<QuestionModel>>();
            if (isMy)
            {
                result = await QuestionRequest.ListQuestion(UserTokenUtil.GetToken(this.Activity), position, pageIndex,true);
            }
            else
            {
                result = await QuestionRequest.ListQuestion(AccessTokenUtil.GetToken(this.Activity), position, pageIndex,false);
            }
            if (result.Success)
            {
                _swipeRefreshLayout.Refreshing = false;
                try
                {
                    await SQLiteUtil.UpdateListQuestion(result.Data);
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
        private async Task<List<QuestionModel>> listStatusLocal()
        {
            listQuestion = await SQLiteUtil.SelectListQuestion(Constact.PageSize);
            return listQuestion;
        }

        public async void OnRefresh()
        {
            if(pageIndex>1)
                pageIndex = 1; 
            var tempList  = await listQuestionServer(pageIndex);
            if (tempList != null)
            {
                listQuestion = tempList;
                _swipeRefreshLayout.Refreshing = false;
                adapter.SetNewData(tempList);
            }
        }
    }
}