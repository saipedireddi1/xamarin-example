using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
			
			// Adds a back button
            var backButtonGesture = new TapGestureRecognizer();
            backButtonGesture.Tapped += (s, e) =>
            {
                Navigation.PopModalAsync();
            };
            back_arrow_button.GestureRecognizers.Add(backButtonGesture);

			// Sanity checks so that the user only enters proper information
            var signUpButton = new TapGestureRecognizer();
            signUpButton.Tapped += (s, e) =>
            {
				string phone_string = Regex.Replace(phone.Text, @"[^0-9]", "");
				if (String.IsNullOrEmpty(first_name.Text)){
                    DisplayAlert("First Name Required", "Please enter a first name ", "OK");
				}
				if (String.IsNullOrEmpty(last_name.Text)){
                    DisplayAlert("Last Name Required", "Please enter a last name ", "OK");
				}    
				if (String.IsNullOrEmpty(phone.Text)){
                    DisplayAlert("Phone Number Required", "Please enter a valid phone number", "OK");
				}
				if (phone.Text.Trim('-').Length<10)
				{
                    DisplayAlert("Phone Number Required", "Please enter a valid phone number", "OK");
				}
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
					FireAuth.SignUpWithEmailAndPassword(email_entry.Text, password_again.Text, first_name.Text, last_name.Text, (long)Int64.Parse(phone_string));
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
