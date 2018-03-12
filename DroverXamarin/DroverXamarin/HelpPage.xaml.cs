using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace DroverXamarin
{
	public partial class HelpPage : ContentPage
	{
		public HelpPage ()
		{
			InitializeComponent ();


			// Adds a back button gesture recognizer
			var backButtonGesture = new TapGestureRecognizer();
			backButtonGesture.Tapped += (s, e) =>
			{
				Navigation.PopModalAsync();
			};
			back_arrow_button.GestureRecognizers.Add(backButtonGesture);

		}
	}
}
