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
    [Activity(Label = "DriverAcceptDeclineMatchActivity")]
    public class DriverAcceptDeclineMatchActivity : Activity
    {
   
        //declaring variable for the layout elements and time fields.
        private TextView time_display;
        private int hour;
        private int minute;

        const int TIME_DIALOG_ID = 0;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "Main" layout resource
            SetContentView(Resource.Layout.Main);

            // Capture our View elements
            time_display = FindViewById<TextView>(Resource.Id.timeDisplay);
            

            // Add a click listener to the button
           

            // Get the current time
            hour = DateTime.Now.Hour;
            minute = DateTime.Now.Minute;

            // Display the current date
           
        }

    

    }
}