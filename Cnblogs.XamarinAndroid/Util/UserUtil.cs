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
using Cnblogs.ApiModel;

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
        public bool LoginExpire()
        {
            
            var    userToken = UserTokenUtil.GetToken(context);
            if (userToken.IsExpire||string.IsNullOrEmpty(userToken.access_token))
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