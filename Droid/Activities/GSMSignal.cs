using System;
using Android.Telephony;
using TaVazando.Droid;
using Android.Content;
using Android.App;
using Android.OS;

[assembly: Xamarin.Forms.Dependency(typeof(GSMSignal))]

namespace TaVazando.Droid
{
	public class GSMSignal: PhoneStateListener, IGSMSignal
	{

		TelephonyManager _telephonyManager;
		GSMSignal _signalStrengthListener;
		public int SignalStrength { get; set; }

		public GSMSignal ()
		{
			_telephonyManager = (TelephonyManager)Application.Context.GetSystemService(Context.TelephonyService);
		}

		public delegate void SignalStrengthChangedDelegate(int strength);

		public event SignalStrengthChangedDelegate SignalStrengthChanged;

		public override void OnSignalStrengthsChanged(SignalStrength newSignalStrength)
		{
			if (newSignalStrength.IsGsm)
			{
				if (SignalStrengthChanged != null)
				{
					SignalStrengthChanged(newSignalStrength.GsmSignalStrength);
				}
			}
		}

		void HandleSignalStrengthChanged(int strength)
		{
			try 
			{
				this.SignalStrengthChanged -= HandleSignalStrengthChanged;
				_telephonyManager.Listen (_signalStrengthListener, PhoneStateListenerFlags.None);
				
				SignalStrength = strength;

			}
			catch (Exception ex)
			{
				SignalStrength = 0;
			}

		}


		public void GetSignalStrength(){
			
			try 
			{
				_telephonyManager.Listen (this, PhoneStateListenerFlags.SignalStrengths);
				this.SignalStrengthChanged += HandleSignalStrengthChanged;
			}
			catch (Exception ex) 
			{
				Console.WriteLine(ex.Message);
			}

		}
	}
}

