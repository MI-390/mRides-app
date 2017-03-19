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
using Android.Gms.Maps;
using Android.Gms.Maps.Model;

namespace mRides_app
{
    /// <summary>
    /// This activity takes in the intent a list of strings representing mulitple coordinates
    /// which makes up a route.
    /// </summary>
    [Activity(Label ="DriverAcceptDeclineMatchActivity")]
    public class DriverAcceptDeclineMatchActivity : Activity, IOnMapReadyCallback
    {

        private TextView show_time;
        private TextView riderName;
        private int hour;
        private int minute;
        private ImageView riderPicture;
        private Button acceptButton;
        private Button declineButton;
        private Button doneButton;
        private RatingBar riderRating;
        private GoogleMap riderLocationMap;
        private MapFragment mapFragment;

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
            this.driverRequest = new Request
            {
                destinationCoordinates = coordinates,
                destination = coordinates[coordinates.Count - 1],
                location = coordinates[0],
                type = Request.TYPE_DRIVER
            };
            this.matchedRequests = consoleMapper.FindRiders(this.driverRequest);

            // Start giving the driver choices of riders
            this.currentRiderRequestIndex = 0;
            this.UpdateDisplay();

            // Update the display
            if(this.matchedRequests.Count > 0)
            {
                

                this.UpdateDisplay();
            }
            else
            {
                // Display some message to the user
                Toast.MakeText(ApplicationContext, Resources.GetString(Resource.String.driverMatchNoMatchFound), ToastLength.Long).Show();
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

            // Capture the accept button
            this.acceptButton = FindViewById<Button>(Resource.Id.driverMatchButtonAccept);
            this.acceptButton.Click += delegate { this.Proceed(true); };

            // Capture the decline button
            this.declineButton = FindViewById<Button>(Resource.Id.driverMatchButtonDecline);
            this.declineButton.Click += delegate { this.Proceed(false); };

            // Capture the done button
            this.doneButton = FindViewById<Button>(Resource.Id.driverMatchButtonDone);
            this.doneButton.Click += delegate { this.Finish(); };

            // Put the map fragment programatically
            this.mapFragment = MapFragment.NewInstance();
            var ft = FragmentManager.BeginTransaction();
            ft.Add(Resource.Id.driverMatchingMapPlaceHolder, mapFragment).Commit();

            // Display the rider's location
            this.mapFragment.GetMapAsync(this);
            
            // Set the profile picture
            this.riderPicture = FindViewById<ImageView>(Resource.Id.driverAcceptDeclineMatchRiderPicture);
            Bitmap userPicture = userMapper.GetUserFacebookProfilePicture(riderRequest.rider.facebookID);
            if (userPicture != null)
            {
                this.riderPicture.SetImageBitmap(userPicture);
            }

            // Set the time of the rider's request
            this.show_time = FindViewById<TextView>(Resource.Id.displayTime);
            hour = request.dateTime.Hour;
            minute = request.dateTime.Minute;
            string time = string.Format("{0}:{1}", hour, minute.ToString().PadLeft(2, '0'));
            this.show_time.Text = time;

            // Display the rider's name
            this.riderName = FindViewById<TextView>(Resource.Id.driverMatchingRiderName);
            riderName.Text = riderRequest.rider.firstName + " " + riderRequest.rider.lastName;

            // Update the rating bar to the average rating the rider received
            this.riderRating = FindViewById<RatingBar>(Resource.Id.ratingBarRiderDestinationMatch);
            List<Models.Feedback> riderFeedbacks = UserMapper.getInstance().GetReviews(riderRequest.rider.id);

            if (riderFeedbacks.Count > 0)
            {
                int sumStars = 0;
                double averageStars = 0;
                foreach (Models.Feedback feedback in riderFeedbacks)
                {
                    sumStars = feedback.stars;
                }
                averageStars = (double)sumStars / riderFeedbacks.Count;
                this.riderRating.NumStars = (int)Math.Round(averageStars);
            }
        }

        /// <summary>
        /// Method invoked when the driver either accepts or declines a match
        /// </summary>
        private void Proceed(bool accept)
        {
            // Send confirmation message to the server if driver accepted
            if(accept)
            {
                consoleMapper.Confirm(this.matchedRequests[currentRiderRequestIndex].riderRequests.First().id, this.driverRequest.ID);
                Toast.MakeText(ApplicationContext, Resources.GetString(Resource.String.driverMatchRequestSent), ToastLength.Long).Show();
            }

            // Update to the view if there is a next one, otherwise finish this activity
            if (++this.currentRiderRequestIndex < this.matchedRequests.Count)
            {
                this.UpdateDisplay();
            }
            else
            {
                Toast.MakeText(ApplicationContext, Resources.GetString(Resource.String.driverMatchNoMoreMatch), ToastLength.Long).Show();
                this.Finish();
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            // Set the instance of google map
            this.riderLocationMap = googleMap;

            // Obtain the current rider request being processed
            RiderRequest currentRiderRequest = this.matchedRequests[this.currentRiderRequestIndex].riderRequests.First();

            // Create a custom marker for the rider's location
            MarkerOptions userMarker = new MarkerOptions();
            string[] splitCoordinates = currentRiderRequest.location.Split(',');
            LatLng riderCoordinates = new LatLng(Double.Parse(splitCoordinates[0]), Double.Parse(splitCoordinates[1]));
            userMarker.SetPosition(riderCoordinates)
                                  .SetTitle(currentRiderRequest.rider.firstName.ToString() + " " + currentRiderRequest.rider.lastName.ToString())
                                  .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.userIcon2)).Anchor(0.5f, 0.5f);
            this.riderLocationMap.AddMarker(userMarker);

            // Move camera to the marker
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(riderCoordinates);
            builder.Zoom(17);
            builder.Bearing(45);
            builder.Tilt(90);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            this.riderLocationMap.AnimateCamera(cameraUpdate);
        }
    }
}