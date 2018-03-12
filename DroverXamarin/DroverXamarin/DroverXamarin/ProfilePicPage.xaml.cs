using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using Xamarin.Forms;

namespace DroverXamarin
{
	public partial class ProfilePicPage : ContentPage
	{
		readonly static Image image = new Image();


		public ProfilePicPage()
		{
			InitializeComponent();

			var backButtonGesture = new TapGestureRecognizer();
			backButtonGesture.Tapped += (s, e) =>
			{
				Navigation.PopModalAsync();
			};
			back_arrow_button.GestureRecognizers.Add(backButtonGesture);

			var blackoutGesture = new TapGestureRecognizer();
			blackoutGesture.Tapped += (s, e) =>
			{
				switchPopMenu();
			};
			profile_pic_blackout_box.GestureRecognizers.Add(blackoutGesture);

			var cancelGesture = new TapGestureRecognizer();
			cancelGesture.Tapped += (s, e) =>
			{
				switchPopMenu();
			};
			cancel_button.GestureRecognizers.Add(cancelGesture);

			var picTapGesture = new TapGestureRecognizer();
			picTapGesture.Tapped += (s, e) =>
			{
				switchPopMenu();
			};
			profile_pic_edit_button.GestureRecognizers.Add(picTapGesture);

			var takePicGesture = new TapGestureRecognizer();
			takePicGesture.Tapped += (s, e) =>
			{
				CameraCallback callback = new CameraCallback(processImage);
				DependencyService.Get<ICameraSystem>().TakePicture(callback);
				profile_pic.Source = image.Source;
			};
			take_pic_button.GestureRecognizers.Add(takePicGesture);

			var choosePicGesture = new TapGestureRecognizer();
			choosePicGesture.Tapped += (s, e) =>
			{
				CameraCallback callback = new CameraCallback(processImage);
				DependencyService.Get<ICameraSystem>().GetMediaPicture(callback);
			};
			choose_pic_button.GestureRecognizers.Add(choosePicGesture);

		}

		public void processImage(Image img)
		{
			image.Source = img.Source;
			profile_pic.Source = img.Source;
		}

		public void switchPopMenu()
		{
			top_box.IsVisible = !top_box.IsVisible;
			cancel_button.IsVisible = !cancel_button.IsVisible;
			profile_pic_blackout_box.IsVisible = !profile_pic_blackout_box.IsVisible;
		}


	}
}
