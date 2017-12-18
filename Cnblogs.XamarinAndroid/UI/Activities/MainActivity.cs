using Android.App;
using Android.Widget;
using Android.OS;
using Cnblogs.XamarinAndroid.UI.Activities;
using System;
using Android.Views;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Cnblogs.XamarinAndroid;

using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using Cnblogs.HttpClient;
using Com.Nostra13.Universalimageloader.Core;
using static Android.App.ActionBar;
using Android.Content;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "Cnblogs.XamarinAndroid", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/AppTheme")]
    public class MainActivity : BaseActivity,Toolbar.IOnMenuItemClickListener
    {
        private DateTime ? firstBackTime; //第一次单击返回
        //private Toolbar _toolbar;

        private NewsFragment newsFragment;
        private BlogFragment _homeFragment;
        private QuestionFragment questionFragment;
        private StatusFragment statusFragment;
        private UserCenterFragment userCenterFragment;
        private FragmentManager _fm;
        private TextView tv_blog,tv_userCenter,tv_news,tv_status,tv_question;
        protected override int LayoutResourceId => Resource.Layout.Main;
        protected override string ToolBarTitle =>Resources.GetString(Resource.String.ToolBar_Title_Cnblogs);

    

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            addActivity(this);
            ImageLoaderConfiguration configuration = new ImageLoaderConfiguration.Builder(this).WriteDebugLogs().Build();//初始化图片加载框架
            ImageLoader.Instance.Init(configuration);
            StatusBarUtil.SetColorStatusBars(this);
            _fm = SupportFragmentManager;
            //SetSupportActionBar(_toolbar);
            tv_blog = FindViewById<TextView>(Resource.Id.tv_blog);
            tv_news = FindViewById<TextView>(Resource.Id.tv_news);
            tv_status = FindViewById<TextView>(Resource.Id.tv_status);
            tv_question = FindViewById<TextView>(Resource.Id.tv_question);
            tv_userCenter = FindViewById<TextView>(Resource.Id.tv_userCenter);
            BindViewsClick();
            tv_blog.PerformClick();
        }
        internal static void Enter(Context context)
        {
            Intent intent = new Intent(context,typeof(MainActivity));
            context.StartActivity(intent);
        }
        public override bool OnMenuItemClick(IMenuItem menuItem)
        {
            if (menuItem.ItemId == Resource.Id.search)
            {
                string category = string.Empty;
                if (tv_blog.Selected)
                {
                    category = "Blog";
                }
                else if (tv_news.Selected)
                {
                    if (newsFragment.currentPosition == 0)
                        category = "Kb";
                    else category = "News";
                    System.Diagnostics.Debug.Write("position",newsFragment.currentPosition.ToString());
                }
                else if (tv_question.Selected)
                {
                    category = "Question";
                }
                else
                {
                    category = "Blog";
                }
                SearchResultActivity.Enter(category, this);
            }
            if (menuItem.ItemId == Resource.Id.add)
            {
                if (tv_status.Selected)
                {
                    AddStatusActivity.Enter(this);
                }
                else if (tv_question.Selected)
                {
                    AddQuestionActivity.Enter(this);
                }
                AlertUtil.ToastShort(this,"添加");
            }
            if (menuItem.ItemId == Resource.Id.setting)
            {
                if (tv_userCenter.Selected)
                {
                    AlertUtil.ToastShort(this, "设置");
                }
            }
            return true;
        }
        void SetUnSelected()
        {
            tv_blog.Selected = false;
            tv_userCenter.Selected = false;
            tv_news.Selected = false;
            tv_status.Selected = false;
            tv_question.Selected = false;
        }
        //隐藏所有Fragment
        void hideAllFragment(FragmentTransaction fragmentTransaction)
        {
            if (_homeFragment != null) fragmentTransaction.Hide(_homeFragment);
            if (userCenterFragment != null) fragmentTransaction.Hide(userCenterFragment);
            if (newsFragment != null) fragmentTransaction.Hide(newsFragment);
            if (statusFragment != null) fragmentTransaction.Hide(statusFragment);
            if (questionFragment != null) fragmentTransaction.Hide(questionFragment);
            //if (StatusQuestionFragment._questionFragment != null) fragmentTransaction.Hide(StatusQuestionFragment._questionFragment);
            //if (StatusQuestionFragment._statusFragment != null) fragmentTransaction.Hide(StatusQuestionFragment._statusFragment);
        }
        void BindViewsClick()
        {
            tv_news.Click += (s, e) => { MenuSwitch(tv_news); };
            tv_status.Click += (s, e) => { MenuSwitch(tv_status); };
            tv_userCenter.Click += (s, e) => { MenuSwitch(tv_userCenter); };
            tv_blog.Click += (s, e) => { MenuSwitch(tv_blog); };
            tv_question.Click += (s, e) => { MenuSwitch(tv_question); };
        }
        void MenuSwitch(View v)
        {
            FragmentTransaction ft = _fm.BeginTransaction();
            hideAllFragment(ft);
            switch (v.Id)
            {
                case Resource.Id.tv_blog:
                    SetUnSelected();
                    tv_blog.Selected = true;
                    if (_homeFragment == null)
                    {
                        _homeFragment = new BlogFragment();
                        ft.Add(Resource.Id.frameContent, _homeFragment);
                    }
                    else ft.Show(_homeFragment);
                    SetToolBarTitle(Resources.GetString(Resource.String.ToolBar_Title_Cnblogs));
                    break;

                case Resource.Id.tv_userCenter:
                    SetUnSelected();
                    tv_userCenter.Selected = true;
                    if (userCenterFragment == null)
                    {
                        userCenterFragment = new UserCenterFragment();
                        ft.Add(Resource.Id.frameContent, userCenterFragment);
                    }
                    else
                    {
                        ft.Show(userCenterFragment);
                    }
                    SetToolBarTitle("我的博客园");
                    break;
                case Resource.Id.tv_status:
                    SetUnSelected();
                    tv_status.Selected = true;
                    if (statusFragment == null)
                    {
                        statusFragment = new StatusFragment();
                        ft.Add(Resource.Id.frameContent,statusFragment);
                    }
                    else
                    {
                        ft.Show(statusFragment);
                    }
                    SetToolBarTitle(Resources.GetString(Resource.String.statuses));
                    break;

                case Resource.Id.tv_question:
                    SetUnSelected();
                    tv_question.Selected = true;
                    if (questionFragment == null)
                    {
                        questionFragment = new QuestionFragment();
                        ft.Add(Resource.Id.frameContent, questionFragment);
                    }
                    else
                    {
                        ft.Show(questionFragment);
                    }
                    SetToolBarTitle(Resources.GetString(Resource.String.question));
                    break;

                case Resource.Id.tv_news:
                    SetUnSelected();
                    tv_news.Selected = true;
                    if (newsFragment == null)
                    {
                        newsFragment = new NewsFragment();
                        ft.Add(Resource.Id.frameContent, newsFragment);
                    }
                    else
                        ft.Show(newsFragment);
                    SetToolBarTitle("");
                    SetToolBarTitle(Resources.GetString(Resource.String.news));
                    break;
            }
            ft.Commit();
        }
        #region override method
        public override bool OnKeyDown(Keycode keycode , KeyEvent e)
        {
            if (keycode == Keycode.Back && e.Action == KeyEventActions.Down)
            {
                if (!firstBackTime.HasValue || DateTime.Now.Second - firstBackTime.Value.Second > 2)
                {
                    AlertUtil.ToastShort(this, "再按一次退出");
                    firstBackTime = DateTime.Now;
                }
                else
                {
                    removeAllActivity();
                }
                return true;
            }
            return base.OnKeyDown(keycode,e);
        }
        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    MenuInflater.Inflate(Resource.Menu.search, menu);
        //    return base.OnCreateOptionsMenu(menu);
        //}
        #endregion
    }
}

