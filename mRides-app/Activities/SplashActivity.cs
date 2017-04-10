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
    /// <summary>
    /// Activity that corresponds to the splash screen functionality of the application.
    /// </summary>
    public class SplashActivity : AppCompatActivity
    {
        /// <summary>
        /// Method that is invoked upon the creation of this activity.
        /// </summary>
        /// <param name="bundle">Variable used for passing data between activities.</param>
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            Log.Debug(typeof(SplashActivity).Name, "SplashActivity.OnCreate");
        }

        /// <summary>
        /// Method that is invoked when user returns to this activity.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            Task startUp = new Task(() => { Startup(); });
            startUp.Start();
        }

        /// <summary>
        /// Method that is invoked upon the start of this activity.
        /// </summary>
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