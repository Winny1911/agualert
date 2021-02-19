using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading;
using XLabs.Forms;
using XLabs.Ioc; // Using for SimpleContainer
using XLabs.Platform.Services.Geolocation; // Using for Geolocation
using XLabs.Platform.Device; // Using for Display
using Android.Locations;
using Android.Provider;

namespace TaVazando.Droid
{
	[Activity (Label = "TaVazando", Theme="@style/Theme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]	
	public class MainActivity : XFormsApplicationDroid//global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{

	

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Xlabs
			var container = new SimpleContainer();
			container.Register<IDevice> (t => AndroidDevice.CurrentDevice);
			container.Register<IGeolocator,  global::XLabs.Platform.Services.Geolocation.Geolocator>();
			Resolver.SetResolver(container.GetResolver()); // Resolving the services
			// Xlabs
			//Thread.Sleep (1000);
			StartActivity (typeof(ActivityLoader));
			Finish ();
		}



	}
}
	