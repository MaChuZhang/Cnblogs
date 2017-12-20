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
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Webkit;
using Android.Support.V4.App;
using Com.Nostra13.Universalimageloader.Core;
using Android.Graphics;
using Cnblogs.XamarinAndroid;
using Cnblogs.ApiModel;
using Com.Umeng.Socialize;
using Cnblogs.XamarinAndroid.UI.Widgets;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using Cnblogs.XamarinAndroid.UI;
using Cnblogs.HttpClient;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "questionesCommentActivity", Theme = "@style/AppTheme")]
    public class QuestionActivity : BaseActivity,View.IOnClickListener
    {
        //private Toolbar toolbar;
        private TextView tv_userName, tv_dateAdded, tv_content, tv_title, tv_qScore, tv_dealFlag,tv_tags,tv_viewCount, tv_award;
        private ImageView iv_userIcon;
        private Button btn_mark, btn_diggCount, btn_answerCount;
        private int questionId;
        private  WebView wb_content;
        DisplayImageOptions options;

        private QuestionModel question;
        private UMengShareWidget shareWidget;
        protected override int LayoutResourceId => Resource.Layout.questionDetail;

        protected override string ToolBarTitle => "博问详情";

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            StatusBarUtil.SetColorStatusBars(this);
            ImageLoaderConfiguration configuration = new ImageLoaderConfiguration.Builder(this).WriteDebugLogs().Build();//初始化图片加载框架
            ImageLoader.Instance.Init(configuration);
            //显示图片配置
            options = new DisplayImageOptions.Builder()
                  .ShowImageForEmptyUri(Resource.Drawable.Icon)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .ShowImageOnFail(Resource.Drawable.icon_user)
                  .ShowImageOnLoading(Resource.Drawable.icon_loading)
                  .CacheOnDisk(true)
                  .Displayer(new DisplayerImageCircle(20))
                  .Build();
            SetToolBarNavBack();
            tv_title = FindViewById<TextView>(Resource.Id.tv_title);
            tv_award = FindViewById<TextView>(Resource.Id.tv_award);
            tv_dateAdded = FindViewById<TextView>(Resource.Id.tv_dateAdded);
            tv_userName = FindViewById<TextView>(Resource.Id.tv_userName);
            iv_userIcon = FindViewById<ImageView>(Resource.Id.iv_userIcon);
             tv_tags = FindViewById<TextView>(Resource.Id.tv_tags);
            btn_diggCount = FindViewById<Button>(Resource.Id.btn_digg);
            tv_viewCount = FindViewById<TextView>(Resource.Id.tv_view);
            tv_qScore = FindViewById<TextView>(Resource.Id.tv_qScore);
            btn_answerCount = FindViewById<Button>(Resource.Id.btn_comment);
            btn_mark = FindViewById<Button>(Resource.Id.btn_mark);

            tv_dealFlag = FindViewById<TextView>(Resource.Id.tv_dealFlag);
            wb_content = FindViewById<WebView>(Resource.Id.wb_content);

            questionId = Intent.GetIntExtra("id", 0);
            GetClientQuestion(questionId);
            GetServerQuestion();
            shareWidget = new UMengShareWidget(this);
            btn_answerCount.Click += (s, e) =>
            {
                QuestionAnswerActivity.Enter(this, questionId);
            };
            //_swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            //_swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            //_swipeRefreshLayout.SetOnRefreshListener(this);
            //_swipeRefreshLayout.Post(() =>
            //{
            //    _swipeRefreshLayout.Refreshing = true;

            //});

            wb_content.Settings.DomStorageEnabled = true;
            wb_content.Settings.JavaScriptEnabled = true;//支持js
            wb_content.Settings.DefaultTextEncodingName = "utf-8";//设置编码方式utf-8
            wb_content.Settings.SetSupportZoom(false);//不可缩放
            wb_content.Settings.DisplayZoomControls = false;//隐藏原生的缩放控件
            wb_content.Settings.BuiltInZoomControls = false;//设置内置的缩放控件
            wb_content.Settings.CacheMode = CacheModes.CacheElseNetwork;
            wb_content.ScrollBarStyle = ScrollbarStyles.InsideOverlay;
            wb_content.Settings.LoadsImagesAutomatically = true;//支持自动加载图片
            wb_content.Settings.UseWideViewPort = true; //将图片调整到合适webview的大小
            wb_content.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
            var jsInterface = new WebViewJSInterface(this);
            wb_content.SetWebViewClient(ContentWebViewClient.Instance(this));
            wb_content.AddJavascriptInterface(jsInterface, "openlistner");
            jsInterface.CallFromPageReceived += delegate (object sender, WebViewJSInterface.CallFromPageReceivedEventArgs e)
            {
                PhotoActivity.Enter(this, e.Result.Split(','), e.Index);
            };
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.share,menu);
            return  true;
        }
        internal static void Enter(Context context,int id)
        {
            Intent intent = new Intent(context,typeof(QuestionActivity));
            intent.PutExtra("id",id);
            context.StartActivity(intent);
        }
        internal static void Enter(Context context, int id,bool isSearch)
        {
            Intent intent = new Intent(context, typeof(QuestionActivity));
            intent.PutExtra("id", id);
            intent.PutExtra("issearch",isSearch);
            context.StartActivity(intent);
        }
        async void GetClientQuestion(int id)
        {
            try
            {
                bool isSearch = Intent.GetBooleanExtra("issearch", false);
                if(!isSearch)
                {
                    question = await SQLiteUtil.SelectQuestion(id);
                }
                BindView();
            }
            catch (Exception ex)
            {

            }
        }
        void BindView()
        {
            if (question != null)
            {
                if (question.QuestionUserInfo != null)
                {
                    tv_userName.Text = question.QuestionUserInfo.UserName;
                    ImageLoader.Instance.DisplayImage(Constact.CnblogsPic + question.QuestionUserInfo.IconName, iv_userIcon, options);
                    tv_qScore.Text = question.QuestionUserInfo.QScore.ToString()+"园豆";
                }
                if (question.Award == 0)
                {
                    View awardParent = (View)tv_award.Parent;
                       awardParent.Visibility = ViewStates.Gone;
                }
                else
                {
                    View awardParent = (View)tv_award.Parent;
                    awardParent.Visibility = ViewStates.Visible;
                    tv_award.Text = "奖励" + question.Award.ToString();
                }
                if (!string.IsNullOrEmpty(question.Tags))
                {
                    tv_tags.Visibility = ViewStates.Visible;
                    tv_tags.Text = question.Tags.Replace(',', ' ');
                }
                else
                {
                    tv_tags.Visibility = ViewStates.Gone;
                }
                if (question.DealFlag == 1)
                {
                    this.tv_dealFlag.Text = Resources.GetString(Resource.String.question_dealflag_1);
                    this.tv_dealFlag.Selected = false;
                }
                else if (question.DealFlag == -1)
                {
                    this.tv_dealFlag.Text = Resources.GetString(Resource.String.question_dealflag_2);
                    this.tv_dealFlag.Selected = true;
                }
                else
                {
                    this.tv_dealFlag.Text = Resources.GetString(Resource.String.question_dealflag_0);
                    this.tv_dealFlag.Selected = true;
                }
                if (!string.IsNullOrEmpty(question.Content))
                {
                    var content = HtmlUtil.ReadHtml(Assets);
                    var body = HtmlUtil.ReplaceHtml(question.Content).Trim('"');
                    if (question.Addition != null)
                    {
                        body += " <h2>问题补充：</h2>" + question.Addition.Content;
                    }
                    wb_content.LoadDataWithBaseURL("file:///android_asset/", content.Replace("#title#", "").Replace("#body#", body).Replace("#author#","").Replace("#date#",""), "text/html", "utf-8", null);
                }

                tv_dateAdded.Text = question.DateAdded.ToCommonString();
                btn_diggCount.Text = question.DiggCount.ToString();
                btn_answerCount.Text = question.AnswerCount.ToString();
                tv_viewCount.Text = question.ViewCount.ToString();
                tv_title.Text = question.Title;
                //tv_content.Text = question.Content;
            }
        }

        async void GetServerQuestion()
        {
            try
            {
                var  result = await QuestionService.GetQuestionDetail(AccessTokenUtil.GetToken(this), questionId);
                if (result.Success)
                {
                    question = result.Data;
                    BindView();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public override bool OnMenuItemClick(IMenuItem item)
        {
            if (question != null)
            {
                // sharesWidget
                //shareWidget.Open(question.Url,question.Title);
            }
            return true;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode,resultCode,data);
            UMShareAPI.Get(this).OnActivityResult(requestCode,(int)resultCode,data);
        }
        public void OnClick(View v)
        {
            ActivityCompat.FinishAfterTransition(this);
        }
    }
}