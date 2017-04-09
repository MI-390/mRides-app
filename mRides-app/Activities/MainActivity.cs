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
    [Activity(Label = "MainActivity")]
    public class MainActivity : Activity
    {
        string userName;

        // For UI testing

        // To test fragments, an activity was created and solely used for the purpose of opening
        // the fragments
        [Java.Interop.Export("StartTestFragmentsActivity")]
        public void StartActivityThree()
        {
            UserMapper userMapper = UserMapper.getInstance();
            User user = userMapper.GetUserByFacebookId(113083069215300);
            User.currentUser = user;
            Intent i = new Intent(this, typeof(TestFragments));
            StartActivity(i);
        }

        [Java.Interop.Export("StartEnterDrivingMode")]
        public void StartActivitySix()
        {
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://maps.google.com/maps?" + "saddr=" + "45.222,-72.70&daddr=45.4581,-73.6403"));
            intent.SetClassName("com.google.android.apps.maps", "com.google.android.maps.MapsActivity");
            StartActivity(intent);
        }

        [Java.Interop.Export("StartMainMenuActivity")]
        public void StartMainMenuActivity()
        {
            UserMapper userMapper = UserMapper.getInstance();
            User user = userMapper.GetUserByFacebookId(113083069215300);
            User.currentUser = user;
            string token = FirebaseInstanceId.Instance.Token;
            UserMapper.getInstance().updateFcmToken(token);
            var mainMenuActivity = new Intent(this, typeof(MainMenuActivity));
            mainMenuActivity.PutExtra("id", User.currentUser.id.ToString());
            this.StartActivity(mainMenuActivity);
            Intent intent = new Intent(this, typeof(MainMenuActivity));
            StartActivity(intent);
        }

        [Java.Interop.Export("StartMatchActivity")]
        public void StartMatchActivity()
        {
            UserMapper userMapper = UserMapper.getInstance();
            User user = userMapper.GetUserByFacebookId(113083069215300);
            User.currentUser = user;
            Intent matchActivity = new Intent(this, typeof(MatchActivity));
            matchActivity.PutExtra(Constants.IntentExtraNames.RequestType, User.currentUser.currentType);
            StartActivity(matchActivity);
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
