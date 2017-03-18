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
    /// <summary>
    /// This activity takes in the intent a list of strings representing mulitple coordinates
    /// which makes up a route.
    /// </summary>
    [Activity(Label ="DriverAcceptDeclineMatchActivity")]
    public class DriverAcceptDeclineMatchActivity : Activity
    {

        private TextView show_time;
        private int hour;
        private int minute;
        private ImageView riderPicture;
        private Button acceptButton;
        private Button declineButton;
        private List<Request> matchedRequests;
        private Request driverRequest;

        // Keeps track of the index of the rider request currently displayed
        private int currentRiderRequestIndex;

        private UserMapper userMapper;
        private ConsoleMapper consoleMapper; 

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Obtain the mapper instances
            this.userMapper = UserMapper.getInstance();
            this.consoleMapper = ConsoleMapper.getInstance();

            // Deserialize the list of coordinates
            string json = Intent.GetStringExtra(IntentExtraNames.RouteCoordinatesJson);
            List <string> coordinates = JsonConvert.DeserializeObject<List<string>>(Intent.GetStringExtra(IntentExtraNames.RouteCoordinatesJson));

            // Send the request to the server
            Request newDriverRequest = new Request
            {
                destinationCoordinates = coordinates,
                destination = coordinates[coordinates.Count - 1],
                location = coordinates[0],
                type = Request.TYPE_DRIVER
            };
            this.matchedRequests = consoleMapper.FindRiders(newDriverRequest);

            // Start giving the driver choices of riders
            this.currentRiderRequestIndex = 0;
            this.UpdateDisplay();

            // Update the display
            if(this.matchedRequests.Count > 0)
            {
                UpdateDisplay();
            }
            else
            {
                // TODO: Display some message to the user
                Toast.MakeText(ApplicationContext, "No rider requests found", ToastLength.Long).Show();
            }
        }

        // Updates the time we display in the TextView
        private void UpdateDisplay()
        {
            // Get the rider to display
            Request request = this.matchedRequests[this.currentRiderRequestIndex];
            RiderRequest riderRequest = request.riderRequests.First();

            // Display
            SetContentView(Resource.Layout.DriverAcceptDeclineMatch);

            // Capture various elements from the view
            this.show_time = FindViewById<TextView>(Resource.Id.displayTime);
            this.riderPicture = FindViewById<ImageView>(Resource.Id.driverAcceptDeclineMatchRiderPicture);
            this.acceptButton = FindViewById<Button>(Resource.Id.driverMatchButtonAccept);

            // Capture the button accept button
            acceptButton.Click += delegate { this.Accept(); };

            // Set the current time
            hour = request.dateTime.Hour;
            minute = request.dateTime.Minute;
            string time = string.Format("{0}:{1}", hour, minute.ToString().PadLeft(2, '0'));
            this.show_time.Text = time;

            // Update the profile picture of the rider
            Bitmap userPicture = userMapper.GetUserFacebookProfilePicture(riderRequest.rider.facebookID);
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
            // Go to next rider
            if(++this.currentRiderRequestIndex < this.matchedRequests.Count)
            {
                // TODO: Display confirmation or error
                if(consoleMapper.AcceptConfirmation(this.matchedRequests[currentRiderRequestIndex].riderRequests.First().id, this.driverRequest.ID))
                {
                    Toast.MakeText(ApplicationContext, "A request has been sent to this rider", ToastLength.Long).Show();
                    this.UpdateDisplay();
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "An error has occurred while sending the request", ToastLength.Long).Show();
                }
            }
            else
            {
                this.Finish();
            }
        }
    }
}