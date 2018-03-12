using System;
namespace DroverXamarin
{
	public class ScreenSize
	{
		public int sh;
		public int sw;

		public ScreenSize()
		{
			this.sh = App.ScreenHeight;
			this.sw = App.ScreenWidth;
		}
	}
}
