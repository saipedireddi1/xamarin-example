using System;
using CoreLocation;

namespace DroverXamarin.iOS
{
	public class IOSLocationUpdates
	{
		public IOSLocationUpdates()
		{
		}

		public static void HandleLocationChanged(object sender, LocationUpdatedEventArgs e)
		{
			// Handle foreground updates
			CLLocation location = e.Location;
			Console.WriteLine("locatiion: " + location.Coordinate.ToString());
			//LblAltitude.Text = location.Altitude + " meters";
			//LblLongitude.Text = location.Coordinate.Longitude.ToString();
			//LblLatitude.Text = location.Coordinate.Latitude.ToString();
			//LblCourse.Text = location.Course.ToString();
			//LblSpeed.Text = location.Speed.ToString();

			Console.WriteLine("foreground updated");
		}
	}
}
