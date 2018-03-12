using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace DroverXamarin.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
		LocationManagerIOS Manager;
		bool backgrounded = false;
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {

            global::Xamarin.Forms.Forms.Init();
			Xamarin.FormsMaps.Init();

			LoadApplication(new App());


			if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
								   UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
								   new NSSet());

				UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications();
			}
			else {
				UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
			}

			//INITIALIZE THE DependencyService's IOSREgisterForDriverMode();
			(new IOSRegisterDriverMode()).Init();


			App.ScreenWidth = (int)UIScreen.MainScreen.Bounds.Width;
			App.ScreenHeight = (int)UIScreen.MainScreen.Bounds.Height;
            return base.FinishedLaunching(app, options);
        }

		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			// Get current device token
			var DeviceToken = deviceToken.Description;
			if (!string.IsNullOrWhiteSpace(DeviceToken))
			{
				DeviceToken = DeviceToken.Trim('<').Trim('>');
				DeviceToken = Regex.Replace(DeviceToken, @"\s+", "");
			}

			// Get previous device token
			var oldDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("PushDeviceToken");

			// Has the token changed?
			if (string.IsNullOrEmpty(oldDeviceToken) || !oldDeviceToken.Equals(DeviceToken))
			{
				//TODO: Put your own logic here to notify your server that the device token has changed/been created!
				Console.WriteLine("RECIEVED TOKEN! "+DeviceToken);
				Settings.ApnsToken = DeviceToken;
				if (FireAuth.auth != null && !FireAuth.isTokenExpired() && FireDatabase.isDatabaseInit())
				{
					FireDatabase.write("users/" + FireAuth.auth.User.LocalId + "/device_info/apns_token", DeviceToken);
				}
			}

			// Save new device token 
			NSUserDefaults.StandardUserDefaults.SetString(DeviceToken, "PushDeviceToken");
		}

		public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
		{
			//Console.WriteLine("RECIEVED NOTIFICATION!");
			//FireDatabase.write("test", Time.CurrentTimeMillis().ToString());
			Console.WriteLine("Recieved a notification!");
			Console.WriteLine("test");
			var aps = (NSDictionary)userInfo["aps"];
			var alert = aps["alert"];

			Console.WriteLine(alert.ToString());
			if (alert.ToString().Equals("No Drivers are in your area!"))
			{
				if (!backgrounded)
				{
					if (MainPage.instance != null)
					{
						MainPage.instance.resetGUI();
					}
					showNotification(alert.ToString());

				}
				Settings.AppState = "DEFAULT";
			}
			else if (alert.ToString().Equals("You changed your Drop off Location!"))
			{
				var test = userInfo["verbose"];
				Instances.DropOffLocation = new DroverLocation(test.ToString(), Double.Parse(userInfo["lat"].ToString()), Double.Parse(userInfo["lng"].ToString()));
			}
			else if (alert.ToString().Equals("Rider changed their Drop off Location!"))
			{
				var test = userInfo["verbose"];
				Instances.DropOffLocation = new DroverLocation(test.ToString(), Double.Parse(userInfo["lat"].ToString()), Double.Parse(userInfo["lng"].ToString()));
			}
			else if (alert.ToString().Equals("SEARCHING FOR RIDE!"))
			{
				Settings.AppState = "SEARCHING";
			}
			else if (alert.ToString().Equals("No drivers accepted your request"))
			{
				if (!backgrounded)
				{
					if (MainPage.instance != null)
					{
						MainPage.instance.resetGUI();
					}
					showNotification(alert.ToString());

				}
				Settings.AppState = "DEFAULT";
			}
			else if (alert.ToString().Equals("Your Drover is on the way!"))
			{
				if (!backgrounded)
				{
					if (MainPage.instance != null)
					{
						var dict = FireDatabase.read("users/" + FireAuth.auth.User.LocalId).Result;
						string rideID = dict["is_in_ride"].ToString();
						Settings.LastRideID = rideID;
						MainPage.instance.enterRiderGUI();
					}
					showNotification(alert.ToString());

				}
				Settings.AppState = "WAITING";
				Settings.InRide = true;
			}
			else if (alert.ToString().Equals("You are being Requested!"))
			{
				
				if (!backgrounded)
				{
					if (MainPage.instance != null)
					{
						//proposeRide();
						//MainPage.instance.enterRiderGUI();
					}
					proposeRide();

				}
				Settings.AppState = "REQUESTED";
			}
			else if (alert.ToString().Equals("You have accepted a ride request."))
			{
				if (!backgrounded)
				{
					MainPage.instance.enterDriverGui();
				}
				Settings.AppState = "DRIVING";
				var dict = FireDatabase.read("users/" + FireAuth.auth.User.LocalId).Result;
				string rideID = dict["is_driving"].ToString();
				Settings.LastRideID = rideID;
			}
			//base.DidReceiveRemoteNotification(application, userInfo, completionHandler);
			else if (alert.ToString().Equals("The ride is over!"))
			{
				Settings.InRide = false;
			}
		}

		public static void showNotification(string message)
		{
			UIInstance.app.MainPage.DisplayAlert("Hey!", message, "OK");
		}

		public void proposeRide()
		{
			Console.WriteLine("TEST!");
			//Task.Factory.StartNew(async () =>
			//{
				Console.WriteLine("TEST!2");
			//Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
			//{
			UIInstance.app.MainPage.DisplayAlert("Notice", "Someone is looking for a ride", "yes", "no").ContinueWith(t =>
			{
				GenericSocket.PROPOSAL_RESPONSE(t.Result);
			});
					//var answer = m.Result;
					//GenericSocket.PROPOSAL_RESPONSE(answer);
				//});
				//await Task.Delay(10);
			//});
		}


		public override void DidEnterBackground(UIApplication application)
		{
			Console.WriteLine("App entering background state.");
			backgrounded = true;
		}

		public override void WillEnterForeground(UIApplication application)
		{
			Console.WriteLine("App will enter foreground");
			backgrounded = false;
		}


    }
}
