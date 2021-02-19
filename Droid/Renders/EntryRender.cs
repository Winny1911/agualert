using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using TaVazando.Droid;

[assembly: ExportRenderer(typeof(Entry), typeof(CustomEditTextRender))]
namespace TaVazando.Droid
{
	public class CustomEditTextRender : EntryRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			var nativeControl = (EditText)Control; // UITextView(iOS), EditText(Android)
			nativeControl.SetTextSize(Android.Util.ComplexUnitType.Pt, 7);
			nativeControl.SetTextColor (Android.Graphics.Color.ParseColor("#1b5fa8"));
		}
	}
}