using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace DroverXamarin
{
	public partial class SignInPage : ContentPage
	{
		public SignInPage ()
		{
			InitializeComponent ();

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
				FireAuth.LoginWithEmailAndPassword(email_field.Text, password_field.Text);
                //Navigation.PushModalAsync(new MainPage());
            };
            sign_in_button.GestureRecognizers.Add(tapGestureRecognizer);

            var backButtonGesture = new TapGestureRecognizer();
            backButtonGesture.Tapped += (s, e) =>
            {
                Navigation.PopModalAsync();
            };
            back_arrow_button.GestureRecognizers.Add(backButtonGesture);
        }
	}
}
