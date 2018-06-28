package md5435c5064eedb1eb39316e814cf7ac388;


public class DroidDisplayService
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("DevExpress.Mobile.DataGrid.Android.DroidDisplayService, DevExpress.Mobile.Grid.Android.v17.1", DroidDisplayService.class, __md_methods);
	}


	public DroidDisplayService ()
	{
		super ();
		if (getClass () == DroidDisplayService.class)
			mono.android.TypeManager.Activate ("DevExpress.Mobile.DataGrid.Android.DroidDisplayService, DevExpress.Mobile.Grid.Android.v17.1", "", this, new java.lang.Object[] {  });
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
