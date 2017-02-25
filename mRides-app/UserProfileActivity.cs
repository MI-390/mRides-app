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

namespace mRides_app
{
    [Activity(Label = "UserProfileActivity")]
    public class UserProfileActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            TextView usernameText;

            String username = Intent.GetStringExtra("Profile Info") ?? "Data not available";

            SetContentView(Resource.Layout.UserProfile);

            usernameText = FindViewById<TextView>(Resource.Id.userName);

            usernameText.Text = username;

        }
    }
}