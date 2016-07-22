using Android.App;
using Newtonsoft.Json;
using System;
using System.Linq;
using TodoAWSSimpleDB;
using TodoAWSSimpleDB.Droid;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer (typeof(AuthenticationPage), typeof(AuthenticationPageRenderer))]

namespace TodoAWSSimpleDB.Droid
{
	// Use a custom page renderer to display the authentication UI on the AuthenticationPage
	public class AuthenticationPageRenderer : PageRenderer
	{
		bool isShown;

		protected override void OnElementChanged (ElementChangedEventArgs<Page> e)
		{
			base.OnElementChanged (e);

            AuthenticationPage page = (AuthenticationPage)Element;
            string provider = page.Provider;

			// Retrieve any stored account information
			var accounts = AccountStore.Create (Forms.Context).FindAccountsForService (App.AppName);
			//var account = accounts.FirstOrDefault();

            var account = accounts.Where(x => x.Properties["provider"] == provider).FirstOrDefault();

			if (account == null)
            {
				if (!isShown)
                {
					isShown = true;

                    Authenticator auth = null;

                    if (provider == "Google")
                    {
                        // Initialize the object that communicates with the OAuth service
                        auth = new OAuth2Authenticator(
                                       Constants.ClientId,
                                       Constants.ClientSecret,
                                       Constants.Scope,
                                       new Uri(Constants.AuthorizeUrl),
                                       new Uri(Constants.RedirectUrl),
                                       new Uri(Constants.AccessTokenUrl));
                    }
                    else if (provider == "Twitter")
                    {
                        auth = new OAuth1Authenticator(
    consumerKey: "qAAkFcAEH2gHMsd1jf4dPYNUH",
    consumerSecret: "QlQY0lvJZbQzteQu4qyiiFBW8CINP8PA163wd2qQi0SBJTFw6D",
    requestTokenUrl: new Uri("https://api.twitter.com/oauth/request_token"),
    authorizeUrl: new Uri("https://api.twitter.com/oauth/authorize"),
    accessTokenUrl: new Uri("https://api.twitter.com/oauth/access_token"),
    callbackUrl: new Uri("http://mobile.twitter.com")
);
                    }

					// Register an event handler for when the authentication process completes
					auth.Completed += OnAuthenticationCompleted;

					// Display the UI
					var activity = Context as Activity;
					activity.StartActivity (auth.GetUI (activity));
				}
			}
            else
            {
				if (!isShown)
                {
					App.User.Email = account.Username;
					App.SuccessfulLoginAction.Invoke ();
				}
			}
		}

		async void OnAuthenticationCompleted (object sender, AuthenticatorCompletedEventArgs e)
		{
            AuthenticationPage page = (AuthenticationPage)Element;
            string provider = page.Provider;

            if (e.IsAuthenticated)
            {
                if (provider == "Google")
                {
                    // If the user is authenticated, request their basic user data from Google
                    // UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
                    var request = new OAuth2Request("GET", new Uri(Constants.UserInfoUrl), null, e.Account);
                    var response = await request.GetResponseAsync();
                    if (response != null)
                    {
                        // Deserialize the data and store it in the account store
                        // The users email address will be used to identify data in SimpleDB
                        string userJson = response.GetResponseText();
                        App.User = JsonConvert.DeserializeObject<User>(userJson);
                        e.Account.Username = App.User.Email;
                        e.Account.Properties["provider"] = provider;
                        AccountStore.Create(Forms.Context).Save(e.Account, App.AppName);
                    }
                }
                else if (provider == "Twitter")
                {
                    e.Account.Username = e.Account.Properties["user_id"];
                    e.Account.Properties["provider"] = provider;
                    App.User = new User();
                    App.User.Id = e.Account.Username;
                    App.User.Email = "foobar@test.com";
                    AccountStore.Create(Forms.Context).Save(e.Account, App.AppName);
                }
            }
			// If the user is logged in navigate to the TodoList page.
			// Otherwise allow another login attempt.
			App.SuccessfulLoginAction.Invoke ();
		}
	}
}
