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
using System.Net;
using RestSharp;
using Newtonsoft.Json.Linq;

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
        private TextView riderFrom;
        private TextView riderGoingTo;
        private int hour;
        private int minute;
        private ImageView riderPicture;
        private Button acceptButton;
        private Button declineButton;
        private Button doneButton;
        private Button chatButton;
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

        /// <summary>
        /// Updates the view of this activity with the information related to the current
        /// request defined by the attributes list of requests and by the current index of
        /// that list. Provides a way to loop through all the list.
        /// </summary>
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

            // Capture the chat button
            this.chatButton = FindViewById<Button>(Resource.Id.driverMatchingChatButton);
            this.chatButton.Click += delegate { this.Chat(); };

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

            // Obtain the rider's from/going
            string[] riderFromCoordinates = riderRequest.location.Split(',');
            if(riderFromCoordinates.Length > 1)
            {
                this.riderFrom = FindViewById<TextView>(Resource.Id.driverMatchingRiderFrom);
                this.riderFrom.Text = ReverseGeoCode(riderFromCoordinates[0], riderFromCoordinates[1]);
            }
            
            string[] riderGoingToCoordinates = riderRequest.destination.Split(',');
            if(riderGoingToCoordinates.Length > 1)
            {
                this.riderGoingTo = FindViewById<TextView>(Resource.Id.driverMatchingRiderGoingTo);
                this.riderGoingTo.Text = ReverseGeoCode(riderGoingToCoordinates[0], riderGoingToCoordinates[1]);
            }
            

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
                this.riderRating.Rating = (int)Math.Round(averageStars);
            }
        }

        /// <summary>
        /// Method invoked when the driver either accepts or declines a match. Upon acceptance,
        /// the method will send a confirmation to the server. Then, regardless of whether the
        /// match was accepted or not, this method will update the view to the next match,
        /// or finish the activity if no more matches are found.
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

        /// <summary>
        /// Method called by google map once the map is finished loading
        /// </summary>
        /// <param name="googleMap"></param>
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
                                  .SetTitle(currentRiderRequest.rider.firstName?.ToString() + " " + currentRiderRequest.rider.lastName?.ToString())
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

        /// <summary>
        /// Method invoked when the chat button is clicked. Will start the chat activity with the rider.
        /// </summary>
        private void Chat()
        {
            int riderId = this.matchedRequests[currentRiderRequestIndex].riderRequests.First().riderID;
            
            Intent chatIntent = new Intent(this, typeof(ChatActivity));
            chatIntent.PutExtra("ChatName", CreateChatName(riderId));
            chatIntent.PutExtra("id", riderId);
            StartActivity(chatIntent);
        }

        /// <summary>
        /// Creates the chat name based on the user id to which we want to communicate with
        /// </summary>
        /// <param name="riderId"></param>
        /// <returns>string chat name</returns>
        private string CreateChatName(int riderId)
        {
            int intUserId = Convert.ToInt32(riderId);
            int currentUser = User.currentUser.id;
            if (currentUser < intUserId)
            {
                return currentUser + " & " + riderId;
            }
            return riderId + " & " + currentUser;
        }

        /// <summary>
        /// Reverse geocode using Google API. Given the latitude and longitude,
        /// this method will return a string representing the approximate address
        /// of the location.
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns>string approximate address</returns>
        public string ReverseGeoCode(string latitude, string longitude)
        {
            string reverseGeoCodingBaseUrl = "https://maps.googleapis.com/maps/api/geocode/";
            string reverseGeoEndApi = "json?latlng=" + latitude + "," + longitude;
            
            var client = new RestClient(reverseGeoCodingBaseUrl);
            var request = new RestRequest(reverseGeoEndApi, Method.GET);
            
            var response = client.Execute(request);
            dynamic requestResults = JObject.Parse(response.Content);
            string formattedAddress = (string) requestResults["results"][0]["formatted_address"] ;
            return formattedAddress;
        }


    }
}