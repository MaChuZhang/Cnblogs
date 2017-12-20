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

namespace Cnblogs.XamarinAndroid
{
    public class UserUtil
    {
        private Context context;
        private  static  UserUtil instance;
        public  void Logout()
        {
            UserTokenUtil.SaveToken(new ApiModel.Token(),context);
        }
        public bool Expire()
        {
            var user = UserTokenUtil.GetToken(context);
            if (user.IsExpire||string.IsNullOrEmpty(user.access_token))
            {
                return true;
            }
            return  false;
        }
        public UserUtil(Context context)
        {
            this.context = context;
        }
        public static UserUtil Instance(Context context)
        {
            if (instance == null)
            {
                instance = new UserUtil(context);
            }
            return instance;
        }
    }
}