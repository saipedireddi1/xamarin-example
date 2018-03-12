using System;
using Xamarin.Forms;
using UIKit;
using System.Drawing;

namespace DroverXamarin.iOS
{

	public class IOSImageConverter : IImageConverter
	{
		byte[] IImageConverter.convertImageToByteArray(IImageConverter instance, Image img)
		{
			UIImage img2 = new UIImage();
			byte[] byteArray = UIKit.UIImage.FromImage((CoreGraphics.CGImage)img);
		}
	}
}
