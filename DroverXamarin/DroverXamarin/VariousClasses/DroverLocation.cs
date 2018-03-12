using System;
namespace DroverXamarin
{
	public class DroverLocation
	{
		public String locationName = "Unknown Location Name";
		public double lat = 0;
		public double lng = 0;

		public DroverLocation(String n, double latitude, double longitude)
		{
			locationName = n;
			lat = latitude;
			lng = longitude;
		}
	}
}
