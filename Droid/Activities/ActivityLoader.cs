using System;
using Android.App;
using Xamarin.Forms;
using Android.OS;
using Android.Content.PM;
using Android.Locations;
using Android.Content;
using Android.Provider;
using Android.Telephony;

namespace TaVazando.Droid
{

	[Activity (Label = "ActivityLoader", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]	
	public class ActivityLoader : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{

		public ActivityLoader ()
		{
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);
			Xamarin.FormsMaps.Init(this, bundle);
			LoadApplication (new App ());
		}



	}
}

