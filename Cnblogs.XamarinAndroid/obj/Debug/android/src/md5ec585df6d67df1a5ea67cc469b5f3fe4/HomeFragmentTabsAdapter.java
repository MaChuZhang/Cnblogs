package md5ec585df6d67df1a5ea67cc469b5f3fe4;


public class HomeFragmentTabsAdapter
	extends android.support.v4.app.FragmentPagerAdapter
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_getCount:()I:GetGetCountHandler\n" +
			"n_getPageTitle:(I)Ljava/lang/CharSequence;:GetGetPageTitle_IHandler\n" +
			"n_getItem:(I)Landroid/support/v4/app/Fragment;:GetGetItem_IHandler\n" +
			"";
		mono.android.Runtime.register ("Cnblogs.XamarinAndroid.HomeFragmentTabsAdapter, Cnblogs.XamarinAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", HomeFragmentTabsAdapter.class, __md_methods);
	}


	public HomeFragmentTabsAdapter (android.support.v4.app.FragmentManager p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == HomeFragmentTabsAdapter.class)
			mono.android.TypeManager.Activate ("Cnblogs.XamarinAndroid.HomeFragmentTabsAdapter, Cnblogs.XamarinAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Support.V4.App.FragmentManager, Xamarin.Android.Support.v4, Version=1.0.0.0, Culture=neutral, PublicKeyToken=71f3e3261ac778b5", this, new java.lang.Object[] { p0 });
	}


	public int getCount ()
	{
		return n_getCount ();
	}

	private native int n_getCount ();


	public java.lang.CharSequence getPageTitle (int p0)
	{
		return n_getPageTitle (p0);
	}

	private native java.lang.CharSequence n_getPageTitle (int p0);


	public android.support.v4.app.Fragment getItem (int p0)
	{
		return n_getItem (p0);
	}

	private native android.support.v4.app.Fragment n_getItem (int p0);

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
