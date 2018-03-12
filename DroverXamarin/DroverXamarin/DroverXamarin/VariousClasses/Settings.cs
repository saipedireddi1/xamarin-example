using System;
using System.Drawing;
using System.IO;
using System.Text;
using PCLStorage;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Xamarin.Forms;

namespace DroverXamarin
{
	public static class Settings
	{
		private static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}

		#region Setting Constants

		private const string SettingsKey = "settings_key";
		private static readonly string SettingsDefault = string.Empty;

		private const string RefreshKey = "refresh_key";
		private static readonly string RefreshDefault = "test_default";

		private const string TokenExpKey = "token_exp_key";
		private static readonly long TokenExpDefault = 0;

		private const string LastUserIDKey = "last_user_id_key";
		private static readonly string LastUserIDDefault = string.Empty;

		private const string LastAuthTokenKey = "last_auth_token_key";
		private static readonly string LastAuthTokenIDDefault = string.Empty;

		private const string ApnsTokenKey = "apns_token_key";
		private static readonly string ApnsTokenDefault = string.Empty;

		private const string AppStateKey = "app_state_key";
		private static readonly string AppStateDefault = "DEFAULT";

		private const string LastRideIDKey = "last_ride_id_key";
		private static readonly string LastRideIDDefault = string.Empty;

		private const string InRideKey = "in_ride_key";
		private static readonly bool InRideDefault = false;

		#endregion

		public static void SaveImageToDisk(Image profile_pic)
		{
			var uri = profile_pic.Source.GetValue(UriImageSource.UriProperty);
			var uriString = uri.ToString();

			IFileSystem fileSystem = FileSystem.Current;
			IFolder rootFolder = fileSystem.LocalStorage;
			IFile profilePicture = (PCLStorage.IFile)rootFolder.CreateFileAsync("profile_picture.png", CreationCollisionOption.ReplaceExisting);
			string pic_path = profilePicture.Path;

			//File.WriteAllBytes(pic_path, image_data);
			File.WriteAllText(pic_path, uriString);
		}

		public static Image GetImageFromDisk()
		{
			var filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "profile_pic.jpg");
			Image profile_pic = new Image { Source = "profile_missing.jpg" };
			/*
			var profile_pic = new Image { Source = "profile_missing.jpg" };
			if (File.Exists(filepath))
			{
				profile_pic = new Image { Source = ImageSource.FromFile(filepath) };
			}
			*/
			if (File.Exists(filepath))
			{
				profile_pic = DependencyService.Get<ICameraSystem>().ShowImage(filepath);
			}
			return profile_pic;
		}

		public static string LastRideID
		{
			get { return AppSettings.GetValueOrDefault(LastRideIDKey, LastRideIDDefault); }
			set { AppSettings.AddOrUpdateValue(LastRideIDKey, value); }
		}

		public static string AppState
		{
			get { return AppSettings.GetValueOrDefault(AppStateKey, AppStateDefault); }
			set { AppSettings.AddOrUpdateValue(AppStateKey, value); }
		}

		public static string ApnsToken
		{
			get { return AppSettings.GetValueOrDefault(ApnsTokenKey, ApnsTokenDefault); }
			set { AppSettings.AddOrUpdateValue(ApnsTokenKey, value); }			
		}

		public static string LastAuthToken
		{
			get { return AppSettings.GetValueOrDefault(LastAuthTokenKey, LastAuthTokenIDDefault); }
			set { AppSettings.AddOrUpdateValue(LastAuthTokenKey, value); }
		}

		public static string LastUserID
		{
			get { return AppSettings.GetValueOrDefault(LastUserIDKey, LastUserIDDefault); }
			set { AppSettings.AddOrUpdateValue(LastUserIDKey, value); }
		}

		public static string GeneralSettings
		{
			get{ return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault); }
			set{ AppSettings.AddOrUpdateValue(SettingsKey, value);}
		}

		public static string RefreshToken
		{
			get { return AppSettings.GetValueOrDefault(RefreshKey, RefreshDefault); }
			set { AppSettings.AddOrUpdateValue(RefreshKey, value); }
		}

		public static long TokenExpirationTime
		{
			get { return AppSettings.GetValueOrDefault(TokenExpKey, TokenExpDefault); }
			set { AppSettings.AddOrUpdateValue(TokenExpKey, value); }
		}

		public static bool InRide
		{
			get { return AppSettings.GetValueOrDefault(InRideKey, InRideDefault); }
			set { AppSettings.AddOrUpdateValue(InRideKey, value); }
		}

	}
}
