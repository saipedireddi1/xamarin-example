using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Firebase.Xamarin.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using Xamarin.Forms;

namespace DroverXamarin
{
	public partial class LaunchPage : ContentPage
	{
		public LaunchPage ()
		{
			InitializeComponent ();

			Console.WriteLine("TEST");
			Console.WriteLine("REFRESH TOKEN: " + Settings.RefreshToken);
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped +=  (s, e) =>
            {
                Navigation.PushModalAsync(new SignInPage());
            };
            sign_in_pic.GestureRecognizers.Add(tapGestureRecognizer);

            var tapGestureRecognizer2 = new TapGestureRecognizer();
            tapGestureRecognizer2.Tapped += (s, e) =>
            {
				string test = "0000000013areaofaSQUARE";
				var bits = Encoding.ASCII.GetBytes(test);
				//Console.WriteLine(bits.Length);
				//new GenericSocket(bits);

				//Navigation.PushModalAsync(new SignUpPage());
            };
            sign_up_button.GestureRecognizers.Add(tapGestureRecognizer2);

        }

    }
}
