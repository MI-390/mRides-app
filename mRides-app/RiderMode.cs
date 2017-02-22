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
    [Activity(Label = "RiderMode")]
    public class RiderMode : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.RiderMode);
            String num = Intent.GetStringExtra("numOfPeople");
            int test = Int32.Parse(num); // parse the string to an int
            TextView tv = FindViewById<TextView>(Resource.Id.riderModeText);
            tv.SetText("Number of people in your group: " + test, TextView.BufferType.Normal);
        }
    }
}