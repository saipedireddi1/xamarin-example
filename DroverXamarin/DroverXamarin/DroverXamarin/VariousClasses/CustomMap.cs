using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms.Maps;

namespace DroverXamarin
{
	public class CustomMap : Map
	{

		public ObservableCollection<CustomPin> CustomPins { get; set;}
		public CustomMap()
		{
		}

		public Pin getPin(string label)
		{
			foreach (var pin in this.CustomPins)
			{
				if (pin.Id == label)
				{
					return pin.Pin;
				}
			}
			return null;
		}

		public void removePin(string label)
		{
			foreach (var pin in this.CustomPins)
			{
				if (pin.Id == label)
				{
					CustomPins.Remove(pin);
					return;
				}
			}
		}
	}
}
