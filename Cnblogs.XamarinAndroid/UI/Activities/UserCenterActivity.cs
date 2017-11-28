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
using Android.Support.V4.App;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "我的博客园", Theme = "@style/AppTheme")]
    public class UserCenterActivity : BaseActivity
    {

        protected override int LayoutResourceId
        {
            get
            {
                return Resource.Layout.UserCenter;
            }
        }
        protected override string ToolBarTitle
        {
            get
            {
                return  "我的博客园";
            }
        }

        private ImageView iv_userAvatar;
        private LinearLayout ly_user, ly_startLogin, ly_myBlog;
        private TextView tv_login, tv_seniority, tv_userName, tv_subTitle, tv_postCount, tv_myStatus, tv_myQuestion, tv_myBookmark;
        private DisplayImageOptions options;
        private UserInfo userInfo;
        private UserBlog userBlog;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetToolBarNavBack();
            StatusBarUtil.SetColorStatusBars(this);
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
            ly_user = FindViewById<LinearLayout>(Resource.Id.ly_user);
            tv_login = FindViewById<TextView>(Resource.Id.tv_login);
            tv_seniority= FindViewById<TextView>(Resource.Id.tv_seniority);
            ly_startLogin = FindViewById<LinearLayout>(Resource.Id.ly_startLogin);
            tv_subTitle = FindViewById<TextView>(Resource.Id.tv_subTitle);
            tv_postCount = FindViewById<TextView>(Resource.Id.tv_postCount);
            ly_myBlog =FindViewById<LinearLayout>(Resource.Id.ly_myBlog);
            tv_myStatus = FindViewById<TextView>(Resource.Id.tv_myStatus);
            tv_myQuestion = FindViewById<TextView>(Resource.Id.tv_myQuestion);
            tv_myBookmark = FindViewById<TextView>(Resource.Id.tv_myBookmark);
            tv_myBookmark.Click += (s, e) =>
            {
                StartActivity(new Intent(this,typeof(MyBookmarkActivity)));
            };
            ly_startLogin.Click += (s, e) =>
            {
                StartActivity(new Intent(this, typeof(loginactivity)));
            };

            tv_userName = FindViewById<TextView>(Resource.Id.tv_userName);
            iv_userAvatar = FindViewById<ImageView>(Resource.Id.iv_userAvatar);
            Token userToken = UserTokenUtil.GetToken(this);
            userToken.RefreshTime = DateTime.Now;
            //用户token 未过期
            if (!string.IsNullOrEmpty(userToken.access_token) && !userToken.IsExpire)
            {
                tv_login.Visibility = ViewStates.Gone;
                ly_user.Visibility = ViewStates.Visible;
                //ly_startLogin.Clickable = false;
                //本地读取登录用户信息，用户博客信息
                userInfo = UserInfoShared.GetUserInfo(this);
                userBlog = UserBlogShared.GetUserBlog(this);
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
                        UserInfoShared.SetUserInfo(userInfo, this);
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
                        UserBlogShared.SetUserBlog(userBlog, this);
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
        MyBlogActivity.Enter(userInfo.BlogApp, this);
    };
    tv_myStatus.Click += (s, e) =>
    {
        MyStatusActivity.Enter(userInfo.BlogApp, this);
    };
    tv_myQuestion.Click += (s, e) =>
    {
        MyQuestionActivity.Enter(userInfo.BlogApp, this);
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