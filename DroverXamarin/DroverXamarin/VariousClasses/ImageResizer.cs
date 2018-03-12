using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

// Usings je Platform

#if __IOS__
using System.Drawing;
using UIKit;
using CoreGraphics;
#endif


#if __ANDROID__
using Android.Graphics;
#endif

namespace DroverXamarin
{
	public static class ImageResizer
	{
		static ImageResizer()
		{
		}


		public static byte[] ResizeImage(byte[] imageData, float width, float height)
		{
#if __IOS__
			return ResizeImageIOS(imageData, width, height);
#endif
#if __ANDROID__
            return ResizeImageAndroid(imageData, width, height);
#endif
		}
		//
#if __IOS__

		public static byte[] ResizeImageIOS(byte[] imageData, float new_width, float new_height)
		{
			//Get the image from the byte array that was passed in
			UIImage originalImage = ImageFromByteArray(imageData);

			var original_height = originalImage.Size.Height;
			var original_width = originalImage.Size.Width;

			nfloat target_height = 0;
			nfloat target_width = 0;

			if (original_height > original_width)
			{
				target_height = new_height;
				nfloat splitter = original_height / new_height;
				target_width = original_width / splitter;
			}
			else
			{
				target_width = new_width;
				nfloat splitter = original_width / new_width;
				target_height = original_height / splitter;
			}

			new_width = (float)target_width;
			new_height = (float)target_height;

			UIGraphics.BeginImageContext(new SizeF(new_width, new_height));
			originalImage.Draw(new RectangleF(0, 0, new_width, new_height));
			var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			var bytesImagen = resizedImage.AsJPEG().ToArray();
			resizedImage.Dispose();
			return bytesImagen;
		}

		public static byte[] cropPicture(byte[] imageData)
		{
			// Load the bitmap
			UIImage originalImage = ImageFromByteArray(imageData);
			UIImage resizedImage;

			var h = originalImage.Size.Height;
			var w = originalImage.Size.Width;
			var l = Math.Min(h, w);
			var x = (w - l) / 2;
			var y = (h - l) / 2;

			using (CGImage cr = originalImage.CGImage.WithImageInRect(new CGRect(x, y, (w - x * 2), (h - y * 2))))
			{
				resizedImage = UIImage.FromImage(cr);
			}

			var bytesImagen = resizedImage.AsJPEG().ToArray();
			resizedImage.Dispose();
			return bytesImagen;
		}

		//
		private static UIKit.UIImage ImageFromByteArray(byte[] data)
		{
			if (data == null)
			{
				return null;
			}
			//
			UIKit.UIImage image;
			try
			{
				image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
			}
			catch (Exception e)
			{
				Console.WriteLine("Image load failed: " + e.Message);
				return null;
			}
			return image;
		}

#endif
		//
#if __ANDROID__
        public static byte[] ResizeImageAndroid(byte[] imageData, float width, float height)
        {
            // Load the bitmap 
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            //
            float ZielHoehe = 0;
            float ZielBreite = 0;
            //
            var Hoehe = originalImage.Height;
            var Breite = originalImage.Width;
            //
            if (Hoehe > Breite) // Höhe (71 für Avatar) ist Master
            {
                ZielHoehe = height;
                float teiler = Hoehe / height;
                ZielBreite = Breite / teiler;
            }
            else // Breite (61 für Avatar) ist Master
            {
                ZielBreite = width;
                float teiler = Breite / width;
                ZielHoehe = Hoehe / teiler;
            }
            //
            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)ZielBreite, (int)ZielHoehe, false);
            // 
            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                return ms.ToArray();
            }
        }
#endif
	}
}
