using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace TodoAWSSimpleDB
{
	public partial class AuthenticationPage : ContentPage
	{
        public string Provider { get; private set; }

		public AuthenticationPage (string provider)
		{
			// A custom renderer is used to display the authentication UI
			InitializeComponent ();

            Provider = provider;
		}
	}
}
