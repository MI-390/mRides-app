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
using Android.Support.V7.App;
using Android.Util;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace mRides_app
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true, Icon = "@drawable/red_logo", Label = "mRides")]
    public class SplashActivity : AppCompatActivity
    {

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            Log.Debug(typeof(SplashActivity).Name, "SplashActivity.OnCreate");
        }

        protected override void OnResume()
        {
            base.OnResume();
            Task startUp = new Task(() => { Startup(); });
            startUp.Start();
        }

        async void Startup()
        {
            Log.Debug(typeof(SplashActivity).Name, "Performing startup work that takes time.");
            var account = AccountStore.Create(this).FindAccountsForService("Facebook").FirstOrDefault();
            if (account != null)
                LoginRequest.handleLoginRequest(account, this);
            else
                StartActivity(new Intent(this, typeof(MainActivity)));
        }
    }
}