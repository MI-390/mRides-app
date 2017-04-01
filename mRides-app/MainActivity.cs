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

//cvnewggbsc_1487629189@tfbnw.net
//mi-390
namespace mRides_app
{
    //ggrrg
    [Activity(Label = "mRides_app", Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        string userName;
        // For UI testing
        [Java.Interop.Export("StartActivityOne")]
        // Test PreferencesActivity
        public void StartActivityOne()
        {
            Intent i = new Intent(this, typeof(PreferencesActivity));
            StartActivity(i);
        }
        //Test MapActivity
        [Java.Interop.Export("StartActivityTwo")]
        public void StartActivityTwo()
        {
            Intent i = new Intent(this, typeof(MapActivity));
            StartActivity(i);
        }
        //For testing fragments, an activity is used that contains buttons to open those fragments
        [Java.Interop.Export("StartActivityThree")]
        public void StartActivityThree()
        {
            Intent i = new Intent(this, typeof(TestFragments));
            StartActivity(i);
        }

        [Java.Interop.Export("StartActivityFour")]
        public void StartActivityFour()
        {
            Intent i = new Intent(this, typeof(UserProfileActivity));
            i.PutExtra("id", "8");
            StartActivity(i);
        }

        [Java.Interop.Export("StartActivityFive")]
        public void StartActivityFive()
        {
            Intent i = new Intent(this, typeof(ChatActivity));
            i.PutExtra("ChatName", "UITEST");
            i.PutExtra("id", "172");
            StartActivity(i);
        }

        [Java.Interop.Export("StartActivitySix")]
        public void StartActivitySix()
        {
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://maps.google.com/maps?" + "saddr=" + "45.222,-72.70&daddr=45.4581,-73.6403"));
            intent.SetClassName("com.google.android.apps.maps", "com.google.android.maps.MapsActivity");
            StartActivity(intent);
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

        void OnAuthenticationCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (e.IsAuthenticated)
            {
                AccountStore.Create(this).Save(e.Account, "Facebook");
                LoginRequest.handleRequest(e.Account, this);              
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
