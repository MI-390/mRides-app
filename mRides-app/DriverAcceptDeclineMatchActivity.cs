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
    [Activity(Label ="DriverAcceptDeclineMatchActivity")]
    public class DriverAcceptDeclineMatchActivity : Activity
    {

        private TextView show_time;
        private int hour;
        private int minute;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.DriverAcceptDeclineMatch);

            // Capture View elements
            TextView show_time = FindViewById<TextView>(Resource.Id.displayTime);
                      

            // Get the current time
            hour = DateTime.Now.Hour;
            minute = DateTime.Now.Minute;

            // Display the current date
            UpdateDisplay();
        }

        // Updates the time we display in the TextView
        private void UpdateDisplay()
        {
            string time = string.Format("{0}:{1}", hour, minute.ToString().PadLeft(2, '0'));
            show_time.Text = time;
        }




    }
}