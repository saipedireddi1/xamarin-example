using System;
using Xamarin.Forms;

namespace DroverXamarin
{
	public delegate void CameraCallback(Image img);
	public interface ICameraSystem
	{
		void TakePicture(CameraCallback callback);

		void GetMediaPicture(CameraCallback callback);

		Image ShowImage(string filepath);
	}

}
