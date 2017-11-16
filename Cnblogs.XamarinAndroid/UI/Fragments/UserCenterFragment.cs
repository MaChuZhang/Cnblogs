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
using Cnblogs.HttpClient;
using Cnblogs.ApiModel;
using Com.Nostra13.Universalimageloader.Core;
using Android.Graphics;

namespace Cnblogs.XamarinAndroid
{
    public class UserCenterFragment : Fragment
    {
        private ImageView iv_userAvatar;
        private LinearLayout ly_user, ly_startLogin, ly_myBlog;
        private TextView tv_login, tv_seniority,tv_userName, tv_subTitle, tv_postCount, tv_myStatus,tv_myQuestion;
        private DisplayImageOptions options;
        private UserInfo  userInfo;
        private UserBlog userBlog;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu=true;
            options = new DisplayImageOptions.Builder()
            .ShowImageOnFail(Resource.Drawable.Icon)
            .CacheInMemory(true)
            .BitmapConfig(Bitmap.Config.Rgb565)
            .ShowImageOnFail(Resource.Drawable.icon_user)
            .ShowImageOnLoading(Resource.Drawable.icon_user)
            .CacheOnDisk(true)
            .Displayer(new DisplayerImageCircle(20))
            .Build();
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
           base.OnCreateView(inflater, container, savedInstanceState);
           return   inflater.Inflate(Resource.Layout.UserCenter, container, false);
        }
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            //MenuInflater.Inflate.inflater(Resource.Menu.setting, menu);
            Activity.MenuInflater.Inflate(Resource.Menu.setting,menu);
            //return base.OnCreateOptionsMenu(menu);
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            ly_user = Activity.FindViewById<LinearLayout>(Resource.Id.ly_user);
            tv_login = Activity.FindViewById<TextView>(Resource.Id.tv_login);
            tv_seniority= Activity.FindViewById<TextView>(Resource.Id.tv_seniority);
            ly_startLogin = Activity.FindViewById<LinearLayout>(Resource.Id.ly_startLogin);
            tv_subTitle = Activity.FindViewById<TextView>(Resource.Id.tv_subTitle);
            tv_postCount = Activity.FindViewById<TextView>(Resource.Id.tv_postCount);
            ly_myBlog = Activity.FindViewById<LinearLayout>(Resource.Id.ly_myBlog);
            tv_myStatus = Activity.FindViewById<TextView>(Resource.Id.tv_myStatus);
            tv_myQuestion = Activity.FindViewById<TextView>(Resource.Id.tv_myQuestion);

            ly_startLogin.Click += (s, e) =>
            {
                StartActivity(new Intent(Activity,typeof(loginactivity)));
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
                        UserInfoShared.SetUserInfo(userInfo,Activity);
                        InitViewUserInfo(userInfo);
                    }
                }
                if (userBlog.BlogId != 0)
                {
                    InitViewUserBlog(userBlog);
                }
                else
                {
                    var result = await UserRequest.UserBlog(userToken,userInfo.BlogApp);
                    if (result.Success)
                    {
                        userBlog = result.Data;
                        UserBlogShared.SetUserBlog(userBlog,Activity);
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
                MyBlogActivity.Enter(userInfo.BlogApp,Activity);
            };
            tv_myStatus.Click += (s, e) =>
            {
                MyStatusActivity.Enter(userInfo.BlogApp, Activity);
            };
            tv_myQuestion.Click += (s, e) =>
            {
                MyQuestionActivity.Enter(userInfo.BlogApp,Activity);
            };
            ImageLoader.Instance.DisplayImage(userInfo.Avatar, iv_userAvatar, options);
        }

        void InitViewUserBlog(UserBlog  userBlog)
        {
            tv_subTitle.Text = userBlog.SubTitle.ToDBC();
            tv_postCount.Text = userBlog.PostCount.ToString();

      
            System.Diagnostics.Debug.Write(userBlog.Title);
        }
    }
}