using System;
using DroverXamarin.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(IOSRegisterDriverMode))]
namespace DroverXamarin.iOS
{
	public class IOSRegisterDriverMode : IRegisterDriverMode
	{
		LocationManagerIOS Manager;

		private static IOSRegisterDriverMode instance = null;
		public static bool shouldPushLocation = false;

		public IOSRegisterDriverMode()
		{

		}

		private IOSRegisterDriverMode(int x)
		{
			Manager = new LocationManagerIOS();
			Manager.LocationUpdated += IOSLocationUpdates.HandleLocationChanged;
		}

		public void Register()
		{
			shouldPushLocation = true;
			instance.Manager.StartLocationUpdates();
		}

		public void UnRegister()
		{
			shouldPushLocation = false;
			instance.Manager.StopLocationUpdates();
		}

		public void Init()
		{
			instance = new IOSRegisterDriverMode(2);
		}
	}
}
