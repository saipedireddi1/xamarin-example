using System;
using System.Drawing;
using Xamarin.Forms;

namespace DroverXamarin
{
	public class RideListEntry : StackLayout
	{
		StackLayout textWrapper;
		StackLayout nameAndCostLayout;
		StackLayout locationTimeAndDistanceLayout;

		Image driver_profile_pic;
		Label driver_name;
		Label ride_dropoff_location;
		Label ride_time;
		Label ride_distance;
		Label ride_cost;


		public RideListEntry()
		{
			this.WidthRequest = App.ScreenWidth;
			this.HeightRequest = App.ScreenHeight / 10;


			//Create a loading label
			Label loadingLabel = new Label();

			int fontSize = 14;

			if (App.ScreenHeight < 600)
			{
				fontSize = 32;
			}
			else if (App.ScreenHeight < 700)
			{
				fontSize = 36;
			}
			else if (App.ScreenHeight < 1100)
			{
				fontSize = 52;
			}
			else if (App.ScreenHeight < 1400)
			{
				fontSize = 64;
			}

			loadingLabel.Text = "Loading...";
			loadingLabel.FontSize = fontSize;
			loadingLabel.Margin = 0;
			loadingLabel.WidthRequest = App.ScreenWidth;
			loadingLabel.HeightRequest = App.ScreenHeight / 10;
			loadingLabel.VerticalTextAlignment = TextAlignment.Center;
			loadingLabel.HorizontalTextAlignment = TextAlignment.Center;
			this.Children.Add(loadingLabel);
		}


	public RideListEntry(string name, string location, string time, string distance, string cost)
		{
			this.Orientation = StackOrientation.Horizontal;

			//Create layouts
			textWrapper = new StackLayout();
			nameAndCostLayout = new StackLayout();
			locationTimeAndDistanceLayout = new StackLayout();


			//Create variables for ride info
			driver_profile_pic = new Image();
			driver_name = new Label();
			ride_dropoff_location = new Label();
			ride_time = new Label();
			ride_distance = new Label();
			ride_cost = new Label();

			//Populate variables
			driver_profile_pic.Source = "profile_missing.jpg";
			driver_name.Text = name;
			ride_dropoff_location.Text = location;
			ride_time.Text = time;
			ride_distance.Text = distance;
			ride_cost.Text = cost;


			//Set font sizes and image size
			setFontSizes();

			//Image options
			int imageSize = App.ScreenHeight / 10;
			driver_profile_pic.HeightRequest = imageSize;
			driver_profile_pic.WidthRequest = imageSize;

			//Top text options
			driver_name.HorizontalOptions = LayoutOptions.StartAndExpand;
			driver_name.VerticalTextAlignment = TextAlignment.Center;
			ride_cost.HorizontalOptions = LayoutOptions.EndAndExpand;
			ride_cost.VerticalTextAlignment = TextAlignment.Center;

			//Bottom text options
			ride_dropoff_location.HorizontalOptions = LayoutOptions.StartAndExpand;
			ride_dropoff_location.VerticalTextAlignment = TextAlignment.End;
			ride_time.HorizontalOptions = LayoutOptions.StartAndExpand;
			ride_time.VerticalTextAlignment = TextAlignment.End;
			ride_distance.HorizontalOptions = LayoutOptions.EndAndExpand;
			ride_distance.VerticalTextAlignment = TextAlignment.End;
			ride_dropoff_location.LineBreakMode = LineBreakMode.TailTruncation;
			ride_dropoff_location.WidthRequest = nameAndCostLayout.WidthRequest / 3;

			//Top text container layout options
			nameAndCostLayout.Orientation = StackOrientation.Horizontal;
			nameAndCostLayout.HeightRequest = imageSize / 2;
			nameAndCostLayout.VerticalOptions = LayoutOptions.Center;

			//Bottom text container layout options
			locationTimeAndDistanceLayout.Orientation = StackOrientation.Horizontal;
			locationTimeAndDistanceLayout.HeightRequest = imageSize / 2;
			locationTimeAndDistanceLayout.VerticalOptions = LayoutOptions.EndAndExpand;

			//Container containing containers (wut lol) options
			textWrapper.WidthRequest = App.ScreenWidth - driver_profile_pic.Width - 1;
			textWrapper.HeightRequest = App.ScreenHeight / 10;

			//Add children to containers
			textWrapper.Children.Add(nameAndCostLayout);
			textWrapper.Children.Add(locationTimeAndDistanceLayout);

			nameAndCostLayout.Children.Add(driver_name);
			nameAndCostLayout.Children.Add(ride_cost);

			//LEAVE OUT UNTIL IMPLEMENTED LATER!!!! locationTimeAndDistanceLayout.Children.Add(ride_dropoff_location);
			locationTimeAndDistanceLayout.Children.Add(ride_time);
			locationTimeAndDistanceLayout.Children.Add(ride_distance);

			this.Children.Add(driver_profile_pic);
			this.Children.Add(textWrapper);
		}




		private void setFontSizes()
		{
			int fontSize = 14;

			if (App.ScreenHeight < 600)
			{
				fontSize = 16;
			}
			else if (App.ScreenHeight < 700)
			{
				fontSize = 18;
			}
			else if (App.ScreenHeight < 1100)
			{
				fontSize = 26;
			}
			else if (App.ScreenHeight < 1400)
			{
				fontSize = 32;
			}

			driver_name.FontSize = fontSize + 6;
			ride_cost.FontSize = fontSize + 6;
			ride_time.FontSize = fontSize;
			ride_distance.FontSize = fontSize;
			ride_dropoff_location.FontSize = fontSize;
		}
	}
}
