using System;
using TaVazando.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidMethods))]
namespace TaVazando.Droid
{
	public class AndroidMethods : IAndroidMethods
	{
		public void CloseApp()
		{
			Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
		}
	}
}

