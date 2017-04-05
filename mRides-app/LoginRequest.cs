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
    class LoginRequest
    {

        public static async void handleRequest(Account account, Context context)
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
                // and go to map activity
                if (user != null)
                {
                    User.currentUser = user;
                    string token = FirebaseInstanceId.Instance.Token;
                    UserMapper.getInstance().updateFcmToken(token);
                    var mapActivity = new Intent(context, typeof(MapActivity));
                    context.StartActivity(mapActivity);
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
    }
}