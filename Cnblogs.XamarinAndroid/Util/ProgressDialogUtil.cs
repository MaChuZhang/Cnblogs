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

namespace Cnblogs.XamarinAndroid.Util
{
    public class ProgressDialogUtil
    {
        public static Context context;
        static ProgressDialog dialog;
        public static void Show(string msg, string title)
        {
            // ProgressDialogUtil
            if (dialog == null)
                dialog = new ProgressDialog(context);
            dialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            dialog.SetCancelable(false);
            dialog.SetCanceledOnTouchOutside(false);
            dialog.SetTitle(title);
            dialog.SetMessage(msg);
            dialog.Show();
            AlertDialogUtil.itemClick += (s, e) =>
            {
                //DialogClickEventArgs.Empty.
                var _params = e as DialogClickEventArgs;
                //_params.
            };
            
        }
    }
    public class AlertDialogUtil 
    {
        private static Context context;



        public void OnClick(IDialogInterface dialog, int which)
        {
            //throw new NotImplementedException();
        }
        public static EventHandler<DialogClickEventArgs> itemClick;
        internal void OnClick(DialogClickEventArgs e,object o)
        {
            itemClick.Invoke(o,e);
        }
        internal void show()
        
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.SetItems(new string[] { "删除", "修改", "保存到本地" }, itemClick);
            builder.Show();
        }
    }
}