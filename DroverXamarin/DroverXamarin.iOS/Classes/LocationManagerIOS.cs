using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreLocation;
using UIKit;

namespace DroverXamarin.iOS
{
	public class LocationManagerIOS
	{
protected CLLocationManager locMgr;

		public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };



		public LocationManagerIOS()
		{
			this.locMgr = new CLLocationManager();
			this.locMgr.PausesLocationUpdatesAutomatically = false;

			// iOS 8 has additional permissions requirements
			if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				locMgr.RequestAlwaysAuthorization(); // works in background
													 //locMgr.RequestWhenInUseAuthorization (); // only in foreground
			}

			// iOS 9 requires the following for background location updates
			// By default this is set to false and will not allow background updates
			if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
			{
				locMgr.AllowsBackgroundLocationUpdates = true;
			}

			//LocationUpdated += PrintLocation;
			LocationUpdated += locupdated;
			LocationUpdated += PrintLocation;
			Console.WriteLine("Done Setting UP Location");

		}

		public CLLocationManager LocMgr
		{
			get { return this.locMgr; }
		}
		bool firstTime = true;
		public void StartLocationUpdates()
		{
				// We need the user's permission for our app to use the GPS in iOS. This is done either by the user accepting
				// the popover when the app is first launched, or by changing the permissions for the app in Settings
				if (CLLocationManager.LocationServicesEnabled)
				{

					//set the desired accuracy, in meters
					LocMgr.DesiredAccuracy = 1;
					if (firstTime)
					{
						firstTime = false;
						LocMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
						{
							// fire our custom Location Updated event
							LocationUpdated(this, new LocationUpdatedEventArgs(e.Locations[e.Locations.Length - 1]));
						};
					}

					LocMgr.StartUpdatingLocation();
				}

		}


		public void StopLocationUpdates()
		{
			if (CLLocationManager.LocationServicesEnabled)
			{
				LocMgr.StopUpdatingLocation();
			}
			FireDatabase.write("cities/" + FireDatabase.currentCity + "/driver_locations/" + FireAuth.auth.User.LocalId, null);
		}

		public void locupdated(object sender, LocationUpdatedEventArgs e)
		{
			if (IOSRegisterDriverMode.shouldPushLocation)
			{
				Task.Factory.StartNew(() =>
				{
					var userDB = FireDatabase.read("users/" + FireAuth.auth.User.LocalId).Result;
					//Dictionary<string, Object> children = new Dictionary<string, Object>();
					if (userDB.ContainsKey("stage_1_driving"))
					{
						string stageID = userDB["stage_1_driving"].ToString();
						Dictionary<string, Object> tracker = new Dictionary<string, Object>();
						tracker.Add("lat", e.Location.Coordinate.Latitude);
						tracker.Add("lng", e.Location.Coordinate.Longitude);
						tracker.Add("time", Time.CurrentTimeMillis());
						FireDatabase.write("stage_1_ids/" + stageID + "/tracker/" + Time.CurrentTimeMillis(), tracker);
					}

					if (userDB.ContainsKey("is_driving"))
					{
						string rideID = userDB["is_driving"].ToString();
						Dictionary<string, Object> tracker = new Dictionary<string, Object>();
						tracker.Add("lat", e.Location.Coordinate.Latitude);
						tracker.Add("lng", e.Location.Coordinate.Longitude);
						tracker.Add("time", Time.CurrentTimeMillis());
						FireDatabase.write("rides/" + rideID + "/tracker/" + Time.CurrentTimeMillis(), tracker);
						FireDatabase.write("rides/" + rideID + "/latest_tracker", tracker);
					}

					Dictionary<string, Object> location = new Dictionary<string, Object>();
					location.Add("lat", e.Location.Coordinate.Latitude);
					location.Add("lng", e.Location.Coordinate.Longitude);
					FireDatabase.write("cities/" + FireDatabase.currentCity + "/driver_locations/" + FireAuth.auth.User.LocalId, location);
					if (!IOSRegisterDriverMode.shouldPushLocation)
					{
						FireDatabase.write("cities/" + FireDatabase.currentCity + "/driver_locations/" + FireAuth.auth.User.LocalId, null);
					}
				//FireDatabase.write("", children);
			});
			}
		}

		//This will keep going in the background and the foreground
		public void PrintLocation(object sender, LocationUpdatedEventArgs e)
		{
			
			/*
			CLLocation location = e.Location;
			Console.WriteLine("Altitude: " + location.Altitude + " meters");
			Console.WriteLine("Longitude: " + location.Coordinate.Longitude);
			Console.WriteLine("Latitude: " + location.Coordinate.Latitude);
			Console.WriteLine("Course: " + location.Course);
			Console.WriteLine("Speed: " + location.Speed);
			*/

		}
	}
}

