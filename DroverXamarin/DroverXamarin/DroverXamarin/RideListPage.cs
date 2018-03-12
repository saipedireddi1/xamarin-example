using System;

using Xamarin.Forms;

namespace DroverXamarin
{
	public class RideListPage : ContentPage
	{
		public RideListPage()
		{
			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Hello ContentPage" }
				}
			};
		}
	}
}

