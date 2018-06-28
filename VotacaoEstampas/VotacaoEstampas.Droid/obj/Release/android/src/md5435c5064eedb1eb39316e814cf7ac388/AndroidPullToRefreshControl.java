package md5435c5064eedb1eb39316e814cf7ac388;


public class AndroidPullToRefreshControl
	extends android.widget.LinearLayout
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("DevExpress.Mobile.DataGrid.Android.AndroidPullToRefreshControl, DevExpress.Mobile.Grid.Android.v17.1", AndroidPullToRefreshControl.class, __md_methods);
	}


	public AndroidPullToRefreshControl (android.content.Context p0)
	{
		super (p0);
		if (getClass () == AndroidPullToRefreshControl.class)
			mono.android.TypeManager.Activate ("DevExpress.Mobile.DataGrid.Android.AndroidPullToRefreshControl, DevExpress.Mobile.Grid.Android.v17.1", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public AndroidPullToRefreshControl (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == AndroidPullToRefreshControl.class)
			mono.android.TypeManager.Activate ("DevExpress.Mobile.DataGrid.Android.AndroidPullToRefreshControl, DevExpress.Mobile.Grid.Android.v17.1", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public AndroidPullToRefreshControl (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == AndroidPullToRefreshControl.class)
			mono.android.TypeManager.Activate ("DevExpress.Mobile.DataGrid.Android.AndroidPullToRefreshControl, DevExpress.Mobile.Grid.Android.v17.1", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
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
