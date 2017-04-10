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
using Firebase.Iid;
using System.Linq;

namespace mRides_app
{
    [Activity(Label = "MainActivity")]
    /// <summary>
    /// Class that corresponds to the main activity of the application.
    /// </summary>
    public class MainActivity : Activity
    {
        string userName;
        // For UI testing

        [Java.Interop.Export("StartActivityOne")]
        /// <summary>
        /// This method is used for UI testing.
        /// It is used to test the PreferencesActivity.
        /// </summary>
        public void StartActivityOne()
        {
            Intent i = new Intent(this, typeof(PreferencesActivity));
            StartActivity(i);
        }

        [Java.Interop.Export("StartActivityTwo")]
        /// <summary>
        /// This method is used for UI testing.
        /// It is used to test the MapActivity.
        /// </summary>
        public void StartActivityTwo()
        {
            Intent i = new Intent(this, typeof(MapActivity));
            StartActivity(i);
        }

        [Java.Interop.Export("StartActivityThree")]
        /// <summary>
        /// This method is used for UI testing.
        /// It is used to test the fragments used.
        /// </summary>
        public void StartActivityThree()
        {
            Intent i = new Intent(this, typeof(TestFragments));
            StartActivity(i);
        }


        [Java.Interop.Export("StartActivityFour")]
        /// <summary>
        /// This method is used for UI testing.
        /// It is used to test the UserProfileActivity.
        /// </summary>
        public void StartActivityFour()
        {
            Intent i = new Intent(this, typeof(UserProfileActivity));
            i.PutExtra("id", "8");
            StartActivity(i);
        }

        [Java.Interop.Export("StartActivityFive")]
        /// <summary>
        /// This method is used for UI testing.
        /// It is used to test the ChatActivity.
        /// </summary>
        public void StartActivityFive()
        {
            Intent i = new Intent(this, typeof(ChatActivity));
            i.PutExtra("ChatName", "UITEST");
            i.PutExtra("id", "172");
            StartActivity(i);
        }

        [Java.Interop.Export("StartActivitySix")]
        /// <summary>
        /// This method is used for UI testing.
        /// It is used to test the Google Maps API.
        /// </summary>
        public void StartActivitySix()
        {
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://maps.google.com/maps?" + "saddr=" + "45.222,-72.70&daddr=45.4581,-73.6403"));
            intent.SetClassName("com.google.android.apps.maps", "com.google.android.maps.MapsActivity");
            StartActivity(intent);
        }

        /// <summary>
        /// This method is used to begin the login request.
        /// </summary>
        /// <param name="allowCancel"> The boolean that determines whether user can cancel the operation.</param>
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

        /// <summary>
        /// This method handles the completion of the login request.
        /// </summary>
        /// <param name="sender">The initiator of this intent request.</param>
        /// <param name="e">The authentication event object.</param>
        void OnAuthenticationCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (e.IsAuthenticated)
            {
                AccountStore.Create(this).Save(e.Account, "Facebook");
                LoginRequest.handleLoginRequest(e.Account, this);              
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            var facebook = FindViewById<Button>(Resource.Id.loginButton);
            facebook.Click += delegate
            {
              LoginToFacebook(true);
            };
        }
    }
}
