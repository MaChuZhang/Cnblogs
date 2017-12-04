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

namespace Cnblogs.XamarinAndroid
{
    public class UserCenterFragment: Android.Support.V4.App.Fragment
    {

        private ImageView iv_userAvatar;
        private LinearLayout ly_user, ly_startLogin, ly_myBlog;
        private TextView tv_login, tv_seniority, tv_userName, tv_subTitle, tv_postCount, tv_myStatus, tv_myQuestion, tv_myBookmark;
        private DisplayImageOptions options;
        private UserInfo userInfo;
        private UserBlog userBlog;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.UserCenter, container, false);
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            //SetToolBarTitle(Resources.GetString(Resource.String.myBlog));
            //显示图片配置
            options = new DisplayImageOptions.Builder()
                .ShowImageForEmptyUri(Resource.Drawable.Icon)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .ShowImageOnFail(Resource.Drawable.icon_user)
                  .ShowImageOnLoading(Resource.Drawable.icon_user)
                  .CacheOnDisk(true)
                  .Displayer(new DisplayerImageCircle(20))
                  .Build();
            ly_user = Activity.FindViewById<LinearLayout>(Resource.Id.ly_user);
            tv_login = Activity.FindViewById<TextView>(Resource.Id.tv_login);
            tv_seniority= Activity.FindViewById<TextView>(Resource.Id.tv_seniority);
            ly_startLogin = Activity.FindViewById<LinearLayout>(Resource.Id.ly_startLogin);
            tv_subTitle = Activity.FindViewById<TextView>(Resource.Id.tv_subTitle);
            tv_postCount = Activity.FindViewById<TextView>(Resource.Id.tv_postCount);
            ly_myBlog =Activity.FindViewById<LinearLayout>(Resource.Id.ly_myBlog);
            tv_myStatus = Activity.FindViewById<TextView>(Resource.Id.tv_myStatus);
            tv_myQuestion = Activity.FindViewById<TextView>(Resource.Id.tv_myQuestion);
            tv_myBookmark = Activity.FindViewById<TextView>(Resource.Id.tv_myBookmark);
            tv_myBookmark.Click += (s, e) =>
            {
                StartActivity(new Intent(Activity,typeof(MyBookmarkActivity)));
            };
            ly_startLogin.Click += (s, e) =>
            {
                StartActivity(new Intent(Activity, typeof(loginactivity)));
            };

            tv_userName = Activity.FindViewById<TextView>(Resource.Id.tv_userName);
            iv_userAvatar = Activity.FindViewById<ImageView>(Resource.Id.iv_userAvatar);
            Token userToken = UserTokenUtil.GetToken(Activity);
            userToken.RefreshTime = DateTime.Now;
            //用户token 未过期
            if (!string.IsNullOrEmpty(userToken.access_token) && !userToken.IsExpire)
            {
                tv_login.Visibility = ViewStates.Gone;
                ly_user.Visibility = ViewStates.Visible;
                //ly_startLogin.Clickable = false;
                //本地读取登录用户信息，用户博客信息
                userInfo = UserInfoShared.GetUserInfo(Activity);
                userBlog = UserBlogShared.GetUserBlog(Activity);
                if (userInfo.BlogId != 0)
                {
                   InitViewUserInfo(userInfo);
                }
                else
                {
                    var result = await UserRequest.UserInfo(userToken);
                    if (result.Success)
                    {
                        userInfo = result.Data;
                        UserInfoShared.SetUserInfo(userInfo, Activity);
                        InitViewUserInfo(userInfo);
                    }
                }
                if (userBlog.BlogId != 0)
                {
                   InitViewUserBlog(userBlog);
                }
                else
                {
                    var result = await UserRequest.UserBlog(userToken, userInfo.BlogApp);
                    if (result.Success)
                    {
                        userBlog = result.Data;
                        UserBlogShared.SetUserBlog(userBlog, Activity);
                        InitViewUserBlog(userBlog);
                    }
                }
            }
            else {
                tv_login.Visibility = ViewStates.Visible;
                ly_user.Visibility = ViewStates.Gone;
                ly_startLogin.Clickable = true;
            }
        }
        void InitViewUserInfo(UserInfo userInfo)
      {
        tv_userName.Text = userInfo.DisplayName;
        tv_seniority.Text = "园龄:" + userInfo.Seniority + "积分:" + userInfo.Score;
    ly_myBlog.Click += (s, e) =>
    {
        MyBlogActivity.Enter(userInfo.BlogApp, Activity);
    };
    tv_myStatus.Click += (s, e) =>
    {
        MyStatusActivity.Enter(userInfo.BlogApp, Activity);
    };
    tv_myQuestion.Click += (s, e) =>
    {
        MyQuestionActivity.Enter(userInfo.BlogApp, Activity);
    };
    ImageLoader.Instance.DisplayImage(userInfo.Avatar, iv_userAvatar, options);
}

         void InitViewUserBlog(UserBlog userBlog)
{
    tv_subTitle.Text = userBlog.SubTitle.ToDBC();
    tv_postCount.Text = userBlog.PostCount.ToString();


    System.Diagnostics.Debug.Write(userBlog.Title);
}
    }
}