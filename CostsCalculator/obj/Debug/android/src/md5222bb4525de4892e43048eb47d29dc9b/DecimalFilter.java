package md5222bb4525de4892e43048eb47d29dc9b;


public class DecimalFilter
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.text.InputFilter
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_filter:(Ljava/lang/CharSequence;IILandroid/text/Spanned;II)Ljava/lang/CharSequence;:GetFilter_Ljava_lang_CharSequence_IILandroid_text_Spanned_IIHandler:Android.Text.IInputFilterInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("CostsCalculator.DecimalFilter, CostsCalculator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DecimalFilter.class, __md_methods);
	}


	public DecimalFilter () throws java.lang.Throwable
	{
		super ();
		if (getClass () == DecimalFilter.class)
			mono.android.TypeManager.Activate ("CostsCalculator.DecimalFilter, CostsCalculator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public DecimalFilter (int p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == DecimalFilter.class)
			mono.android.TypeManager.Activate ("CostsCalculator.DecimalFilter, CostsCalculator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0 });
	}


	public java.lang.CharSequence filter (java.lang.CharSequence p0, int p1, int p2, android.text.Spanned p3, int p4, int p5)
	{
		return n_filter (p0, p1, p2, p3, p4, p5);
	}

	private native java.lang.CharSequence n_filter (java.lang.CharSequence p0, int p1, int p2, android.text.Spanned p3, int p4, int p5);

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
