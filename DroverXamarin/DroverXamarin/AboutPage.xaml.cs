using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace DroverXamarin
{
	public partial class AboutPage : ContentPage
	{
		public AboutPage()
		{
			InitializeComponent();

			// Creates a back button gesture recognizer so the user can go back if needed
			var backButtonGesture = new TapGestureRecognizer();
			backButtonGesture.Tapped += (s, e) =>
			{
				Navigation.PopModalAsync();
			};
			
			// Adds the back button gesture recognizer to the project
			back_arrow_button.GestureRecognizers.Add(backButtonGesture);

		}

		void webOnNavigating(object sender, WebNavigatingEventArgs e)
		{
			Console.WriteLine(sender.ToString() + ", " + e.Url);
		}
	}
}
