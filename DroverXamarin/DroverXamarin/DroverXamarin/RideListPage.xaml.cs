using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Threading;
using System.Resources;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DroverXamarin
{
	public partial class RideListPage : ContentPage
	{
		public static List<Task> taskList = new List<Task>();
		Dictionary<String, RideListEntry> entries = new Dictionary<string, RideListEntry>();
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

			//GET LIST OF RIDE IDS HERE 
			string path_of_rides = "users/" + FireAuth.auth.User.LocalId + "/ride_info/ride_history/" + date_of_rides.ToString("M-y") + "/" + date_of_rides.ToString("M-d-yy");
			this.rides = FireDatabase.read(path_of_rides).Result;
			this.numRides = rides.Values.Count;

			this.date_of_rides = date_of_rides;

			//Back button functionality
			var backButtonGesture = new TapGestureRecognizer();
			backButtonGesture.Tapped += (s, e) =>
			{
				Navigation.PopModalAsync();
			};
			back_arrow_button.GestureRecognizers.Add(backButtonGesture);

			ride_date.Text = date_of_rides.ToString("MM-dd-yyyy");


			ride_stack = new RideStack();
			scroll_view.Content = ride_stack;
			ride_stack.ChildAdded += method;



			SetupInitialView();
		}

		public void method(object sender, ElementEventArgs args)
		{
			if (args.Element is RideListEntry && ((RideListEntry)args.Element).Children.Count == 1)
			{
				//FIRE IT UP.
				Task.Run(async () =>
				{
					//Get Ride ID
					int index = ride_stack.Children.IndexOf((RideListEntry)args.Element) / 2;
					string ride_id = (string)rides.ElementAt(index).Value;

					string ridePath = "rides/" + ride_id;
					var ride_dict = FireDatabase.read(ridePath).Result;


					//GET INFO ABOUT RIDES HERE
					string driver_name = "name";
					string duid = (string)ride_dict["duid"];
					string dropOff_location = "Location";

					getDriverName(ref driver_name, duid);

					var tracker_dict =  ((JObject)(ride_dict["tracker"])).Values<Object>();

					double total_dist = 0;

					for (int i = 0; i < tracker_dict.Count() - 1; i++)
					{
						JObject tracker1 = (JObject)((JContainer)tracker_dict.ElementAt(i)).First();
						JObject tracker2 = (JObject)((JContainer)tracker_dict.ElementAt(i + 1)).First();

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

					DateTime start_time = ConvertUnixTimeStamp((long)ride_dict["start_time"]);
					DateTime finish_time = ConvertUnixTimeStamp((long)ride_dict["finish_time"]);
					TimeSpan time = start_time.Subtract(finish_time).Duration();

					string ride_time = time.ToString();
					string ride_distance = Math.Round(total_dist, 2).ToString() + " miles";
					string ride_cost = "$" + (Math.Round((time.TotalMinutes * perMinuteRate) + (total_dist * perMileRate) + safetyCharge + baseRate,2)).ToString();

					Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
					{
						ride_stack.Children[ride_stack.Children.IndexOf((Xamarin.Forms.View)args.Element)] = new RideListEntry(driver_name, dropOff_location, ride_time, ride_distance, ride_cost);
					});
					await Task.Delay(100);
				});
			}
		}

		public async Task SetupInitialView()
		{
			await Task.Run(async () =>
			{
				for (int i = 0; i < this.numRides; i++)
				{
					Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
					{
						ride_stack.addRide(new RideListEntry());
					});
					await Task.Delay(25);
				}
			});
		}

		private void getDriverName(ref string driver_name, string duid)
		{
			string driver_path = "users/" + duid + "/personal_info";
			var driver = FireDatabase.read(driver_path).Result;
			driver_name = driver["name_first"] + " " + driver["name_last"];
		}


		private double calcDistBetweenPoints(double lat1, double lng1, double lat2, double lng2)
		{
			double distance = Math.Sqrt(Math.Pow(lat1 - lat2, 2) + Math.Pow(lng1 - lng2, 2));
			distance *= 111.111;
			return distance;
		}

		public static DateTime ConvertUnixTimeStamp(long unixTimeStamp)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(Convert.ToDouble(unixTimeStamp));
		}
	}
}
