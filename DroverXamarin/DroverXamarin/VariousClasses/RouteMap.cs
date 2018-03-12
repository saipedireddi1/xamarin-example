using System;
using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace DroverXamarin
{
	public class RouteMap : Map
	{
		public List<Position> RouteCoordinates { get; set; }

		public RouteMap()
		{
			RouteCoordinates = new List<Position>();
		}
	}
}
