using Android.App;
using Android.Widget;
using Android.OS;
using Xamarin.Auth;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Android.Content;
//cvnewggbsc_1487629189@tfbnw.net
//mi-390
namespace mRides_app
{
    [Activity(Label = "mRides_app", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        string userName;

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
                var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me"), null, e.Account);
                var response = await request.GetResponseAsync();
                if (response != null)
                {
                    var obj = JObject.Parse(response.GetResponseText());
                    userName = "Name: " + obj["name"].ToString();
                    var mapActivity = new Intent(this, typeof(MapActivity));
                    mapActivity.PutExtra("Profile Info", userName);
                    StartActivity(mapActivity);
                }
            }
        }

       // private static readonly TaskScheduler UIScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            var facebook = FindViewById<Button>(Resource.Id.button1);
            facebook.Click += delegate {
                LoginToFacebook(true);
            };


            //var facebookNoCancel = FindViewById<Button>(Resource.Id.FacebookButtonNoCancel);
            // facebookNoCancel.Click += delegate { LoginToFacebook(false); };
        }
    }
}
