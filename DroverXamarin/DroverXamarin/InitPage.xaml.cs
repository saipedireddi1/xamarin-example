using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xamarin.Forms;

namespace DroverXamarin
{
	public partial class InitPage : ContentPage
	{
		public InitPage()
		{
			// Setup the firebase database, show an animation that the app is loading, and push the main page asynchronously as a modal
			InitializeComponent();
			UIInstance.nav = Navigation;
			FireDatabase.initDatabase();
			Instances.Init();
			var assembly = typeof(FireCore).GetTypeInfo().Assembly;

			Stream stream = assembly.GetManifestResourceStream("DroverXamarin.iOS.animation.html");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text += reader.ReadToEnd();
			}
			//HtmlWebViewSource source = new HtmlWebViewSource();
			//source.Html = text;
			//splash_screen.Source = source;
			//DependencyService.Get<IListenForRideProposal>().Listen();
			//FireAuth.LoginWithEmailAndPassword("d@d.com", "Password_10");
			Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
					{
				UIInstance.nav.PushModalAsync(new LaunchPage());
					} );
			//FireAuth.LoginWithEmailAndPassword("dhall@drover.email", "Password_10");
			//var test = assembly.GetManifestResourceStream("DroverXamarin.iOS.animation.html");
			//Console.WriteLine("Stream: "+test+" isNull? "+(test==null).ToString()+" assembly: "+assembly);
			//foreach (var res in assembly.GetManifestResourceNames())
			//	Console.WriteLine("found resource: " + res);
		}
	}
}
