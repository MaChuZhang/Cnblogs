package md5ec585df6d67df1a5ea67cc469b5f3fe4;


public class BaseHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Cnblogs.XamarinAndroid.BaseHolder, Cnblogs.XamarinAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", BaseHolder.class, __md_methods);
	}


	public BaseHolder (android.view.View p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == BaseHolder.class)
			mono.android.TypeManager.Activate ("Cnblogs.XamarinAndroid.BaseHolder, Cnblogs.XamarinAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Views.View, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}

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
