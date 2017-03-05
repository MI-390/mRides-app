using Android.App;
using Android.Widget;
using Android.OS;
using Xamarin.Auth;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Android.Content;
using mRides_app.Mappers;
using mRides_app.Models;
using System.Collections.Generic;
//cvnewggbsc_1487629189@tfbnw.net
//mi-390
namespace mRides_app
{
    //ggrrg
    [Activity(Label = "mRides_app", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        string userName;
        // For UI testing
        [Java.Interop.Export("StartActivityOne")]
        public void StartActivityOne()
        {
            Intent i = new Intent(this, typeof(PreferencesActivity));
            StartActivity(i);
        }
        [Java.Interop.Export("StartActivityTwo")]
        public void StartActivityTwo()
        {
            Intent i = new Intent(this, typeof(MapActivity));
            StartActivity(i);
        }

        [Java.Interop.Export("StartActivityThree")]
        public void StartActivityThree()
        {
            Intent i = new Intent(this, typeof(TestFragments));
            StartActivity(i);
        }

        [Java.Interop.Export("StartActivityFour")]
        public void StartActivityFour()
        {
            Intent i = new Intent(this, typeof(Feedback.FeedbackTest));
            StartActivity(i);
        }

        [Java.Interop.Export("StartActivityFive")]
        public void StartActivityFive()
        {
            Intent i = new Intent(this, typeof(UserProfileActivity));
            StartActivity(i);
        }

        void LoginToFacebook(bool allowCancel)
        {
            var auth = new OAuth2Authenticator(
                clientId: "211249475945892",
                scope: "",
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html"));

            auth.Completed += OnAuthenticationCompleted;

            auth.AllowCancel = allowCancel;

            var intent = auth.GetUI(this);
            StartActivity(intent);
        }

        async void OnAuthenticationCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (e.IsAuthenticated)
            {
                var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me?fields=email,first_name,last_name,gender,picture"), null, e.Account);
                var response = await request.GetResponseAsync();
                if (response != null)
                {
                    // Obtain the content from the response]
                    var obj = JObject.Parse(response.GetResponseText());
                    long facebookId = Convert.ToInt64(obj["id"]);
                    string facebookFirstName = obj["first_name"].ToString();
                    string facebookLastName = obj["last_name"].ToString();
                    // string facebookPicture = obj["picture"].ToString();

                    // Try to obtain the user
                    UserMapper userMapper = UserMapper.getInstance();
                    User user = userMapper.GetUserByFacebookId(facebookId);                    
                    
                    // If the user already exists, set the current user to it
                    // and go to map activity
                   // if (user != null)
                   // {
                    //    User.currentUser = user;
                     //   var mapActivity = new Intent(this, typeof(MapActivity));
                     //   StartActivity(mapActivity);
                   // }
                    // Otherwise, go to the preference activity
                   // else
                   // {
                        var preferencesActivity = new Intent(this, typeof(PreferencesActivity));
                        preferencesActivity.PutExtra(Constants.IntentExtraNames.UserFacebookId, obj["id"].ToString());
                        preferencesActivity.PutExtra(Constants.IntentExtraNames.UserFacebookFirstName, facebookFirstName);
                        preferencesActivity.PutExtra(Constants.IntentExtraNames.UserFacebookLastName, facebookLastName);
                        preferencesActivity.PutExtra(Constants.IntentExtraNames.PreviousActivity, Constants.ActivityNames.MainActivity);
                        StartActivity(preferencesActivity);
                  //  }


                    /** UNCOMMENT THE FOLLOWING TO VIEW USER PROFILE ACTIVITY UPON LOGIN **/
                    //userName = "" + obj["name"].ToString();
                    //var userProfileActivity = new Intent(this, typeof(UserProfileActivity));
                    //userProfileActivity.PutExtra("Profile Info", userName);
                    //StartActivity(userProfileActivity);
                    
                }
            }
        }

        // private static readonly TaskScheduler UIScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            var facebook = FindViewById<Button>(Resource.Id.loginButton);
            facebook.Click += delegate {
                LoginToFacebook(true);
            };


            //var facebookNoCancel = FindViewById<Button>(Resource.Id.FacebookButtonNoCancel);
            // facebookNoCancel.Click += delegate { LoginToFacebook(false); };
        }
    }
}
