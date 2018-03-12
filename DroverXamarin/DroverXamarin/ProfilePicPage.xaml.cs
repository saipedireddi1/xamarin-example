using System.Drawing;
using Xamarin.Forms;

namespace DroverXamarin
{
	public partial class ProfilePicPage : ContentPage
	{
		readonly static Xamarin.Forms.Image image = new Xamarin.Forms.Image();


		public ProfilePicPage()
		{
			InitializeComponent();
			
			// Create a bunch of tap gesture recognizers to form the UI for the profile picture page
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

		public void processImage(Xamarin.Forms.Image img)
		{
			// Makes sure the image is valid and sets up the variables necessary to display it
			image.Source = img.Source;
			profile_pic.Source = img.Source;
		}

		public void switchPopMenu()
		{
			// Switches the popup menu
			top_box.IsVisible = !top_box.IsVisible;
			cancel_button.IsVisible = !cancel_button.IsVisible;
			profile_pic_blackout_box.IsVisible = !profile_pic_blackout_box.IsVisible;
		}


	}
}
