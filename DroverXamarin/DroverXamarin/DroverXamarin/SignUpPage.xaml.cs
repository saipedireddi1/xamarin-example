using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace DroverXamarin
{
	public partial class SignUpPage : ContentPage
	{
		public SignUpPage ()
		{
			InitializeComponent ();
            var backButtonGesture = new TapGestureRecognizer();
            backButtonGesture.Tapped += (s, e) =>
            {
                Navigation.PopModalAsync();
            };
            back_arrow_button.GestureRecognizers.Add(backButtonGesture);

            var signUpButton = new TapGestureRecognizer();
            signUpButton.Tapped += (s, e) =>
            {

				if (String.IsNullOrEmpty(email_entry.Text))
				{
					DisplayAlert("Blank E-Mail Field", "E-Mail field was left empty.", "OK");
				}
				if (String.IsNullOrEmpty(password.Text))
				{
					DisplayAlert("Blank Password Field", "Password field 1 was left empty.", "OK");
				}
				else if (String.IsNullOrEmpty(password_again.Text))
				{
					DisplayAlert("Blank Password Field", "Password field 2 was left empty.", "OK");
				}
                else if (password.Text.Equals(password_again.Text))
                {
					FireAuth.SignUpWithEmailAndPassword(email_entry.Text, password_again.Text);
					//Navigation.PushModalAsync(new MainPage());
                }
                else
                {
                    DisplayAlert("Passwords Do Not Match!", "Your passwords do not match.", "OK");
                }
            };
            sign_in_button.GestureRecognizers.Add(signUpButton);
        }
	}
}
