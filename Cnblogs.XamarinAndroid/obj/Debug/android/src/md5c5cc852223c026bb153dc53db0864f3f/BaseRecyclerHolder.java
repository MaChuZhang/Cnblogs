package md5c5cc852223c026bb153dc53db0864f3f;


public class BaseRecyclerHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Cnblogs.XamarinAndroid.Adapter.BaseRecyclerHolder, Cnblogs.XamarinAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", BaseRecyclerHolder.class, __md_methods);
	}


	public BaseRecyclerHolder (android.view.View p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == BaseRecyclerHolder.class)
			mono.android.TypeManager.Activate ("Cnblogs.XamarinAndroid.Adapter.BaseRecyclerHolder, Cnblogs.XamarinAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Views.View, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
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
