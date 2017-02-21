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
    [Activity(Label = "UserProfile")]
    public class UserProfile : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            String username = Intent.GetStringExtra("Profile Info") ?? "Data not available";

            SetContentView(Resource.Layout.UserProfile);

            TextView text = (TextView)FindViewById(Resource.Id.userName);

            text.Text = username;



            // Create your application here
        }
    }
}