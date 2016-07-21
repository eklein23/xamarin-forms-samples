using System;
using Xamarin.Forms;

namespace TodoAWSSimpleDB
{
	public partial class LoginPage : ContentPage
	{
		public LoginPage ()
		{
			InitializeComponent ();
		}

		void OnGoogleLoginClicked (object sender, EventArgs e)
		{
			// Use a custom renderer to display the authentication UI
			Navigation.PushModalAsync (new AuthenticationPage("Google"));			
		}

        void OnTwitterLoginClicked(object sender, EventArgs e)
        {
            // Use a custom renderer to display the authentication UI
            Navigation.PushModalAsync(new AuthenticationPage("Twitter"));
        }
    }
}
