using System;
using Xamarin.Forms;
using DroverXamarin.iOS;
using UIKit;
using Foundation;
using System.IO;
//using System.Drawing;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using System.Drawing;

[assembly: Xamarin.Forms.Dependency(typeof(IOSCameraSystem))]
namespace DroverXamarin.iOS
{
	public class IOSCameraSystem : ICameraSystem
	{
		public IOSCameraSystem()
		{
		}

		public void GetMediaPicture(CameraCallback callback)
		{
			if (UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.PhotoLibrary))
			{
				var imagePicker = new UIImagePickerController { SourceType = UIImagePickerControllerSourceType.PhotoLibrary };



				//imagePicker.Canceled += (sender, e) => UIApplication.SharedApplication.KeyWindow.RootViewController.DismissViewController(true, null);
				//.SharedApplication.KeyWindow.RootViewController.PresentViewController(imagePicker, true, null);
				var window = UIApplication.SharedApplication.KeyWindow;
				var vc = window.RootViewController;
				while (vc.PresentedViewController != null)
				{
					vc = vc.PresentedViewController;
				}
				vc.PresentViewController(imagePicker, true, null);
				imagePicker.Canceled += (sender, e) => vc.DismissViewController(true, null);
				imagePicker.FinishedPickingMedia += (sender, e) =>
				{
					var filepath = Path.Combine(Environment.GetFolderPath(
									   Environment.SpecialFolder.MyDocuments), "profile_pic.jpg");
					var image = (UIImage)e.Info.ObjectForKey(new NSString("UIImagePickerControllerOriginalImage"));


					image.AsJPEG().Save(filepath, false);
					callback(ShowImage(filepath));
					vc.DismissViewController(true, null);
				};
			}
			else {
				Console.WriteLine("NO CAMERA");
			}
		}

		public void TakePicture(CameraCallback callback)
		{
			if (UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera))
			{
				var imagePicker = new UIImagePickerController { SourceType = UIImagePickerControllerSourceType.Camera };

				var window = UIApplication.SharedApplication.KeyWindow;
				var vc = window.RootViewController;
				while (vc.PresentedViewController != null)
				{
					vc = vc.PresentedViewController;
				}
				vc.PresentViewController(imagePicker, true, null);
				imagePicker.Canceled += (sender, e) => vc.DismissViewController(true, null);
				imagePicker.FinishedPickingMedia += (sender, e) =>
				{
					var filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "profile_pic.jpg");
					var image = (UIImage)e.Info.ObjectForKey(new NSString("UIImagePickerControllerOriginalImage"));
					/*
					UIImageView imageView = new UIImageView(new CGRect(0,0,image.Size.Width,image.Size.Height));
					imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
					imageView.Image = image;
					imageView.Transform = CGAffineTransform.MakeRotation((float)Math.PI / 4);
					image = imageView.Image;
					*/
					image = ScaleAndRotateImage(image, UIImageOrientation.Right);


					image.AsJPEG().Save(filepath, false);
					callback(ShowImage(filepath));
					vc.DismissViewController(true, null);
				};
			}
			else {
				Console.WriteLine("NO CAMERA");
			}
		}

		public Xamarin.Forms.Image ShowImage(string filepath)
		{
			Xamarin.Forms.Image img = (new Xamarin.Forms.Image());														//Create a new Image
			byte[] imageByteArray = File.ReadAllBytes(filepath);							//Make a byte array based on file specified
			imageByteArray = ImageResizer.cropPicture(imageByteArray);						//Crop the image to a square
			imageByteArray = ImageResizer.ResizeImage(imageByteArray,256,256);  			//Resize the image to 256x256
			img.Source = ImageSource.FromStream(() => new MemoryStream(imageByteArray));	//Set and return the image
			return img;
		}

		public byte[] GetBitmapFromCache(string fileName)
		{
			return File.ReadAllBytes(fileName);
		}

		UIImage ScaleAndRotateImage(UIImage imageIn, UIImageOrientation orIn)
		{
			int kMaxResolution = 2048;

			CGImage imgRef = imageIn.CGImage;
			float width = imgRef.Width;
			float height = imgRef.Height;
			CGAffineTransform transform = CGAffineTransform.MakeIdentity();
			System.Drawing.RectangleF bounds = new System.Drawing.RectangleF(0, 0, width, height);

			if (width > kMaxResolution || height > kMaxResolution)
			{
				float ratio = width / height;

				if (ratio > 1)
				{
					bounds.Width = kMaxResolution;
					bounds.Height = bounds.Width / ratio;
				}
				else
				{
					bounds.Height = kMaxResolution;
					bounds.Width = bounds.Height * ratio;
				}
			}

			float scaleRatio = bounds.Width / width;
			System.Drawing.SizeF imageSize = new System.Drawing.SizeF(width, height);
			UIImageOrientation orient = orIn;
			float boundHeight;

			switch (orient)
			{
				case UIImageOrientation.Up:                                        //EXIF = 1
					transform = CGAffineTransform.MakeIdentity();
					break;

				case UIImageOrientation.UpMirrored:                                //EXIF = 2
					transform = CGAffineTransform.MakeTranslation(imageSize.Width, 0f);
					transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
					break;

				case UIImageOrientation.Down:                                      //EXIF = 3
					transform = CGAffineTransform.MakeTranslation(imageSize.Width, imageSize.Height);
					transform = CGAffineTransform.Rotate(transform, (float)Math.PI);
					break;

				case UIImageOrientation.DownMirrored:                              //EXIF = 4
					transform = CGAffineTransform.MakeTranslation(0f, imageSize.Height);
					transform = CGAffineTransform.MakeScale(1.0f, -1.0f);
					break;

				case UIImageOrientation.LeftMirrored:                              //EXIF = 5
					boundHeight = bounds.Height;
					bounds.Height = bounds.Width;
					bounds.Width = boundHeight;
					transform = CGAffineTransform.MakeTranslation(imageSize.Height, imageSize.Width);
					transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
					transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI / 2.0f);
					break;

				case UIImageOrientation.Left:                                      //EXIF = 6
					boundHeight = bounds.Height;
					bounds.Height = bounds.Width;
					bounds.Width = boundHeight;
					transform = CGAffineTransform.MakeTranslation(0.0f, imageSize.Width);
					transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI / 2.0f);
					break;

				case UIImageOrientation.RightMirrored:                             //EXIF = 7
					boundHeight = bounds.Height;
					bounds.Height = bounds.Width;
					bounds.Width = boundHeight;
					transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
					transform = CGAffineTransform.Rotate(transform, (float)Math.PI / 2.0f);
					break;

				case UIImageOrientation.Right:                                     //EXIF = 8
					boundHeight = bounds.Height;
					bounds.Height = bounds.Width;
					bounds.Width = boundHeight;
					transform = CGAffineTransform.MakeTranslation(imageSize.Height, 0.0f);
					transform = CGAffineTransform.Rotate(transform, (float)Math.PI / 2.0f);
					break;

				default:
					throw new Exception("Invalid image orientation");
			}

			UIGraphics.BeginImageContext(bounds.Size);

			CGContext context = UIGraphics.GetCurrentContext();

			if (orient == UIImageOrientation.Right || orient == UIImageOrientation.Left)
			{
				context.ScaleCTM(-scaleRatio, scaleRatio);
				context.TranslateCTM(-height, 0);
			}
			else
			{
				context.ScaleCTM(scaleRatio, -scaleRatio);
				context.TranslateCTM(0, -height);
			}

			context.ConcatCTM(transform);
			context.DrawImage(new RectangleF(0, 0, width, height), imgRef);

			UIImage imageCopy = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return imageCopy;
		}
	}
}
