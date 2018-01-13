using Android;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Com.Nostra13.Universalimageloader.Core;
using Com.Umeng.Socialize;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Com.Umeng;
using Com.Umeng.Analytics;

namespace Cnblogs.XamarinAndroid
{
    [Application]
    public class InitApp : Application
    {
        public InitApp() { }
        public InitApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
        private List<Activity> activities;
        public override void OnCreate()
        {
            base.OnCreate();
            ImageLoaderConfiguration configuration = new ImageLoaderConfiguration.Builder(this).WriteDebugLogs().Build();
            ImageLoader.Instance.Init(configuration);//初始化图片加载框架
            Config.Debug =true;
            SQLiteUtil.Instance();
            MobclickAgent.SetDebugMode(true);//开启调试模式，如果不开启则不会上传umeng统计
            MobclickAgent.OpenActivityDurationTrack(false);
            MobclickAgent.SetScenarioType(this,MobclickAgent.EScenarioType.EUmNormal);
            PlatformConfig.SetWeixin("wx633888643fbae319", "5034ad765b2ba64dec7ed7c6618581fb");
            PlatformConfig.SetSinaWeibo("1422675167", "02975c36afd93d3ae983f8da9e596b86", "https://api.weibo.com/oauth2/default.html");
            PlatformConfig.SetQQZone("100424468", "c7394704798a158208a74ab60104f0ba");
            if (!BuildConfig.Debug)
            {
                AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;
            }
            activities = new List<Activity>();
        }
        internal void addActivity(Activity activity)
        {
            if (!activities.Contains(activity))
            {
                activities.Add(activity);
            }
        }
        internal void removeAllActivity()
        {
            activities.ForEach(activity => { activity.Finish(); });
            System.Diagnostics.Debug.Write("finsh all activity success");
        }

        protected override void Dispose(bool disposing)
        {
            AndroidEnvironment.UnhandledExceptionRaiser -= AndroidEnvironment_UnhandledExceptionRaiser;
            base.Dispose(disposing);
        }
        async void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs ex)
        {
            await Task.Run(() =>
            {
                Looper.Prepare();
                Toast.MakeText(this, Resources.GetString(Resource.String.search), ToastLength.Long).Show();
                Looper.Loop();
            });
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("生产厂商\n");
            sb.Append(Build.Manufacturer).Append("\n\n");
            sb.Append("手机型号\n");
            sb.Append(Build.Model).Append("\n\n");
            sb.Append("系统版本\n");
            sb.Append(Build.VERSION.Release).Append("\n\n");
            sb.Append("异常时间\n");
            sb.Append(DateTime.Now.ToString()).Append("\n\n");
            sb.Append("异常信息\n");
            sb.Append(ex.Exception).Append("\n");
            sb.Append(ex.Exception.Message).Append("\n");
            sb.Append(ex.Exception.StackTrace).Append("\n\n");
            MobclickAgent.ReportError(this, sb.ToString());

            System.Threading.Thread.Sleep(2000);
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            Java.Lang.JavaSystem.Exit(1);
        }
    }
}