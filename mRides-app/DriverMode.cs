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
    [Activity(Label = "DriverMode")]
    public class DriverMode : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.DriverMode);

            String num = Intent.GetStringExtra("numOfSeats");
            TextView tv = FindViewById<TextView>(Resource.Id.driverModeText);
            tv.SetText("Number of available seats: " + num, TextView.BufferType.Normal);
        }
    }
}