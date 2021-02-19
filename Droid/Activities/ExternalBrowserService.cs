using System;
using Xamarin.Forms;
using TaVazando.Droid;
using Android.Content;
using Android.App;


[assembly: Dependency(typeof(ExternalBrowserService))]
namespace TaVazando.Droid
{
	public class ExternalBrowserService: Activity, IExternalBrowserService
	{
		public void OpenUrl(string url)
		{
//			if(string.IsNullOrEmpty(url)) return;
//
//			if (!url.StartsWith("http://") && !url.StartsWith("https://"))
//				url = "http://" + url;
//
//			var intent = new Intent(Intent.ActionView, Uri(url.ToString()));
//			intent.AddFlags(ActivityFlags.NewTask);
//
//			this.ApplicationContext.StartActivity(intent);
		}
	}
}

