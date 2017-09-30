package md51c66d3d5c3e0736558fca3a4c59ce2c6;


public class RecyclerViewDecoration_MyItemDecoration
	extends android.support.v7.widget.RecyclerView.ItemDecoration
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDraw:(Landroid/graphics/Canvas;Landroid/support/v7/widget/RecyclerView;)V:GetOnDraw_Landroid_graphics_Canvas_Landroid_support_v7_widget_RecyclerView_Handler\n" +
			"n_getItemOffsets:(Landroid/graphics/Rect;ILandroid/support/v7/widget/RecyclerView;)V:GetGetItemOffsets_Landroid_graphics_Rect_ILandroid_support_v7_widget_RecyclerView_Handler\n" +
			"";
		mono.android.Runtime.register ("Cnblogs.XamarinAndroid.UI.Fragments.RecyclerViewDecoration+MyItemDecoration, Cnblogs.XamarinAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", RecyclerViewDecoration_MyItemDecoration.class, __md_methods);
	}


	public RecyclerViewDecoration_MyItemDecoration () throws java.lang.Throwable
	{
		super ();
		if (getClass () == RecyclerViewDecoration_MyItemDecoration.class)
			mono.android.TypeManager.Activate ("Cnblogs.XamarinAndroid.UI.Fragments.RecyclerViewDecoration+MyItemDecoration, Cnblogs.XamarinAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public RecyclerViewDecoration_MyItemDecoration (android.content.Context p0, int p1) throws java.lang.Throwable
	{
		super ();
		if (getClass () == RecyclerViewDecoration_MyItemDecoration.class)
			mono.android.TypeManager.Activate ("Cnblogs.XamarinAndroid.UI.Fragments.RecyclerViewDecoration+MyItemDecoration, Cnblogs.XamarinAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1 });
	}


	public void onDraw (android.graphics.Canvas p0, android.support.v7.widget.RecyclerView p1)
	{
		n_onDraw (p0, p1);
	}

	private native void n_onDraw (android.graphics.Canvas p0, android.support.v7.widget.RecyclerView p1);


	public void getItemOffsets (android.graphics.Rect p0, int p1, android.support.v7.widget.RecyclerView p2)
	{
		n_getItemOffsets (p0, p1, p2);
	}

	private native void n_getItemOffsets (android.graphics.Rect p0, int p1, android.support.v7.widget.RecyclerView p2);

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
