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

using mRides_app.Models;
using mRides_app.Constants;
using mRides_app.Mappers;
using Android.Graphics;
using Newtonsoft.Json;

namespace mRides_app
{
    [Activity(Label ="DriverAcceptDeclineMatchActivity")]
    public class DriverAcceptDeclineMatchActivity : Activity
    {

        private TextView show_time;
        private int hour;
        private int minute;
        private ImageView riderPicture;
        private Button acceptButton;
        private Button declineButton;
        private RiderRequest riderRequest;
        private Request driverRequest;

        private UserMapper userMapper;
        private ConsoleMapper consoleMapper; 

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.DriverAcceptDeclineMatch);

            // Obtain the mapper instances
            this.userMapper = UserMapper.getInstance();
            this.consoleMapper = ConsoleMapper.getInstance();

            // Deserialize the rider request and driver request from the json sent in the intent
            this.riderRequest = JsonConvert.DeserializeObject<RiderRequest>(Intent.GetStringExtra(IntentExtraNames.RiderRequestJson));
            this.driverRequest = JsonConvert.DeserializeObject<Request>(Intent.GetStringExtra(IntentExtraNames.DriverRequestJson));

            // Capture View elements
            this.show_time = FindViewById<TextView>(Resource.Id.displayTime);
            this.riderPicture = FindViewById<ImageView>(Resource.Id.driverAcceptDeclineMatchRiderPicture);

            // Capture the button accept button
            this.acceptButton = FindViewById<Button>(Resource.Id.driverMatchButtonAccept);
            acceptButton.Click += delegate { this.Accept(); };
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
            this.show_time.Text = time;

            // Update the profile picture of the rider
            Bitmap userPicture = userMapper.GetUserFacebookProfilePicture(this.riderRequest.rider.facebookID);
            if(userPicture != null)
            {
                this.riderPicture.SetImageBitmap(userPicture);
            }
        }

        /// <summary>
        /// Method invoked when the driver accepts the match with the suggested rider.
        /// </summary>
        private void Accept()
        {
            // Return to the caller activity once the driver accepted
            if(consoleMapper.AcceptConfirmation(this.riderRequest.id, this.driverRequest.ID))
            {
                this.Finish();
            }
        }
    }
}