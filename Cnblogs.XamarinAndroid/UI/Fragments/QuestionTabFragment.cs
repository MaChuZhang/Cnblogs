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
        private LinearLayout ly_expire;
        private TextView tv_startLogin;
        private int position;
        private DisplayImageOptions options;
        private int pageIndex = 1;
        private bool isMy;
        private UserInfo userInfo;
        private List<QuestionModel> listQuestion = new List<QuestionModel>();
        private bool isFirstRefresh = true;
        private Token userToken, accessToken;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            position = Arguments.GetInt("position");
            isMy = Arguments.GetBoolean("isMy");
            //œ‘ æÕº∆¨≈‰÷√
            options = new DisplayImageOptions.Builder()
                .ShowImageForEmptyUri(Resource.Drawable.icon_yuanyou)
                  .ShowImageOnFail(Resource.Drawable.icon_yuanyou)
                  .ShowImageOnLoading(Resource.Drawable.icon_user)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .CacheOnDisk(true)
                 // .Displayer(new DisplayerImageCircle(20))
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
            ly_expire = view.FindViewById<LinearLayout>(Resource.Id.ly_expire);
            tv_startLogin = view.FindViewById<TextView>(Resource.Id.tv_startLogin);
            userInfo = UserInfoShared.GetUserInfo(this.Activity); ;

            _swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);

            _swipeRefreshLayout.SetOnRefreshListener(this);


            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(this.Activity));

             userToken = UserTokenUtil.GetToken(Activity);
            accessToken = AccessTokenUtil.GetToken(Activity);
            if (isMy && userToken.IsExpire)
            {
                ly_expire.Visibility = ViewStates.Visible;
                _swipeRefreshLayout.Visibility = ViewStates.Gone;
                tv_startLogin.Click += (s, e) =>
                {
                    Activity.StartActivity(new Intent(Activity, typeof(loginactivity)));
                };
            }
            else
            {
                ly_expire.Visibility = ViewStates.Gone;
                _swipeRefreshLayout.Visibility = ViewStates.Visible;
                listQuestion = await SQLiteUtil.SelectListQuestion(Constact.PageSize, isMy);
                if (listQuestion != null && listQuestion.Count > 0)
                {
                    initRecycler();
                }
                OnRefresh();
            }
        }
        private async void LoadMore()
        {
            if (_swipeRefreshLayout.Refreshing)
            {
                _swipeRefreshLayout.Post(() =>
                {
                    _swipeRefreshLayout.Refreshing = false;
                });
            }
            pageIndex++;
            var result = new ApiResult<List<QuestionModel>>();
            if (isMy)
            {
                
                result = await QuestionService.ListQuestion(userToken, position, pageIndex, true, userInfo.SpaceUserId);
            }
            else
            {
                result = await QuestionService.ListQuestion(accessToken, position, pageIndex, false, 0);
            }
            if (result.Success)
            {
                var tempList = result.Data;
                listQuestion.AddRange(tempList);
                adapter.SetNewData(listQuestion);
            }
            else
            {
                AlertUtil.ToastShort(Activity,result.Message);
            }
        }

     
        void initRecycler()
        {
            adapter = new BaseRecyclerViewAdapter<QuestionModel>(this.Activity, listQuestion, Resource.Layout.item_recyclerview_question, LoadMore);
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClick += (position, tag) =>
            {
                System.Diagnostics.Debug.Write(position, tag);
                //AlertUtil.ToastShort(this.Activity, tag);
                QuestionActivity.Enter(Activity, int.Parse(tag));
            };
            adapter.ItemLongClick += (tag, position) =>
            {
                //AlertUtil.ToastShort(this.Activity, tag);
            };
            string read = Resources.GetString(Resource.String.read);
            string answer = Resources.GetString(Resource.String.answer);
            try
            {
                adapter.OnConvertView += (holder, position) =>
                {
                    var model = listQuestion[position];
                    holder.SetText(Resource.Id.tv_dateAdded, model.DateAdded.ToCommonString());
                    holder.SetText(Resource.Id.tv_title, model.Title);
                    //holder.SetText(Resource.Id.tv_summary, model.Summary);
                    //holder.SetText(Resource.Id.tv_viewCount, read + " " + model.ViewCount.ToString());
                    holder.SetText(Resource.Id.tv_answerCount, model.AnswerCount.ToString());
                    holder.SetText(Resource.Id.tv_awardCount, model.Award.ToString());
                    TextView tv_tags = (holder.GetView<TextView>(Resource.Id.tv_tags));
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
                        holder.SetImageLoader(Resource.Id.iv_avatar, options, Constact.CnblogsPic + model.QuestionUserInfo.IconName);
                    }
                    holder.GetView<CardView>(Resource.Id.ly_item).Tag = model.Qid.ToString();
                    //holder.SetTag(Resource.Id.ly_item, model.Qid.ToString());
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.ToString());
            }
        }
        private async Task<List<QuestionModel>> listQuestionService()
        {
            var result = new ApiResult<List<QuestionModel>>();
            if (isMy)
            {
                var user = UserInfoShared.GetUserInfo(this.Activity);
                result = await QuestionService.ListQuestion(UserTokenUtil.GetToken(this.Activity), position, pageIndex,true,user.SpaceUserId);
            }
            else
            {
                result = await QuestionService.ListQuestion(AccessTokenUtil.GetToken(this.Activity), position, pageIndex,false,0);
            }
            if (result.Success)
            {
                _swipeRefreshLayout.Refreshing = false;
                try
                {
                    await SQLiteUtil.UpdateListQuestion(result.Data,isMy);
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


        public async void OnRefresh()
        {
            _swipeRefreshLayout.Post(() =>
            {
                _swipeRefreshLayout.Refreshing = true;
            });
            pageIndex = 1;
            var result = new ApiResult<List<QuestionModel>>();
            
            if (isMy)
            {
                //var user = UserInfoShared.GetUserInfo(this.Activity);
                result = await QuestionService.ListQuestion(UserTokenUtil.GetToken(this.Activity), position, pageIndex, true, userInfo.SpaceUserId);
            }
            else
            {
                result = await QuestionService.ListQuestion(AccessTokenUtil.GetToken(this.Activity), position, pageIndex, false, 0);
            }
            if (result.Success)
            {
                listQuestion = result.Data;
                if (!isFirstRefresh)
                {
                    adapter.SetNewData(listQuestion);
                }
                else
                {
                    initRecycler();
                    try
                    {
                        await SQLiteUtil.UpdateListQuestion(listQuestion, isMy);
                    }
                    catch (Exception ex)
                    {
                        //todo 
                        //AlertUtil.ToastShort(Activity,ex.ToString());
                    }
                }
                isFirstRefresh = false;
                _swipeRefreshLayout.Post(() =>
                {
                    _swipeRefreshLayout.Refreshing = false;
                });
            }
            else
            {
                AlertUtil.ToastShort(Activity,result.Message);
                _swipeRefreshLayout.Post(() =>
                {
                    _swipeRefreshLayout.Refreshing = false;
                });
            }
        }
    }
}