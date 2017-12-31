using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Nostra13.Universalimageloader.Core;
using Cnblogs.ApiModel;
using Android.Graphics;
using Cnblogs.HttpClient;
using Android.Support.V4.Widget;
using System.Collections.Generic;

namespace Cnblogs.XamarinAndroid
{
    public class UserCenterFragment: Android.Support.V4.App.Fragment,SwipeRefreshLayout.IOnRefreshListener
    {

        private ImageView iv_userAvatar;
        private LinearLayout  ly_unLogin,ly_logged, ll_question, ll_status, ll_blog;
        private TextView tv_seniority, tv_userName, tv_subTitle, tv_postCount, tv_myBookmark, tv_about;
        private DisplayImageOptions options;
        private UserInfo userInfo;
        private UserBlog userBlog;
        private Token userToken;
        private SwipeRefreshLayout swipeRefreshLayout;
        private bool unLoginClickRegistered;
        private bool loggedClickRegistered;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
        }
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.setting,menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.UserCenter, container, false);
        }
      
        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            //SetToolBarTitle(Resources.GetString(Resource.String.myBlog));
            //��ʾͼƬ����
            options = new DisplayImageOptions.Builder()
           .ShowImageForEmptyUri(Resource.Drawable.icon_yuanyou)
             .ShowImageOnFail(Resource.Drawable.icon_yuanyou)
             .ShowImageOnLoading(Resource.Drawable.icon_user)
             .CacheInMemory(true)
             .BitmapConfig(Bitmap.Config.Rgb565)
             .CacheOnDisk(true)
            // .Displayer(new DisplayerImageCircle(20))
             .Build();
            swipeRefreshLayout = View.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeColors(Resources.GetColor(Resource.Color.primary)) ;
            swipeRefreshLayout.SetOnRefreshListener(this);
            tv_seniority= View.FindViewById<TextView>(Resource.Id.tv_seniority);
            ly_unLogin = View.FindViewById<LinearLayout>(Resource.Id.ly_unLogin);
            ly_logged = View.FindViewById<LinearLayout>(Resource.Id.ly_logged);
            tv_subTitle = View.FindViewById<TextView>(Resource.Id.tv_subTitle);
            tv_postCount = View.FindViewById<TextView>(Resource.Id.tv_postCount);
            ll_blog = View.FindViewById<LinearLayout>(Resource.Id.ll_blog);
            ll_status = View.FindViewById<LinearLayout>(Resource.Id.ll_status);
            ll_question = View.FindViewById<LinearLayout>(Resource.Id.ll_question);
            tv_myBookmark = View.FindViewById<TextView>(Resource.Id.tv_myBookmark);
            tv_about = view.FindViewById<TextView>(Resource.Id.tv_about);
            tv_about.Click += (s, e) =>
            {
                AboutActivity.Enter(Activity);
            };
            ly_unLogin.Click += (s, e) =>
            {
                StartActivity(new Intent(Activity, typeof(loginactivity)));
            };

            tv_userName = View.FindViewById<TextView>(Resource.Id.tv_userName);
            iv_userAvatar = View.FindViewById<ImageView>(Resource.Id.iv_userAvatar);
             userToken = UserTokenUtil.GetToken(Activity);
             OnRefresh(); 
            //�û�token δ����
        }
        public void OnRefresh()
        {
            swipeRefreshLayout.Post(() =>
            {
                swipeRefreshLayout.Refreshing = true;
            });
            UpdateViewStatus(()=> {
                swipeRefreshLayout.PostDelayed(() =>
                {
                    swipeRefreshLayout.Refreshing =false;
                },1000);
            },()=> {
                swipeRefreshLayout.PostDelayed(() =>
                {
                    swipeRefreshLayout.Refreshing = false;
                },1000);
            });
        }
        async  void UpdateViewStatus(Action callBack,Action callBackError)
        {
            
            if (!UserUtil.Instance(Activity).LoginExpire())
            {
                ly_unLogin.Visibility = ViewStates.Gone;
                ly_logged.Visibility = ViewStates.Visible;
                
                userInfo = UserInfoShared.GetUserInfo(Activity);
                userBlog = UserBlogShared.GetUserBlog(Activity);

                if (userInfo.SpaceUserId == 0)
                {
                    var result = await UserInfoService.GetUser(userToken);
                    if (result.Success)
                    {
                        userInfo = result.Data;
                        UserInfoShared.SetUserInfo(userInfo, Activity);
                        callBack();
                    }
                    else
                    {
                        callBackError();
                    }
                }
                if (userBlog.BlogId == 0)
                {
                    var result = await UserInfoService.GetUserBlog(userToken, userInfo.BlogApp);
                    if (result.Success)
                    {
                        userBlog = result.Data;
                        UserBlogShared.SetUserBlog(userBlog, Activity);
                        tv_subTitle.Text = userBlog.SubTitle.ToDBC();
                        tv_postCount.Text = userBlog.PostCount.ToString();
                        callBack();
                    }
                    else
                    {
                        callBackError();
                    }
                }
                //�û���Ϣ
                tv_userName.Text = userInfo.DisplayName;
                tv_seniority.Text = "԰��:" + userInfo.Seniority + "����:" + userInfo.Score;
                ImageLoader.Instance.DisplayImage(userInfo.Avatar, iv_userAvatar, options);
                //�û�������Ϣ
                tv_subTitle.Text = userBlog.SubTitle.ToDBC();
                tv_postCount.Text = userBlog.PostCount.ToString();
                ll_blog.Click -= UnLoginClick;
                ll_status.Click -= UnLoginClick;
                ll_question.Click -= UnLoginClick;
                tv_myBookmark.Click -= UnLoginClick;
                if (!loggedClickRegistered)
                {
                    ll_blog.Click += MyBlogClick;
                    ll_status.Click += MyStatusClick;
                    ll_question.Click += MyQuestionClick;
                    tv_myBookmark.Click += MyBookMarkClick;
                }
                loggedClickRegistered = true;
                callBack();
            }
            else
            {
                ImageLoader.Instance.DisplayImage("drawable://" + Resource.Drawable.icon_userDefault, iv_userAvatar, options);
                ly_unLogin.Visibility = ViewStates.Visible;
                ly_logged.Visibility = ViewStates.Gone;//�û�layout����ʾ
                tv_postCount.Text = "0";
                
                ll_blog.Click -= MyBlogClick;
                ll_status.Click -= MyStatusClick;
                ll_question.Click -= MyQuestionClick;
                tv_myBookmark.Click -=MyBookMarkClick;
                if (!unLoginClickRegistered)
                {
                    ll_blog.Click += UnLoginClick;
                    ll_status.Click += UnLoginClick;
                    ll_question.Click += UnLoginClick;
                    tv_myBookmark.Click += UnLoginClick;
                }
                unLoginClickRegistered = true;
                callBack();
            }
        }
        void MyBlogClick(object o ,EventArgs e)
        {
            MyBlogActivity.Enter(userInfo.BlogApp, Activity);
        }
        void MyBookMarkClick(object o, EventArgs e)
        {
            StartActivity(new Intent(Activity, typeof(MyBookmarkActivity)));
        }
        void MyStatusClick(object o, EventArgs e)
        {
            MyStatusActivity.Enter(userInfo.BlogApp, Activity);
        }
        void MyQuestionClick(object o,EventArgs e)
        {
            MyQuestionActivity.Enter(userInfo.BlogApp, Activity);
        }
        void UnLoginClick(object o, EventArgs e)
        {
            AlertUtil.ToastShort(Activity, "����ʱ��û�е�¼,����µ�¼��Ȩ");
        }
        void InitViewUserInfo(UserInfo userInfo)
      {
            if (!UserUtil.Instance(Activity).LoginExpire())
            {
                tv_userName.Text = userInfo.DisplayName;
                tv_seniority.Text = "԰��:" + userInfo.Seniority + "����:" + userInfo.Score;
                ImageLoader.Instance.DisplayImage(userInfo.Avatar, iv_userAvatar, options);
            }
            else
            {
                ImageLoader.Instance.DisplayImage("drawable://"+Resource.Drawable.icon_userDefault, iv_userAvatar, options);
            }
}




    }
}