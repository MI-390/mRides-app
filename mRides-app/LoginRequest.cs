using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Auth;
using mRides_app.Models;
using mRides_app.Mappers;
using Newtonsoft.Json.Linq;
using Firebase.Iid;

namespace mRides_app
{
    /// <summary>
    /// Class for the login request for the Facebook Login functionality.
    /// </summary>
    class LoginRequest
    {
        /// <summary>
        /// This method handles the login request by passing an existing account and the activity context
        /// </summary>
        /// <param name="account"> The account to authenticate.</param>
        /// <param name="context"> The context of the activity calling this method</param>
        public static async void handleLoginRequest(Account account, Context context)
        {
            var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me?fields=email,first_name,last_name,gender,picture"), null, account);
            var response = await request.GetResponseAsync();
            if (response != null)
            {
                // Obtain the content from the response
                var obj = JObject.Parse(response.GetResponseText());
                long facebookId = Convert.ToInt64(obj["id"].ToString());
                string facebookFirstName = obj["first_name"].ToString();
                string facebookLastName = obj["last_name"].ToString();
                string facebookGender = obj["gender"].ToString();

                // Try to obtain the user
                UserMapper userMapper = UserMapper.getInstance();
                User user = userMapper.GetUserByFacebookId(facebookId);

                // If the user already exists, set the current user to it
                // and go to main menu activity
                if (user != null)
                {
                    User.currentUser = user;
                    string token = FirebaseInstanceId.Instance.Token;
                    UserMapper.getInstance().updateFcmToken(token);
                    var mainMenuActivity = new Intent(context, typeof(MainMenuActivity));
                    mainMenuActivity.PutExtra("id", User.currentUser.id.ToString());
                    context.StartActivity(mainMenuActivity);
                }
                // Otherwise, go to the preference activity
                else
                {
                    var preferencesActivity = new Intent(context, typeof(PreferencesActivity));
                    preferencesActivity.PutExtra(Constants.IntentExtraNames.UserFacebookId, obj["id"].ToString());
                    preferencesActivity.PutExtra(Constants.IntentExtraNames.UserFacebookFirstName, facebookFirstName);
                    preferencesActivity.PutExtra(Constants.IntentExtraNames.UserFacebookLastName, facebookLastName);
                    preferencesActivity.PutExtra(Constants.IntentExtraNames.UserFacebookGender, facebookGender);
                    preferencesActivity.PutExtra(Constants.IntentExtraNames.PreviousActivity, Constants.ActivityNames.MainActivity);
                    context.StartActivity(preferencesActivity);
                }
            }
        }


        /// <summary>
        /// This method handles the logout request and deletes the account from the AccountStore of Xamarin.Auth
        /// </summary>
        /// <param name="context"> The context of the activity calling this method</param>
        public static void handleLogoutRequest(Context context)
        {
            var account = AccountStore.Create(context).FindAccountsForService("Facebook").FirstOrDefault();
            AccountStore.Create(context).Delete(account, "Facebook");
            Intent i = new Intent(context, typeof(MainActivity));
            context.StartActivity(i);
        }
    }
}