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
using Cnblogs.HttpClient;
using System.Threading;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "AddBookmarkActivity", Theme = "@style/AppTheme")]
    public class AddBookmarkActivity : BaseActivity
    {
        protected override int LayoutResourceId => Resource.Layout.addBookmark;

        protected override string ToolBarTitle => Resources.GetString(Resource.String.bookmark_add);
        private EditText edit_url, edit_title,edit_summary,edit_tags;
        private Button btn_submit;
        private TextView tv_startLogin;
        private LinearLayout ly_expire;
        private  string mode;
        private int wzLinkId;
        internal static void Enter(Context context, string url, string title,string mode)
        {
            Intent intent = new Intent(context, typeof(AddBookmarkActivity));
            intent.PutExtra("url", url);
            intent.PutExtra("title", title);
            intent.PutExtra("mode", mode);
            context.StartActivity(intent);
        }
        internal static void Enter(Context context,int id, string url, string title,string  tags,string summary, string mode)
        {
            Intent intent = new Intent(context, typeof(AddBookmarkActivity));
            //intent.put
            intent.PutExtra("id",id);
            intent.PutExtra("url", url);
            intent.PutExtra("title", title);
            intent.PutExtra("tags", tags);
            intent.PutExtra("summary", summary);
            intent.PutExtra("mode", mode);
            context.StartActivity(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            StatusBarUtil.SetColorStatusBars(this);
            SetToolBarNavBack();

             edit_url = FindViewById<EditText>(Resource.Id.edit_url);
            edit_title = FindViewById<EditText>(Resource.Id.edit_title);
            edit_summary = FindViewById<EditText>(Resource.Id.edit_summary);
            edit_tags = FindViewById<EditText>(Resource.Id.edit_tags);
            btn_submit = FindViewById<Button>(Resource.Id.btn_submit);
            ly_expire = FindViewById<LinearLayout>(Resource.Id.ly_expire);
            tv_startLogin = FindViewById<TextView>(Resource.Id.tv_startLogin);
            tv_startLogin.Click += (s, e) =>
            {
                StartActivity(new Intent(this,typeof(loginactivity)));
            };
            AlertUtil.ToastLong(this, "当前线程id:" + Thread.CurrentThread.ManagedThreadId);

            string  title = Intent.GetStringExtra("title");
            string url = Intent.GetStringExtra("url");
            mode = Intent.GetStringExtra("mode");

            edit_url.Text = url;
            edit_title.Text = title;
            Token token = UserTokenUtil.GetToken(this);
            if (mode == "edit")
            {
                edit_title.Focusable = true;
                string editTitle = Resources.GetString(Resource.String.bookmark_edit);
                btn_submit.Text = editTitle;
                SetToolBarTitle(editTitle);
                string summary = Intent.GetStringExtra("summary");
                string tags = Intent.GetStringExtra("tags");
                wzLinkId = Intent.GetIntExtra("id",0);
                if (!string.IsNullOrEmpty(summary))
                {
                    edit_summary.Text = summary;   
                }
                if (!string.IsNullOrEmpty(tags))
                {
                    edit_tags.Text = tags;
                }
            }
            if (mode == "add")
            {
                edit_title.Focusable = false;
                btn_submit.Text = "添加收藏";
            }
            if (token.IsExpire)
            {
                btn_submit.Enabled = false;
                ly_expire.Visibility = ViewStates.Visible;
            }
            else
            {
                btn_submit.Enabled = true;
                ly_expire.Visibility = ViewStates.Gone;
            }
            btn_submit.Click += async (s, e) =>
            {
                var userToken = UserTokenUtil.GetToken(this);
                if (userToken.IsExpire)
                {
                    Android.Support.V7.App.AlertDialog.Builder alertDialog = new Android.Support.V7.App.AlertDialog.Builder(this)
                    .SetTitle("登录提示")
                     .SetMessage("未登录或登录token已经过期")
                     .SetPositiveButton("授权", (s1, e1) =>
                     {
                         StartActivity(new Intent(this, typeof(loginactivity)));
                     })
                     .SetNegativeButton("取消", (s1, e1) =>
                     {
                         return;
                     });
                    alertDialog.Create().Show();
                }
                if (edit_summary.Text.Length > 200)
                {
                    AlertUtil.ToastShort(this, "摘要不能超过200字符");
                    return;
                }
                if (token.IsExpire)
                {
                    AlertUtil.ToastShort(this, "未登录或登录token已经过期，请邓丽");
                    return;
                }
                var model = new BookmarksModel();
                model.Title = edit_title.Text;
                model.LinkUrl = edit_url.Text;
                model.Summary = edit_summary.Text;
                model.Tags = edit_tags.Text.Split(',').ToList();
                var dialog = new ProgressDialog(this);
                dialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                dialog.SetCancelable(false);
                dialog.SetCanceledOnTouchOutside(false);
                dialog.SetTitle(Resources.GetString(Resource.String.bookmark_add));
                dialog.SetMessage("提交中.......");
                dialog.Show();
                if (mode == "add")
                {
                    var result = await BookmarksRequest.Add(token, model);

                    if (result.Success)
                    {
                        btn_submit.Enabled = false;
                        dialog.Hide();
                        AlertUtil.ToastLong(this, result.Message + "添加收藏成功");
                        this.Finish();
                        System.Diagnostics.Debug.Write(result.Message);
                    }
                    else
                    {
                        dialog.Hide();
                        AlertUtil.ToastLong(this, result.Message);
                        System.Diagnostics.Debug.Write(result);
                    }
                }
                if (mode == "edit")
                {
                    model.WzLinkId = wzLinkId;
                    BookmarksRequest.Edit(token, model,(result)=> {
                         if (result.IsSuccess)
                         {
                            RunOnUiThread(()=> {
                                dialog.Hide();
                                btn_submit.Enabled = false;
                                AlertUtil.ToastLong(this, result.Message + "编辑收藏成功");
                            });
                             System.Diagnostics.Debug.Write(result.Message);
                         }
                         else
                         {
                            RunOnUiThread(() =>
                            {
                                dialog.Hide();
                                AlertUtil.ToastLong(this, result.Message);
                                System.Diagnostics.Debug.Write(result);
                            });                             
                         }
                     });
                }
            };
        }

        public override bool OnMenuItemClick(IMenuItem item)
        {
            //if (article != null)
            //{
            //    // sharesWidget
            //    shareWidget.Open(article.Url,article.Title);
            //}
            return true;
        }

        //public void OnClick(View v)
        //{
        //    ActivityCompat.FinishAfterTransition(this);
        //}
    }
}