using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace DroverXamarin
{
	public class RideStack : StackLayout
	{
		ObservableCollection<RideListEntry> rides;

		public RideStack()
		{
			this.WidthRequest = App.ScreenWidth;
		}

		public void addRide(RideListEntry entry)
		{
			this.Children.Add(entry);
			BoxView separator = new BoxView();
			separator.HeightRequest = 2;
			separator.Color = Color.FromHex("#e2e2e2");
			this.Children.Add(separator);
		}

	}
}
