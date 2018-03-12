using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace DroverViewModels
{
	class MainPageViewModel : INotifyPropertyChanged
	{
		public String dropOffLocation = "Unknown Pickup Location";
		public String pickUpLocation = "Unknown Pickup Location";

		public static MainPageViewModel instance;

		public event PropertyChangedEventHandler PropertyChanged;
		//public event PropertyChangedEventHandler PropertyChanged_2;

		public MainPageViewModel()
		{
			instance = this;
			//DropOffLocation = "Unknown";
			PropertyChanged += test;
		}

		public void test(object sender, EventArgs e)
		{
			Console.WriteLine("Property Changed in test");
			Console.WriteLine(DropOffLocation);
		}

		public String DropOffLocation
		{
			set
			{
				dropOffLocation = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this,
						new PropertyChangedEventArgs("DropOffLocation"));
				}
			}
			get
			{
				return dropOffLocation;
			}
		}
		/*
		public String PickUpLocation
		{
			set
			{
				pickUpLocation = value;
				if (PropertyChanged_2 != null)
				{
					PropertyChanged_2(this,
						new PropertyChangedEventArgs("PickUpLocation"));
				}
			}
			get
			{
				return pickUpLocation;
			}
		}
		*/
	}
}