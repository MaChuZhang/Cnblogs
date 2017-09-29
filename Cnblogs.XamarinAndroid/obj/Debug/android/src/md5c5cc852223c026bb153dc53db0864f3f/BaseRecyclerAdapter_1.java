package md5c5cc852223c026bb153dc53db0864f3f;


public abstract class BaseRecyclerAdapter_1
	extends android.support.v7.widget.RecyclerView.Adapter
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onAttachedToRecyclerView:(Landroid/support/v7/widget/RecyclerView;)V:GetOnAttachedToRecyclerView_Landroid_support_v7_widget_RecyclerView_Handler\n" +
			"n_onDetachedFromRecyclerView:(Landroid/support/v7/widget/RecyclerView;)V:GetOnDetachedFromRecyclerView_Landroid_support_v7_widget_RecyclerView_Handler\n" +
			"n_onCreateViewHolder:(Landroid/view/ViewGroup;I)Landroid/support/v7/widget/RecyclerView$ViewHolder;:GetOnCreateViewHolder_Landroid_view_ViewGroup_IHandler\n" +
			"n_onBindViewHolder:(Landroid/support/v7/widget/RecyclerView$ViewHolder;I)V:GetOnBindViewHolder_Landroid_support_v7_widget_RecyclerView_ViewHolder_IHandler\n" +
			"";
		mono.android.Runtime.register ("Cnblogs.XamarinAndroid.Adapter.BaseRecyclerAdapter`1, Cnblogs.XamarinAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", BaseRecyclerAdapter_1.class, __md_methods);
	}


	public BaseRecyclerAdapter_1 () throws java.lang.Throwable
	{
		super ();
		if (getClass () == BaseRecyclerAdapter_1.class)
			mono.android.TypeManager.Activate ("Cnblogs.XamarinAndroid.Adapter.BaseRecyclerAdapter`1, Cnblogs.XamarinAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onAttachedToRecyclerView (android.support.v7.widget.RecyclerView p0)
	{
		n_onAttachedToRecyclerView (p0);
	}

	private native void n_onAttachedToRecyclerView (android.support.v7.widget.RecyclerView p0);


	public void onDetachedFromRecyclerView (android.support.v7.widget.RecyclerView p0)
	{
		n_onDetachedFromRecyclerView (p0);
	}

	private native void n_onDetachedFromRecyclerView (android.support.v7.widget.RecyclerView p0);


	public android.support.v7.widget.RecyclerView.ViewHolder onCreateViewHolder (android.view.ViewGroup p0, int p1)
	{
		return n_onCreateViewHolder (p0, p1);
	}

	private native android.support.v7.widget.RecyclerView.ViewHolder n_onCreateViewHolder (android.view.ViewGroup p0, int p1);


	public void onBindViewHolder (android.support.v7.widget.RecyclerView.ViewHolder p0, int p1)
	{
		n_onBindViewHolder (p0, p1);
	}

	private native void n_onBindViewHolder (android.support.v7.widget.RecyclerView.ViewHolder p0, int p1);

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
