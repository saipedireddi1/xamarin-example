using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using System.Reflection;
using ImageCircle.Forms.Plugin.Abstractions;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DroverXamarin
{
    public partial class MainPage : ContentPage
    {
		// Tracker for if the user is in a ride or not
		bool inRide = false;
		
		// The string that holds the default text for the Drop-off location field
		public string drop_off_location = "Drop Off Location";
		
		// Static instance of the MainPage class
		public static MainPage instance = null;
		
		// The profile picture's circular pin image
		CircleImage profile_pic_pin;

        public MainPage()
        {
			BindingContext = this;
            InitializeComponent();
			instance = this;
			//Set the map to the Current Location
			setPositionToCurrentLocation();

			BindingContext = this;

			//SET UP SERACHING WEB VIEW
			var assembly = typeof(FireCore).GetTypeInfo().Assembly;

			Stream stream = assembly.GetManifestResourceStream("DroverXamarin.iOS.SEARCHING.html");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text += reader.ReadToEnd();
			}
			HtmlWebViewSource source = new HtmlWebViewSource();
			source.Html = text;
			searching_web.Source = source;


			//------------------------------Gesture Recognizer Section --------------------------

            //Three bars image tap functionality
            var drawer_button_gesture_recognizer = new TapGestureRecognizer();
            drawer_button_gesture_recognizer.Tapped += (s, e) => {
                onButtonClicked(s, e);
            };
            drawer_button.GestureRecognizers.Add(drawer_button_gesture_recognizer);

            
            //Quarter page filler box tap functionality
            var view_box_gesture_recognizer = new TapGestureRecognizer();
            view_box_gesture_recognizer.Tapped += (s, e) => {
                onButtonClicked(s, e);
            };
            view_box.GestureRecognizers.Add(view_box_gesture_recognizer);

			//Payment option tap functionality
			var paymentGesture = new TapGestureRecognizer();
			paymentGesture.Tapped += (s, e) =>
			{
				Navigation.PushModalAsync(new PaymentPage());
			};
			payment_button.GestureRecognizers.Add(paymentGesture);
			payment_picture.GestureRecognizers.Add(paymentGesture);

            //About option tap functionality
            var aboutGesture = new TapGestureRecognizer();
            aboutGesture.Tapped += (s, e) => {
                Navigation.PushModalAsync(new AboutPage());
            };
            about_button.GestureRecognizers.Add(aboutGesture);
			about_picture.GestureRecognizers.Add(aboutGesture);

			//Sign out option tab functionality
			var signOutGesture = new TapGestureRecognizer();
			signOutGesture.Tapped += (s, e) =>
			{
				signOutButtonClicked(s, e);
			};
			sign_out_button.GestureRecognizers.Add(signOutGesture);
			sign_out_picture.GestureRecognizers.Add(signOutGesture);

			//Help option tap functionality
            var helpGesture = new TapGestureRecognizer();
			helpGesture.Tapped += (s, e) =>
			{
				Navigation.PushModalAsync(new HelpPage());
			};
			help_button.GestureRecognizers.Add(helpGesture);
			help_picture.GestureRecognizers.Add(helpGesture);

			//History option tap functionality
			var historyGesture = new TapGestureRecognizer();
			historyGesture.Tapped += (s, e) =>
			{
				Navigation.PushModalAsync(new CalendarPage());
			};
			history_button.GestureRecognizers.Add(historyGesture);
			history_picture.GestureRecognizers.Add(historyGesture);

			//Search button tap functionality
			var searchGesture = new TapGestureRecognizer();
			searchGesture.Tapped += (s, e) =>
			{
				input_location_box.IsVisible = true;
				input_location_blackout_box.IsVisible = true;
				entered_pickup_location.Focus();

			};
			search_button.GestureRecognizers.Add(searchGesture);

			//Input location blackout box tap functionality
			var blackoutBoxGesture = new TapGestureRecognizer();
			blackoutBoxGesture.Tapped += (s, e) =>
			{
				input_location_box.IsVisible = false;
				input_location_blackout_box.IsVisible = false;

			};
			input_location_blackout_box.GestureRecognizers.Add(blackoutBoxGesture);

			//Profile Pic tap functionality
			var profilePicGesture = new TapGestureRecognizer();
			profilePicGesture.Tapped += (s, e) =>
			{
				switchPopMenu();	//PROFILE PICTURE STUFF
			};
			profile_pic.GestureRecognizers.Add(profilePicGesture);

			//Compass Image tap functionality
			var compassGesture = new TapGestureRecognizer();
			compassGesture.Tapped += (s, e) =>
			{
				setPositionToCurrentLocation();
			};
			compass_image.GestureRecognizers.Add(compassGesture);
			compass_frame.GestureRecognizers.Add(compassGesture);

			//Set Pickup Location tap functionality
			var setPickupGesture = new TapGestureRecognizer();
			setPickupGesture.Tapped += (s, e) =>
			{
				//FireDatabase.write("test/test+test", "test+test");
				getPinPosition();
			};
			set_pickup_button.GestureRecognizers.Add(setPickupGesture);

			//Confirm Pickup tap functionality
			var confirmPickupGesture = new TapGestureRecognizer();
			confirmPickupGesture.Tapped += (s, e) =>
			{
				confirmPickup();
			};
			confirm_location_button.GestureRecognizers.Add(confirmPickupGesture);

			//Cancel Pickup tap functionality
			var cancelPickupGesture = new TapGestureRecognizer();
			cancelPickupGesture.Tapped += (s, e) =>
			{
				cancelPickup();
			};
			cancel_pickup_button.GestureRecognizers.Add(cancelPickupGesture);

			//Cancel Search tap functionality
			var cancelSearchGesture = new TapGestureRecognizer();
			cancelSearchGesture.Tapped += (s, e) =>
			{
				input_location_box.IsVisible = false;
				input_location_blackout_box.IsVisible = false;
				entered_pickup_location.Text = "";
			};
			cancel_location_search_button.GestureRecognizers.Add(cancelSearchGesture);

			//Confirm Search tap functionality
			var confirmSearchGesture = new TapGestureRecognizer();
			confirmSearchGesture.Tapped += (s, e) =>
			{
				confirmSearch(entered_pickup_location.Text);
			};
			search_for_location_button.GestureRecognizers.Add(confirmSearchGesture);

			//Dropoff location search tap functionality
			var dropoffSearchGesture = new TapGestureRecognizer();
			dropoffSearchGesture.Tapped += (s, e) =>
			{
				setDropoffLocation();
			};
			dropoff_location_search.GestureRecognizers.Add(dropoffSearchGesture);

			var confirmDropOff = new TapGestureRecognizer();
			confirmDropOff.Tapped += (s, e) =>
			{
				GenericSocket.CONFIRM_DROP_OFF(Settings.LastRideID);
			};

			var confirmArrival = new TapGestureRecognizer();
			confirmArrival.Tapped += (s, e) =>
			{
				GenericSocket.CONFIRM_ARRIVAL(Settings.LastRideID);
			};

			//PROFILE PICTURE STUFF
			var cancelPictureGesture = new TapGestureRecognizer();
			cancelPictureGesture.Tapped += (s, e) =>
			{
				switchPopMenu();
			};
			cancel_button.GestureRecognizers.Add(cancelPictureGesture);
			profile_pic_blackout_box.GestureRecognizers.Add(cancelPictureGesture);

			var takePicGesture = new TapGestureRecognizer();
			takePicGesture.Tapped += (s, e) =>
			{
				CameraCallback callback = new CameraCallback(processImage);
				DependencyService.Get<ICameraSystem>().TakePicture(callback);
			};
			take_pic_button.GestureRecognizers.Add(takePicGesture);

			var choosePicGesture = new TapGestureRecognizer();
			choosePicGesture.Tapped += (s, e) =>
			{
				CameraCallback callback = new CameraCallback(processImage);
				DependencyService.Get<ICameraSystem>().GetMediaPicture(callback);
			};
			choose_pic_button.GestureRecognizers.Add(choosePicGesture);

			var cancelDriverSearchGesture = new TapGestureRecognizer();
			cancelDriverSearchGesture.Tapped += (s, e) =>
			{
				cancelDriverSearch();
			};
			cancel_driver_search_button.GestureRecognizers.Add(cancelDriverSearchGesture);


			profile_pic_pin = new CircleImage { Source = Settings.GetImageFromDisk().Source };
			profile_pic.Source = Settings.GetImageFromDisk().Source;
			profile_pic_pin.WidthRequest = App.ScreenWidth * 0.12;
			profile_pic_pin.HeightRequest = App.ScreenWidth * 0.12;
			double pic_x = App.ScreenWidth * 0.4415;
			double pic_y = App.ScreenHeight * getPicYPosition();
			parentLayout.Children.Add(profile_pic_pin, Constraint.Constant(pic_x), Constraint.Constant(pic_y));

			//-----------------------End Gesture Recognizer Section----------------------------



			string path = "users/" + FireAuth.auth.User.LocalId + "/personal_info";

			//Set the User's name in the slide out menu
			var user_info = FireDatabase.read(path).Result;
			if (user_info != null && user_info.ContainsKey("name_first") && user_info.ContainsKey("name_last"))
			{
				user_name_label.Text = user_info["name_first"].ToString() + " " + user_info["name_last"].ToString();
			}

			//Set the User's phone number in the slide out menu
			if (user_info != null && user_info.ContainsKey("phone"))
			{
				user_phone_label.Text = String.Format("{0:(###) ###-####}", long.Parse(user_info["phone"].ToString()));

			}


			path = "users/" + FireAuth.auth.User.LocalId;
			var is_driver = FireDatabase.read(path).Result;
			Console.WriteLine("UPCOMING!");
			Console.WriteLine("IS DRIVER: " + is_driver.Values);
			if (is_driver.ContainsKey("is_driver") && is_driver["is_driver"].Equals(true))
			{
				driver_switch.IsVisible = true;
			}
			parentLayout.RaiseChild(map_pin);
			parentLayout.RaiseChild(input_location_blackout_box);
			parentLayout.RaiseChild(input_location_box);

			map.CustomPins = new System.Collections.ObjectModel.ObservableCollection<CustomPin>();
        }

		void Handle_DriverSwitch_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
		{
			// Called when the user toggles the driver switch, puts them into Driver mode if they are allowed to enter that state
			Console.WriteLine("TOGGLED THE DRIVER SWITCH!");
			if (driver_switch.IsToggled)
			{
				GenericSocket.REQUEST_DRIVER_MODE();
				DependencyService.Get<IRegisterDriverMode>().Register();
				//set_pickup_button.Is
			}
			else {
				DependencyService.Get<IRegisterDriverMode>().UnRegister();
				FireDatabase.write("users/" + FireAuth.auth.User.LocalId + "/stage_1_driving", null);
			}
		}

		public async void signOutButtonClicked(object sender, EventArgs e)
		{
			// Prompts the user if they want to sign out, then signs out if they press Yes
			var response = await DisplayAlert("Sign Out", "Are you sure you want to sign out?", "Yes", "No");
			if (response)
			{
				FireAuth.SignOut();
				await Navigation.PushModalAsync(new LaunchPage());
			}
		}
		
        public async void onButtonClicked(object sender, EventArgs e)
		{
			// Collapses the slide out menu
			drawer_button.IsVisible = !drawer_button.IsVisible;
			if (view.IsVisible == true)
			{
				await view_box.FadeTo(0, 260);
				await parentLayout.TranslateTo(0, 0, 250, Easing.SinIn);
				await view.TranslateTo(parentLayout.Width * -0.75, 0, 250, Easing.SinIn);
				await view.TranslateTo(0, 0, 0);
				view.IsVisible = false;
			}
			else 
			{
				view_box.Opacity = 0;
				view_box.IsVisible = true;
				await view.TranslateTo(parentLayout.Width * -0.75, 0, 0);
				view.IsVisible = true;
				await view_box.FadeTo(0.4, 260);
				await view.TranslateTo(0, 0, 250, Easing.SinIn);
				await parentLayout.TranslateTo(parentLayout.Width * 0.75, 0, 250, Easing.SinIn);
			}

            view_box.IsVisible = view.IsVisible;
        }

		public async void setPositionToCurrentLocation()
		{
			// Updates the position of the map to the current location reported by the device's GPS
			try
			{
				var locator = CrossGeolocator.Current;
				locator.DesiredAccuracy = 50;
				var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);
				map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), Distance.FromMiles(0.1)));
			}
			catch (Exception e)
			{
				await DisplayAlert("Location Unavailable", "Location is unavailable!", "OK");
			}
		}

		public Position getPinPosition()
		{
			// Gets the position of a pin and returns it as a Position object
			Position pos = map.VisibleRegion.Center;
			var pin = new Pin();
			if (inRide == false)
			{
				pin = new Pin() { Position = new Position(pos.Latitude, pos.Longitude), Label = "Pick Up Location" };
			}
			else
			{
				pin = new Pin() { Position = new Position(pos.Latitude, pos.Longitude), Label = "Drop-Off Location" };
			}
			addRegularPin(pin);

			map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(pos.Latitude, pos.Longitude), Distance.FromMiles(0.1)));
			map_pin.IsVisible = false;
			profile_pic_pin.IsVisible = false;
			compass_frame.IsVisible = false;
			search_button.IsVisible = false;
			set_pickup_button.IsVisible = false;
			cancel_pickup_button.IsVisible = true;
			confirm_location_button.IsVisible = true;
			return pos;
		}

		public void resetGUI()
		{
			// Resets the main page GUI to its default state
			map.Pins.Clear();
			confirm_location_button.IsVisible = false;
			cancel_pickup_button.IsVisible = false;
			rider_ui.IsVisible = false;
			driver_ui.IsVisible = false;
			compass_frame.IsVisible = true;
			search_button.IsVisible = true;
			set_pickup_button.IsVisible = true;
			map_pin.IsVisible = true;
			showSearch(false);
			profile_pic_pin.IsVisible = true;
			cancel_driver_search_button.IsVisible = false;
		}

		public void cancelPickup()
		{
			// Cancels the selection of the pickup location
			confirm_location_button.IsVisible = false;
			cancel_pickup_button.IsVisible = false;
			compass_frame.IsVisible = true;
			search_button.IsVisible = true;
			set_pickup_button.IsVisible = true;
			map_pin.IsVisible = true;
			profile_pic_pin.IsVisible = true;
			if (!inRide)
			{
				this.removePin("Pick Up Location");
			}
			else {
				this.removePin("Drop-Off Location");
			}
			entered_pickup_location.Text = "";
		}

		public void confirmPickup()
		{
			// Confirms the pickup location, hides rider UI elements relating to pickups, and moves onto the next form of the ride
			//SET PICKUP LOCATION
			if (inRide == false)
			{
				showSearch(true);
				confirm_location_button.IsVisible = false;
				cancel_pickup_button.IsVisible = false;
				var pin = map.getPin("Pick Up Location");
				Console.WriteLine("Latitude: " + pin.Position.Latitude + "   Longitude: " + pin.Position.Longitude);
				//rider_ui.IsVisible = true;
				inRide = true;
				GenericSocket.GET_DROVER(pin.Position.Latitude, pin.Position.Longitude);
				cancel_driver_search_button.IsVisible = true;
				//
			}

			//SET DROPOFF LOCATION
			else 
			{
				if (map.getPin("Drop-Off Location") != null)
				{
					confirm_location_button.IsVisible = false;
					cancel_pickup_button.IsVisible = false;
					var pin = map.getPin("Pick Up Location");
					var pin_2 = map.getPin("Drop-Off Location");
					rider_ui.IsVisible = true;
					set_pickup_label.Text = "Set Pickup Location";
					centerMapOnTwoPins(pin, pin_2);
					GenericSocket.CHANGE_DROP_OFF_LOCATION(pin_2.Position.Latitude, pin_2.Position.Longitude);
				}
			}
			confirm_location_button.IsVisible = false;
			cancel_pickup_button.IsVisible = false;
		}

		public async void confirmSearch(string locationToSearchFor)
		{
			// Confirms the rider's search and initiates it in the firebase and the server
			Geocoder gc = new Geocoder();
			var positions = await gc.GetPositionsForAddressAsync(locationToSearchFor);
			if (positions.Count() > 0)
			{
				map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(positions.First().Latitude, positions.First().Longitude), Distance.FromMiles(0.1)));
				input_location_box.IsVisible = false;
				input_location_blackout_box.IsVisible = false;
				entered_pickup_location.Text = "";
			}
			else {
				await DisplayAlert("Address Not Found", "The entered address could not be found. Please try being more specific", "OK");
			}
		}

		public void setDropoffLocation()
		{
			// Changes the riders drop off location to a new one
			rider_ui.IsVisible = false;
			set_pickup_label.Text = "Set Drop-Off Location";
			set_pickup_button.IsVisible = true;
			compass_frame.IsVisible = true;
			search_button.IsVisible = true;
			map_pin.IsVisible = true;
			profile_pic_pin.IsVisible = true;
			this.removePin("Drop-Off Location");
		}

		public void showSearch(bool to)
		{
			// Makes the "Searching" text visible on screen
			searching_view.IsVisible = to;
		}

		public void resetGuiByRyan()
		{
			// Ryan Shivers made this - not sure what it does
			showSearch(false);
			rider_ui.IsVisible = false;
			driver_ui.IsVisible = false;
			compass_frame.IsVisible = true;
			search_button.IsVisible = true;
			set_pickup_button.IsVisible = true;
			map.Pins.Clear();
		}

		public void enterRiderGUI()
		{
			// Hides the Driver GUI and enables the rider UI elements
			showSearch(false);
			compass_frame.IsVisible = false;
			search_button.IsVisible = false;
			set_pickup_button.IsVisible = false;
			rider_ui.IsVisible = true;
			//Task t = updateDriverLocation();
		}

		private Task updateDriverLocation()
		{
			// Immediately starts a loop for every 6 seconds that updates the drivers location asynchronously
			return Task.Factory.StartNew(() =>
			{
				Task.Delay(6000);

				while (Settings.InRide)
				{
					try
					{
						Console.WriteLine("Hello");
						var tracker = FireDatabase.read("rides/" + Settings.LastRideID + "/latest_tracker").Result;
						makeDriverPin(new Position((double)tracker["lat"], (double)tracker["lng"]));
					}
					catch (Exception e)
					{

					}
					Task.Delay(1000);
				}

			});
		}

		public void enterDriverGui()
		{
			// Setups the Driver GUI for when a driver enters Driver mode
			hideAll();
			driver_ui.IsVisible = true;
		}

		public void hideAll()
		{
			// Hides everything relating to the Rider UI
			rider_ui.IsVisible = false;
			driver_ui.IsVisible = false;
			compass_frame.IsVisible = false;
			search_button.IsVisible = false;
			set_pickup_button.IsVisible = false;
			searching_view.IsVisible = false;
		}

		//-----------------Profile Picture Stuff------------------
		public void processImage(Image img)
		{
			// Sets up variables for the profile picture to be used
			profile_pic.Source = img.Source;
			profile_pic_pin.Source = img.Source;
		}

		public void switchPopMenu()
		{
			// Switches the popup menu to a different one
			top_box.IsVisible = !top_box.IsVisible;
			cancel_button.IsVisible = !cancel_button.IsVisible;
			profile_pic_blackout_box.IsVisible = !profile_pic_blackout_box.IsVisible;
		}

		private double getPicYPosition()
		{
			// Gets the Y Position of a picture
			double yPOS = 0.420;

			if (App.ScreenHeight < 600)
			{
				yPOS = 0.417;
			}
			else if (App.ScreenHeight < 700)
			{
				yPOS = 0.415;
			}
			else if (App.ScreenHeight < 1100)
			{
				yPOS = 0.405;
			}
			else if (App.ScreenHeight < 1400)
			{
				yPOS = 0.4;
			}

			return yPOS;
		}

		private void makeDriverPin(Position pos)
		{
			// Creates a new Driver pin that will represent the location of a driver and adds it to the map
			var pin = new CustomPin
			{
				Pin = new Pin
				{
					Type = PinType.Generic,
					Position = pos,
					Label = "Driver"
				},
				isCustom = true,
				Id = "Driver"
			};
			Console.WriteLine("Making Driver Pin");
			map.CustomPins.Add(pin);
			map.Pins.Add(pin.Pin);
			centerMapOnTwoPins(map.getPin("Pick Up Location"), pin.Pin);
		}

		private void addRegularPin(Pin pin)
		{
			// Adds a new standard map pin to the map
			var custom_pin = new CustomPin
			{
				Pin = pin,
				isCustom = false,
				Id = pin.Label
			};
			map.CustomPins.Add(custom_pin);
			map.Pins.Add(pin);
		}

		private void removePin(string label)
		{
			// Removes a map pin from the map
			Pin pinToRemove = map.getPin(label);
			if (pinToRemove != null)
			{
				map.removePin(label);
				map.Pins.Remove(pinToRemove);
			}
		}

		private void changePinLocation(Pin pin, Position newLocation)
		{
			// Updates the pin location to a new location
			pin.Position = newLocation;
			MessagingCenter.Send<MainPage>(this, "updated pins");
		}

		private void centerMapOnTwoPins(Pin pin, Pin pin_2)
		{
			// Finds the center between two specified pin locations and puts the "camera" at that location
			double distanceBetweenPins = Math.Sqrt(Math.Pow((pin_2.Position.Latitude - pin.Position.Latitude), 2) + (Math.Pow(pin_2.Position.Longitude - pin.Position.Longitude, 2)));
			distanceBetweenPins = distanceBetweenPins * 68.073;
			Position midpoint = new Position(((pin_2.Position.Latitude + pin.Position.Latitude) / 2), ((pin_2.Position.Longitude + pin.Position.Longitude) / 2));
			map.MoveToRegion(MapSpan.FromCenterAndRadius(midpoint, Distance.FromMiles(distanceBetweenPins)));
		}

		private void cancelDriverSearch()
		{
			// Resets the GUI to cancel any search that may be in effect
			resetGUI();
		}
    }
}
