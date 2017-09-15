package md522127645c21675e531a6ac609ef72b2a;


public class LoginWebChromeClient
	extends android.webkit.WebChromeClient
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onProgressChanged:(Landroid/webkit/WebView;I)V:GetOnProgressChanged_Landroid_webkit_WebView_IHandler\n" +
			"";
		mono.android.Runtime.register ("Cnblogs.XamarinAndroid.UI.Activities.LoginWebChromeClient, Cnblogs.XamarinAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", LoginWebChromeClient.class, __md_methods);
	}


	public LoginWebChromeClient () throws java.lang.Throwable
	{
		super ();
		if (getClass () == LoginWebChromeClient.class)
			mono.android.TypeManager.Activate ("Cnblogs.XamarinAndroid.UI.Activities.LoginWebChromeClient, Cnblogs.XamarinAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public LoginWebChromeClient (android.widget.ProgressBar p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == LoginWebChromeClient.class)
			mono.android.TypeManager.Activate ("Cnblogs.XamarinAndroid.UI.Activities.LoginWebChromeClient, Cnblogs.XamarinAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Widget.ProgressBar, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public void onProgressChanged (android.webkit.WebView p0, int p1)
	{
		n_onProgressChanged (p0, p1);
	}

	private native void n_onProgressChanged (android.webkit.WebView p0, int p1);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
