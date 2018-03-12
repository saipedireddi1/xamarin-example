using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Firebase.Xamarin.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace DroverXamarin
{
	public static class FireAuth
	{

		public static FirebaseAuthLink auth = null;
		private static long expirationTime
		{
			get { return Settings.TokenExpirationTime; }
			set { Settings.TokenExpirationTime = value; }
		}


		public static Task LoginWithEmailAndPassword(string email, string password)
		{
			return Task.Factory.StartNew(() =>
			{
				try
				{
					var authProvider = new FirebaseAuthProvider(new FirebaseConfig(FireCore.FIREBASE_KEY));
					auth = authProvider.SignInWithEmailAndPasswordAsync(email, password).Result;

					Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
					{
						UIInstance.nav.PushModalAsync(new MainPage());
					});

					Console.WriteLine("First Token: "+auth.FirebaseToken);
					expirationTime = Time.CurrentTimeMillis() + (auth.ExpiresIn * 1000);
					Settings.RefreshToken = auth.RefreshToken;
					Settings.LastUserID = auth.User.LocalId;
					Settings.LastAuthToken = auth.FirebaseToken;
					//RenewAuthToken(auth.RefreshToken);
					if(!Settings.ApnsToken.Equals(string.Empty))
						FireDatabase.write("users/" + auth.User.LocalId + "/device_info/apns_token", Settings.ApnsToken);

				}
				catch (Exception e)
				{
					auth = null;
					Console.WriteLine("Bad Login Info! ");
					Console.WriteLine(e.StackTrace);
					Console.WriteLine(e.GetBaseException());
					Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
					{
						UIInstance.app.MainPage.DisplayAlert("ERROR","Unable to log you in. Check password or internet connection.", "OK");
					});
				}
				/*
				var firebase = new FirebaseClient("https://droverrideshare-69d73.firebaseio.com/");
				var dinos = firebase
					.Child("city-lookups");
				var query = QueryExtensions.WithAuth(dinos, auth.FirebaseToken).OnceAsync<Object>().Result;

				foreach (var item in query)
				{
					Console.WriteLine(item.Key + ", " + item.Object);
				}
				*/
			});
		}

		public static Task SignUpWithEmailAndPassword(string email, string password)
		{
			return Task.Factory.StartNew(() =>
			{
				try
				{
					var authProvider = new FirebaseAuthProvider(new FirebaseConfig(FireCore.FIREBASE_KEY));
					auth = authProvider.CreateUserWithEmailAndPasswordAsync(email, password).Result;

					Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
					{
						UIInstance.nav.PushModalAsync(new MainPage());
					});
					expirationTime = Time.CurrentTimeMillis() + (auth.ExpiresIn * 1000);
					Settings.RefreshToken = auth.RefreshToken;
					Settings.LastUserID = auth.User.LocalId;
					Settings.LastAuthToken = auth.FirebaseToken;
					if (!Settings.ApnsToken.Equals(string.Empty))
						FireDatabase.write("users/" + auth.User.LocalId + "/device_info/apns_token", Settings.ApnsToken);
				}
				catch (Exception e)
				{
					auth = null;
					//Console.WriteLine(e);
					Console.WriteLine("Bad Login Info!");
					Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
					{
						UIInstance.app.MainPage.DisplayAlert("ERROR", "Unable to Create Account. Check your email and internet.", "OK");
					});
				}

				/*
				var firebase = new FirebaseClient("https://droverrideshare-69d73.firebaseio.com/");
				var dinos = firebase
					.Child("city-lookups");
				var query = QueryExtensions.WithAuth(dinos, auth.FirebaseToken).OnceAsync<Object>().Result;

				foreach (var item in query)
				{
					Console.WriteLine(item.Key + ", " + item.Object);
				}
				*/
			});
		}

		public static Task InitFireAuth()
		{
			return Task.Factory.StartNew(() =>
			{
				//CHECK TO SEE IF WE HAVE A VALID ACCOUNT ON THIS DEVICE
				if (!Settings.RefreshToken.Equals(string.Empty))
				{
					//CHECK TO SEE IF OUR AUTH TOKEN NEEDS RENEWING
					if (isTokenExpired())
					//ATTEMPT TO RENEW OUR AUTH TOKEN	
					{
						bool success = RenewAuthToken(Settings.RefreshToken).Result;
						if (success)
						{
							//IF TOKEN WAS RENEWED. GO ONTO MAIN PAGE.
							Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
							{
								UIInstance.app.MainPage.Navigation.PushModalAsync(new MainPage());
							});							
							return;
						}
						//ELSE. CONTINUE TO PUSH THE LAUNCH PAGE.
					}
					//SEND US TO THE MAIN PAGE. WE ARE ALREADY LOGGED IN.
					else {
						if (!Settings.ApnsToken.Equals(string.Empty))
							FireDatabase.write("users/" + auth.User.LocalId + "/device_info/apns_token", Settings.ApnsToken);
						Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
						{
							UIInstance.app.MainPage.Navigation.PushModalAsync(new MainPage());
						});
						return;
					}
				}
				//SEND US TO THE LOGIN PAGE. WE ARE LOGGED OUT.
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
				{
					UIInstance.app.MainPage.Navigation.PushModalAsync(new LaunchPage());
				});

			});
		}

		public static Task<Boolean> RenewAuthToken(string refreshToken)
		{
			return Task<Boolean>.Factory.StartNew(() =>
			{
				string responseData = "N/A";
				HttpClient client = new HttpClient();

				try
				{
					var tokenPost = $"{{\"grant_type\":\"refresh_token\",\"refresh_token\":\"{refreshToken}\"}}";
					var response = client.PostAsync(new Uri("https://securetoken.googleapis.com/v1/token?key="+FireCore.FIREBASE_KEY), new StringContent(tokenPost, Encoding.UTF8, "application/json")).Result;
					responseData = response.Content.ReadAsStringAsync().Result;
					response.EnsureSuccessStatusCode();
					FirebaseAuthLink test = JsonConvert.DeserializeObject<FirebaseAuthLink>(responseData);
					Console.WriteLine("REFRESH TOKEN FROM: "+test.RefreshToken);
					var auth2 = JsonConvert.DeserializeObject<Dictionary<string, Object>>(responseData);

					//auth.FirebaseToken = auth2["access_token"].ToString();
					Settings.LastAuthToken = auth2["access_token"].ToString();
					//auth.RefreshToken = auth2["refresh_token"].ToString();
					Settings.RefreshToken = auth2["refresh_token"].ToString();
					var u = new User();
					u.LocalId = Settings.LastUserID;
					string stringData = $"{{\"localId\":\"{Settings.LastUserID}\", \"idToken\":\"{Settings.LastAuthToken}\"}}";

					FirebaseAuthLink tempAuth = JsonConvert.DeserializeObject<FirebaseAuthLink>(stringData);
					tempAuth.User = u;
					auth = tempAuth;
					expirationTime = Time.CurrentTimeMillis() + (long.Parse(auth2["expires_in"].ToString()) * 1000);
					if (!Settings.ApnsToken.Equals(string.Empty))
						FireDatabase.write("users/" + auth.User.LocalId + "/device_info/apns_token", Settings.ApnsToken);
					return true;
				}
				catch (Exception ex)
				{
					Console.WriteLine("ERROR");
					Console.WriteLine(ex);
					return false;
				}
			});
		}

		public static bool isTokenExpired()
		{
			return (DateTime.UtcNow.Millisecond >= expirationTime);
		}

		public static string getFirebaseToken()
		{
			if (isTokenExpired())
			{
				bool result = RenewAuthToken(Settings.RefreshToken).Result;
				if (result)
					return auth.FirebaseToken;
				else
					return "";
			}
			return auth.FirebaseToken;
		}

		public static void SignOut()
		{
			auth = null;
			expirationTime = 0;
			Settings.RefreshToken = string.Empty;
			Settings.LastUserID = string.Empty;
			Settings.LastAuthToken = string.Empty;
		}
	}
}
