using System;

using Xamarin.Forms;

namespace DroverXamarin
{
	public class RideListPage : ContentPage
	{
		public RideListPage()
		{
			// This is presumably unused/unfinished
			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Hello ContentPage" }
				}
			};
		}
	}
}

