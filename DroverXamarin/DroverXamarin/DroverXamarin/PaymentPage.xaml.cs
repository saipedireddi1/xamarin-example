using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xamarin.Forms;

namespace DroverXamarin
{
	public partial class PaymentPage : ContentPage
	{
		public PaymentPage()
		{
			InitializeComponent();

			var assembly = typeof(FireCore).GetTypeInfo().Assembly;

			Stream stream = assembly.GetManifestResourceStream("DroverXamarin.iOS.BrainTree.html");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text += reader.ReadToEnd();
			}
			HtmlWebViewSource source = new HtmlWebViewSource();
			source.Html = text;
			payment_view.Source = source;

			//Back arrow tap functionality
			var backGesture = new TapGestureRecognizer();
			backGesture.Tapped += (s, e) =>
			{
				Navigation.PopModalAsync();
			};
			back_arrow_button.GestureRecognizers.Add(backGesture);
		}

		void Handle_Navigating(object sender, Xamarin.Forms.WebNavigatingEventArgs e)
		{
			string val = e.Url;
			Console.WriteLine(val);
			if (val.Contains("https://www.droverrideshare-")){
				Console.WriteLine(val);
				int index = val.IndexOf('-') + 1;
				int newIndex = val.Length-5;
				val = val.Substring(index, (newIndex - index));
				Console.WriteLine(val);
			}
		}
	}
}
