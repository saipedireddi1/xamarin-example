using System;
using System.Drawing;
using Xamarin.Forms;

namespace DroverXamarin
{
	public interface IImageConverter
	{
		public static byte[] convertImageToByteArray(IImageConverter instance, Image img);
	}
}
