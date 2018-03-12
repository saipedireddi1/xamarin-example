using System;
namespace DroverXamarin
{
	public class Instances
	{
		public static DroverLocation pickUpLocation = null;
		public static DroverLocation dropOffLocation = null;
		public static void Init()
		{
			pickUpLocation = new DroverLocation("Unknown Location Name", 0, 0);
			dropOffLocation = new DroverLocation("Unknown Location Name", 0, 0);
		}

		public static DroverLocation PickUpLocation
		{
			set
			{
				pickUpLocation = value;
				//DroverViewModels.MainPageViewModel.instance.PickUpLocation = pickUpLocation.locationName;
			}
			get
			{
				return pickUpLocation;
			}
		}

		public static DroverLocation DropOffLocation
		{
			set
			{
				dropOffLocation = value;
				DroverViewModels.MainPageViewModel.instance.DropOffLocation = dropOffLocation.locationName;
				Console.WriteLine("SETTING DROP OFF LOCATION!");
			}
			get
			{
				return dropOffLocation;
			}
		}
	}
}
