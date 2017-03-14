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
using Android.Gms.Common; // For FCM
using Firebase.Messaging;
using Firebase.Iid;
using Android.Util;

namespace mRides_app
{
    [Activity(Label = "MessagingActivity")]
    public class MessagingActivity : Activity
    {
        TextView msgText;
        const string TAG = "MessagingActivity";
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Messaging);
            msgText = FindViewById<TextView>(Resource.Id.msgText1);
            // When the user taps a notification issued from FCMClient, any data accompanying that notification message is made available in Intent extras.
            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    var value = Intent.Extras.GetString(key);
                    Log.Debug(TAG, "Key: {0} Value: {1}", key, value);
                }
            }

            IsPlayServicesAvailable();

            // Logs the current token to the output window when the Log Token button is clicked
            var logTokenButton = FindViewById<Button>(Resource.Id.logTokenButton);
            logTokenButton.Click += delegate {
                Log.Debug(TAG, "InstanceID token: " + FirebaseInstanceId.Instance.Token);
            };
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    msgText.Text = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                else
                {
                    msgText.Text = "This device is not supported";
                    Finish();
                }
                return false;
            }
            else
            {
                msgText.Text = "Google Play Services is available.";
                return true;
            }
        }
    }
}