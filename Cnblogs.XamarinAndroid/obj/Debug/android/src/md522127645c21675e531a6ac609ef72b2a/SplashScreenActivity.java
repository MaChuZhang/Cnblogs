package md522127645c21675e531a6ac609ef72b2a;


public class SplashScreenActivity
	extends md522127645c21675e531a6ac609ef72b2a.BaseActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("Cnblogs.XamarinAndroid.UI.Activities.SplashScreenActivity, Cnblogs.XamarinAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", SplashScreenActivity.class, __md_methods);
	}


	public SplashScreenActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == SplashScreenActivity.class)
			mono.android.TypeManager.Activate ("Cnblogs.XamarinAndroid.UI.Activities.SplashScreenActivity, Cnblogs.XamarinAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
