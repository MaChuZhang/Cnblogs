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
namespace Cnblogs.XamarinAndroid
{
    public class UserCenterFragment : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu=true;
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
    }
}