using Android.App;
using Android.Widget;
using Android.OS;
using Xamarin.Auth;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Android.Content;

namespace mRides_app
{
    //ggrrg
    [Activity(Label = "mRides_app", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        void LoginToFacebook(bool allowCancel)
        {
            var auth = new OAuth2Authenticator(
                clientId: "211249475945892",
                scope: "",
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html"));

            auth.AllowCancel = allowCancel;

            // If authorization succeeds or is canceled, .Completed will be fired.
            auth.Completed += (s, ee) =>
            {
                if (!ee.IsAuthenticated)
                {
                    var builder = new AlertDialog.Builder(this);
                    builder.SetMessage("Not Authenticated");
                    builder.SetPositiveButton("Ok", (o, e) => { });
                    builder.Create().Show();
                    return;
                }

                // Now that we're logged in, make a OAuth2 request to get the user's info.
                var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me"), null, ee.Account);
                request.GetResponseAsync().ContinueWith(t =>
                {
                    var builder = new AlertDialog.Builder(this);
                    if (t.IsFaulted)
                    {
                        builder.SetTitle("Error");
                        builder.SetMessage(t.Exception.Flatten().InnerException.ToString());
                    }
                    else if (t.IsCanceled)
                        builder.SetTitle("Task Canceled");
                    else
                    {
                        var obj = JObject.Parse(t.Result.GetResponseText());

                        builder.SetTitle("Logged in");
                        builder.SetMessage("Name: " + obj["name"]);
                        String username = ("" + obj["name"]);
                        
                        if (username != null && ee.IsAuthenticated)
                        {
                            var userProfile = new Intent(this, typeof(UserProfile));
                            userProfile.PutExtra("Profile Info", username);
                            StartActivity(userProfile);

                        }
                    }

                    builder.SetPositiveButton("Ok", (o, e) => { });
                    builder.Create().Show();
                }, UIScheduler);
            };

            var intent = auth.GetUI(this);
            StartActivity(intent);
        }

        private static readonly TaskScheduler UIScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            var facebook = FindViewById<Button>(Resource.Id.button1);
            facebook.Click += delegate { LoginToFacebook(true); };


            //var facebookNoCancel = FindViewById<Button>(Resource.Id.FacebookButtonNoCancel);
            // facebookNoCancel.Click += delegate { LoginToFacebook(false); };
        }
    }
}
