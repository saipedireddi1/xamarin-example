using System;
using System.Collections.Generic;
using System.Reflection;

using Xamarin.Forms;
using ImageCircle.Forms.Plugin.Abstractions;
using System.IO;

namespace DroverXamarin
{
	public partial class CalendarPage : ContentPage
	{
		DateTime date;

		public CalendarPage()
		{
			InitializeComponent();

			//Set the date to today's date and fill the calendar
			date = DateTime.Today;
			monthChanged(date);

			//Back button functionality
			var backButtonGesture = new TapGestureRecognizer();
			backButtonGesture.Tapped += (s, e) =>
			{
				Navigation.PopModalAsync();
			};
			back_arrow_button.GestureRecognizers.Add(backButtonGesture);

			//Left arrow functonality
			var left_arrow_gesture = new TapGestureRecognizer();
			left_arrow_gesture.Tapped += (s, e) =>
			{
				leftArrowTapped(s, e);
			};
			left_arrow.GestureRecognizers.Add(left_arrow_gesture);

			//Right arrow functionality
			var right_arrow_gesture = new TapGestureRecognizer();
			right_arrow_gesture.Tapped += (s, e) =>
			{
				
				rightArrowTapped(s, e);
			};
			right_arrow.GestureRecognizers.Add(right_arrow_gesture);


		}

		public void leftArrowTapped(object sender, EventArgs e)
		{
			date = date.AddMonths(-1);
			monthChanged(date);
		}

		public void rightArrowTapped(object sender, EventArgs e)
		{
			date = date.AddMonths(1);
			monthChanged(date);
		}


		public void monthChanged(DateTime new_date)
		{
			
			date_grid.Children.Clear();

			date_grid.Children.Add(new Label { Text = "S", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 0, 0);
			date_grid.Children.Add(new Label { Text = "M", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 1, 0);
			date_grid.Children.Add(new Label { Text = "T", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 2, 0);
			date_grid.Children.Add(new Label { Text = "W", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 3, 0);
			date_grid.Children.Add(new Label { Text = "R", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 4, 0);
			date_grid.Children.Add(new Label { Text = "F", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 5, 0);
			date_grid.Children.Add(new Label { Text = "S", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 6, 0);

			date_label.Text = new_date.ToString("MMMM yyyy");


			while (new_date.Day != 1)
			{
				new_date = new_date.AddDays(-1);
			}

			String dayOfFirst = new_date.DayOfWeek.ToString();
			int row = 1;
			int col = 0;
			switch (dayOfFirst)
			{
				case "Sunday":
					col = 0;
					break;
				case "Monday":					
					col = 1;
					break;
				case "Tuesday":					
					col = 2;
					break;
				case "Wednesday":
					col = 3;
					break;
				case "Thursday":
					col = 4;
					break;
				case "Friday":
					col = 5;
					break;
				case "Saturday":
					col = 6;
					break;
			}

			string path = "users/" + FireAuth.auth.User.LocalId + "/ride_info/ride_history/" + new_date.ToString("M-y");
			var rides = FireDatabase.read(path).Result;


			for (int i = 0; i < DateTime.DaysInMonth(new_date.Year, new_date.Month); i++)
			{
				if (col == 7)
				{
					row++;
					col = 0;
				}

				var nextDay = new Label { Text = (i + 1).ToString(), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center};


				int numDays = i;
				var day_clicked_gesture = new TapGestureRecognizer();
				day_clicked_gesture.Tapped += (s, e) =>
				{
					Console.WriteLine("TEST");
					nextDay.BackgroundColor = Color.Gray;
					if (rides.ContainsKey(new_date.ToString("M-") + nextDay.Text + new_date.ToString("-y")))
					{
						DateTime date_clicked_on = new DateTime();
						date_clicked_on = new_date.AddDays(numDays);
						Console.WriteLine("TEST3");
						Navigation.PushModalAsync(new RideListPage(date_clicked_on));
					}
					nextDay.BackgroundColor = Color.Transparent;

				};

				nextDay.GestureRecognizers.Add(day_clicked_gesture);

				date_grid.Children.Add(nextDay, col, row);

				if (rides != null)
				{
					//Add ride markers for neccessary days
					if (rides.ContainsKey(new_date.ToString("M-") + nextDay.Text + new_date.ToString("-y")))
					{
						addRideMarker(nextDay, col, row);
					}
				}

				col++;
			}



		}

		public void changeDateColor( Label nextDay)
		{
			if (nextDay.BackgroundColor != Xamarin.Forms.Color.Gray)
			{
				nextDay.BackgroundColor = Xamarin.Forms.Color.Gray;
			}
			else
			{
				nextDay.BackgroundColor = Xamarin.Forms.Color.Transparent;
			}
		}

		public void addRideMarker(Label day, int col, int row)
		{
			var ride_marker = new CircleImage
			{
				Source = "circle.png",
				HorizontalOptions = Xamarin.Forms.LayoutOptions.Center,
				VerticalOptions = Xamarin.Forms.LayoutOptions.Center,
				Margin = new Thickness(0, 0, 0, 0)
			};

			if (date_grid.Children.Contains(day)) {

				date_grid.Children.Remove(day);
			}
			day.TextColor = Xamarin.Forms.Color.White;
			date_grid.Children.Add(ride_marker, col, row);
			date_grid.Children.Add(day,col,row);

		}

		public void removeRideMarker(Label day)
		{
			date_grid.Children.Remove(day);
		}
	}
}
