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

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "Cnblogs.XamarinAndroid", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/AppTheme")]
    public class MainActivity : BaseActivity,Toolbar.IOnMenuItemClickListener
    {
        private DateTime ? firstBackTime; //第一次单击返回
        //private Toolbar _toolbar;

        private UserCenterFragment _userCenterFragment;
        private HomeFragment _homeFragment;
        private StatusQuestionFragment _statusQuestionFragment;
        private StatusFragment _statusFragment;
        private QuestionFragment _questionFragment;
        private KbArticlesFragment _kbArticlesFragment;
        private FragmentManager _fm;
        private TextView tv_news,tv_kbArticles,tv_userCenter,tv_status;
        private RecyclerView _recyclerView;
        private Button btn_status;
        private Button btn_question;
        protected override int LayoutResourceId => Resource.Layout.Main;
        protected override string ToolBarTitle =>Resources.GetString(Resource.String.ToolBar_Title_Cnblogs);

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ImageLoaderConfiguration configuration = new ImageLoaderConfiguration.Builder(this).WriteDebugLogs().Build();//初始化图片加载框架
            ImageLoader.Instance.Init(configuration);
            StatusBarUtil.SetColorStatusBars(this);
            _fm = SupportFragmentManager;
            //SetSupportActionBar(_toolbar);
            tv_news = FindViewById<TextView>(Resource.Id.tv_news);
            tv_userCenter = FindViewById<TextView>(Resource.Id.tv_userCenter);
            tv_status = FindViewById<TextView>(Resource.Id.tv_status);
            tv_kbArticles = FindViewById<TextView>(Resource.Id.tv_kbArticles);
            BindViewsClick();
            tv_news.PerformClick();
        }
        void SetUnSelected()
        {
            tv_news.Selected = false;
            tv_kbArticles.Selected = false;
            tv_userCenter.Selected = false;
            tv_status.Selected = false;
        }
        //隐藏所有Fragment
        void hideAllFragment(FragmentTransaction fragmentTransaction)
        {
            if (_homeFragment != null) fragmentTransaction.Hide(_homeFragment);
            if (_kbArticlesFragment != null) fragmentTransaction.Hide(_kbArticlesFragment);
            if (_userCenterFragment != null) fragmentTransaction.Hide(_userCenterFragment);
            if (_statusQuestionFragment != null) fragmentTransaction.Hide(_statusQuestionFragment);
            if (StatusQuestionFragment._questionFragment != null) fragmentTransaction.Hide(StatusQuestionFragment._questionFragment);
            if (StatusQuestionFragment._statusFragment != null) fragmentTransaction.Hide(StatusQuestionFragment._statusFragment);
        }
        void BindViewsClick()
        {
            tv_userCenter.Click += (s, e) => { MenuSwitch(tv_userCenter); };
            tv_status.Click += (s, e) => { MenuSwitch(tv_status); };
            tv_kbArticles.Click += (s, e) => { MenuSwitch(tv_kbArticles); };
            tv_news.Click += (s, e) => { MenuSwitch(tv_news); };
        }
        void MenuSwitch(View v)
        {
            FragmentTransaction ft = _fm.BeginTransaction();
            hideAllFragment(ft);
            switch (v.Id)
            {
                case Resource.Id.tv_news:
                    SetUnSelected();
                    tv_news.Selected = true;
                    if (_homeFragment == null)
                    {
                        _homeFragment = new HomeFragment();
                        ft.Add(Resource.Id.frameContent, _homeFragment);
                    }
                    else ft.Show(_homeFragment);
                    base.SetToolBarTitle(Resources.GetString(Resource.String.CnblogsTitle));
                    SetTabVisible(false);
                    break;

                case Resource.Id.tv_kbArticles:
                    SetUnSelected();
                    tv_kbArticles.Selected = true;
                    if (_kbArticlesFragment == null)
                    {
                        _kbArticlesFragment = new KbArticlesFragment();
                        ft.Add(Resource.Id.frameContent, _kbArticlesFragment);
                    }
                    else
                    {
                        ft.Show(_kbArticlesFragment);
                    }
                    base.SetToolBarTitle(Resources.GetString(Resource.String.kbArticles));
                    SetTabVisible(false);
                    break;
                case Resource.Id.tv_status:
                    SetUnSelected();
                    tv_status.Selected = true;
                    if (_statusQuestionFragment == null)
                    {
                        _statusQuestionFragment = new StatusQuestionFragment();
                        ft.Add(Resource.Id.frameContent, _statusQuestionFragment);
                    }
                    else
                    {
                        ft.Show(_statusQuestionFragment);
                        ft.Show(StatusQuestionFragment._statusFragment);
                    }
                    base.SetToolBarTitle("");
                    SetTabVisible(true);
                    break;
                case Resource.Id.tv_userCenter:
                    SetUnSelected();
                    tv_userCenter.Selected = true;
                    if (_userCenterFragment == null)
                    {
                        _userCenterFragment = new UserCenterFragment();
                        ft.Add(Resource.Id.frameContent, _userCenterFragment);
                    }
                    else
                        ft.Show(_userCenterFragment);
                    base.SetToolBarTitle(Resources.GetString(Resource.String.CnblogsTitle));
                    SetTabVisible(false);
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
                    this.Finish();
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

        public override bool OnMenuItemClick(IMenuItem item)
        {
            //throw new NotImplementedException();
            return true;
        }
        #endregion
    }
}

