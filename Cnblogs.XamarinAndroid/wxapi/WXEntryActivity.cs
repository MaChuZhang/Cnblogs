
using Android.App;
using Com.Umeng.Weixin.Callback;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Exported = true , Theme = "@style/AppTheme", ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class WXEntryActivity : WXCallbackActivity
    {
    }
}