using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DroverXamarin
{
	public partial class RideListPage : ContentPage
	{
        public static List<Task> taskList = new List<Task>();
        Dictionary<string, RideListEntry> entries = new Dictionary<string, RideListEntry>();
        RideStack ride_stack;
        int numRides;
        DateTime date_of_rides;
        Dictionary<string, object> rides;
        double perMinuteRate = 0.3;
        double perMileRate = 1.8;
        double baseRate = 4;
        double safetyCharge = 1.55;


        public RideListPage(DateTime date_of_rides)
		{
			InitializeComponent();

			// Grabs the list of all rides IDs on a specific date from the database
			string path_of_rides = "users/" + FireAuth.auth.User.LocalId + "/ride_info/ride_history/" + date_of_rides.ToString("M-y") + "/" + date_of_rides.ToString("M-d-yy");
			rides = FireDatabase.read(path_of_rides).Result;
			numRides = rides.Values.Count;

			this.date_of_rides = date_of_rides;

			// Adds a back button
			var backButtonGesture = new TapGestureRecognizer();
			backButtonGesture.Tapped += (s, e) =>
			{
				Navigation.PopModalAsync();
			};
			back_arrow_button.GestureRecognizers.Add(backButtonGesture);

			// Sets the ride date text
			ride_date.Text = date_of_rides.ToString("MM-dd-yyyy");


			// Creates a new stack of rides
			ride_stack = new RideStack();
			scroll_view.Content = ride_stack;
			ride_stack.ChildAdded += method;

            SetupInitialView();
		}

		public void method(object sender, ElementEventArgs args)
		{
			if (args.Element is RideListEntry && ((RideListEntry)args.Element).Children.Count == 1)
			{
				// Start an asynchronous task
				Task.Run(async () =>
				{
					// Grab the ride ID from the ride object
					int index = ride_stack.Children.IndexOf((RideListEntry)args.Element) / 2;
					var ride_id = (string)rides.ElementAt(index).Value;

					string ridePath = "rides/" + ride_id;
					var ride_dict = FireDatabase.read(ridePath).Result;


					// Grab all the information about the current ride from the database
					string driver_name = "name";
					var duid = (string)ride_dict["duid"];
					string dropOff_location = "Location";

					getDriverName(ref driver_name, duid);

					var tracker_dict =  ((JObject)(ride_dict["tracker"])).Values<object>();

					double total_dist = 0;

					for (int i = 0; i < tracker_dict.Count() - 1; i++)
					{
						var tracker1 = (JObject)((JContainer)tracker_dict.ElementAt(i)).First();
						var tracker2 = (JObject)((JContainer)tracker_dict.ElementAt(i + 1)).First();

						var lat1 = tracker1["lat"];
						var lng1 = tracker1["lng"];
						var lat2 = tracker2["lat"];
						var lng2 = tracker2["lng"];

						if (i + 1 == tracker_dict.Count() - 1)
						{
							dropOff_location = lat2 + "," +  lng2;
						}

						total_dist += calcDistBetweenPoints((double)lat1, (double)lng1, (double)lat2, (double)lng2);
					}
					total_dist *= 0.621371;

					var start_time = ConvertUnixTimeStamp((long)ride_dict["start_time"]);
					var finish_time = ConvertUnixTimeStamp((long)ride_dict["finish_time"]);
					var time = start_time.Subtract(finish_time).Duration();

					var ride_time = time.ToString();
					string ride_distance = Math.Round(total_dist, 2).ToString() + " miles";
					string ride_cost = "$" + (Math.Round((time.TotalMinutes * perMinuteRate) + (total_dist * perMileRate) + safetyCharge + baseRate,2)).ToString();

					Device.BeginInvokeOnMainThread(() =>
					{
						ride_stack.Children[ride_stack.Children.IndexOf((View)args.Element)] = new RideListEntry(driver_name, dropOff_location, ride_time, ride_distance, ride_cost);
					});
					await Task.Delay(100);
				});
			}
		}

		public async Task SetupInitialView()
		{
			// Sets up the initial view asynchronously
			await Task.Run(async () =>
			{
				for (int i = 0; i < numRides; i++)
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						ride_stack.addRide(new RideListEntry());
					});
					await Task.Delay(25);
				}
			});
		}

		private void getDriverName(ref string driver_name, string duid)
		{
			// Gets the driver name for the current ride from the database
			string driver_path = "users/" + duid + "/personal_info";
			var driver = FireDatabase.read(driver_path).Result;
			driver_name = driver["name_first"] + " " + driver["name_last"];
		}


		private double calcDistBetweenPoints(double lat1, double lng1, double lat2, double lng2)
		{
			// Calculates the distance between two coordinates
			var distance = Math.Sqrt(Math.Pow(lat1 - lat2, 2) + Math.Pow(lng1 - lng2, 2));
			distance *= 111.111;
			return distance;
		}

		public static DateTime ConvertUnixTimeStamp(long unixTimeStamp)
		{
			// Converts a Unix timestamp to a C# DateTime object
			return new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(Convert.ToDouble(unixTimeStamp));
		}
	}
}
