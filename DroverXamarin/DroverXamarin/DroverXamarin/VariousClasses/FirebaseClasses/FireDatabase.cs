using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;

namespace DroverXamarin
{
	public class FireDatabase
	{
		private static FirebaseClient firebase = null;
		public static string currentCity = "COOKEVILLE_TN";
		public FireDatabase()
		{
			
		}

		public static Task initDatabase()
		{
			return Task.Factory.StartNew(() =>
			{
				firebase = new FirebaseClient(FireCore.FIREBASE_URL);
			});
		}

		public static bool isDatabaseInit()
		{
			return firebase != null;
		}

		public  static Task<Dictionary<string, Object>> read(string path)
		{
			path = WebUtility.UrlEncode(path);
			return Task<Dictionary<string, Object>>.Factory.StartNew(() =>
			{
				try
				{
					if (FireAuth.isTokenExpired())
					{
						FireAuth.RenewAuthToken(Settings.RefreshToken).RunSynchronously();
					}
						
						var reference = firebase.Child(path);
						var authReference = QueryExtensions.WithAuth(reference, FireAuth.auth.FirebaseToken);
						var result = authReference.OnceSingleAsync<Dictionary<string, Object>>().Result;
						return result;
					
				}
				catch (Exception e)
				{
					Console.WriteLine("FireDatabase.cs:read(): ERROR: " + e.GetBaseException());
					Console.WriteLine(path);
					return new Dictionary<string, Object>();
				}
			});


		}

		public static Task write(string path, object value) 
		{
			path = WebUtility.UrlEncode(path);
			return Task.Factory.StartNew( async () =>
			{
				try
				{
					if (FireAuth.isTokenExpired())
					{
						Console.WriteLine("TOKEN WAS EXPIRED WHEN WRITING");
						FireAuth.RenewAuthToken(Settings.RefreshToken).RunSynchronously();
					}
					var query = firebase.Child(path);
					await QueryExtensions.WithAuth(query, FireAuth.auth.FirebaseToken).PutAsync(value);
						//.PutAsync(value);
				}
				catch (Exception e)
				{
					Console.WriteLine("FireDatabase.cs:write(): ERROR: " + e.GetBaseException());
					Console.WriteLine("ERROR ON: " + path);
				}
			});
		}

		public void subscribe()
		{

		}

	}
}
