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
using mRides_app.Tasks;
using Android.Graphics;
using Newtonsoft.Json;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System.Net;
using RestSharp;
using Newtonsoft.Json.Linq;
using static mRides_app.Models.Request;

namespace mRides_app
{
    /// <summary>
    /// This activity takes in the intent a list of strings representing mulitple coordinates
    /// which makes up a route.
    /// </summary>
    [Activity(Label ="MatchActivity")]
    public class MatchActivity : Activity, IOnMapReadyCallback, IOnFindMatchCompleteCallback
    {
        // Page elements
        private TextView show_time;
        private TextView matchedUserName;
        private TextView matchedUserFrom;
        private TextView matchedUserGoingTo;
        private TextView matchedUserRole;
        private ImageView matchedUserPicture;
        private Button acceptButton;
        private Button declineButton;
        private Button doneButton;
        private Button chatButton;
        private RatingBar matchedUserRatingBar;
        private GoogleMap matchedUserLocationMap;
        private MapFragment mapFragment;

        // Request data
        private string userType;
        private int hour;
        private int minute;
        private List<Request> matchedRequests;
        private Request userRequest;

        // Keeps track of the index of the rider request currently displayed
        private int currentMatchedUserIndex;

        // Mappers
        private UserMapper userMapper;
        private ConsoleMapper consoleMapper;

        protected override void OnCreate(Bundle bundle)
        {
            UserMapper.getInstance().setTheme(this);
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Match);

            // Capture the accept button
            this.acceptButton = FindViewById<Button>(Resource.Id.userMatchButtonAccept);
            this.acceptButton.Click += delegate { this.Proceed(true); };

            // Capture the decline button
            this.declineButton = FindViewById<Button>(Resource.Id.userMatchButtonDecline);
            this.declineButton.Click += delegate { this.Proceed(false); };

            // Capture the done button
            this.doneButton = FindViewById<Button>(Resource.Id.userMatchButtonDone);
            this.doneButton.Click += delegate { this.Finish(); };

            // Capture the chat button
            this.chatButton = FindViewById<Button>(Resource.Id.userMatchingChatButton);
            this.chatButton.Click += delegate { this.Chat(); };
            LinearLayout layout = FindViewById<LinearLayout>(Resource.Id.matchingLinearLayout3);
            // Set button colors sto the right color
            if (User.currentUser != null)
            {
                if (User.currentUser.currentType == "rider")
                {
                    this.acceptButton.SetBackgroundResource(Resource.Drawable.green_button);
                    this.declineButton.SetBackgroundResource(Resource.Drawable.green_button);
                    this.doneButton.SetBackgroundResource(Resource.Drawable.green_button);
                    this.chatButton.SetBackgroundResource(Resource.Drawable.smsicongreen);
                    layout.SetBackgroundResource(Resource.Drawable.greenRoundedBg);
                }
                else
                {
                    this.acceptButton.SetBackgroundResource(Resource.Drawable.red_button);
                    this.declineButton.SetBackgroundResource(Resource.Drawable.red_button);
                    this.doneButton.SetBackgroundResource(Resource.Drawable.red_button);
                    this.chatButton.SetBackgroundResource(Resource.Drawable.smsiconred);
                    layout.SetBackgroundResource(Resource.Drawable.redRoundedBg);
                }
            }

            // Put the map fragment programatically
            this.mapFragment = MapFragment.NewInstance();
            var ft = FragmentManager.BeginTransaction();
            ft.Add(Resource.Id.userMatchingMapPlaceHolder, mapFragment).Commit();


            // Obtain the mapper instances
            this.userMapper = UserMapper.getInstance();
            this.consoleMapper = ConsoleMapper.getInstance();

            // Obtain the type of the user for this request and deserialize the list of coordinates
            this.userType = Intent.GetStringExtra(Constants.IntentExtraNames.RequestType);
            string json = Intent.GetStringExtra(IntentExtraNames.RouteCoordinatesJson);
            List <DestinationCoordinate> coordinates = JsonConvert.DeserializeObject<List<DestinationCoordinate>>(Intent.GetStringExtra(IntentExtraNames.RouteCoordinatesJson));

            // Send an async request to find matches
            this.userRequest = new Request
            {
                destinationCoordinates = coordinates,
                destination = coordinates.Last().coordinate,
                location = coordinates.First().coordinate,
                type = this.userType
            };
            FindMatchAsyncTask findMatchTask = new FindMatchAsyncTask(this.userRequest, this);
            findMatchTask.Execute();
        }

        /// <summary>
        /// Method called when finding the match from the server is done
        /// </summary>
        /// <param name="requests">List of matched requests</param>
        public void OnFindMatchComplete(List<Request> requests)
        {
            // Start giving the user choices of matched users
            this.matchedRequests = requests;
            this.currentMatchedUserIndex = 0;
            if (this.matchedRequests.Count > 0)
            {
                RunOnUiThread(() => this.UpdateDisplay());
            }
            else
            {
                // Display some message to the user
                Toast.MakeText(ApplicationContext, Resources.GetString(Resource.String.matchNoMatchFound), ToastLength.Long).Show();
                Finish();
            }
        }

        /// <summary>
        /// Updates the view of this activity with the information related to the current
        /// request defined by the attributes list of requests and by the current index of
        /// that list. Provides a way to loop through all the list.
        /// </summary>
        private void UpdateDisplay()
        {
            // Get the matched user to display
            Request currentRequest = this.matchedRequests[this.currentMatchedUserIndex];
            RiderRequest matchedRiderRequest = null;
            User matchedUser;
            if (this.userType == Request.TYPE_DRIVER)
            {
                matchedRiderRequest = currentRequest.riderRequests.First();
                matchedUser = matchedRiderRequest.rider;
            }
            else
            {
                matchedUser = currentRequest.driver;
            }

            // Put the map fragment programatically
            this.mapFragment = MapFragment.NewInstance();
            var ft = FragmentManager.BeginTransaction();
            ft.Add(Resource.Id.userMatchingMapPlaceHolder, mapFragment).Commit();

            // Display the rider's location
            this.mapFragment.GetMapAsync(this);
            
            // Set the profile picture
            this.matchedUserPicture = FindViewById<ImageView>(Resource.Id.matchedUserPicture);
            this.matchedUserPicture.Click += delegate { this.OpenUserProfile(); };
            Bitmap userPicture = userMapper.GetUserFacebookProfilePicture(matchedUser.facebookID);
            if (userPicture != null)
            {
                this.matchedUserPicture.SetImageBitmap(userPicture);
            }

            // Display the matched user's name
            this.matchedUserName = FindViewById<TextView>(Resource.Id.matchedUserName);
            this.matchedUserName.Text = matchedUser.firstName + " " + matchedUser.lastName;

            // Display the role of the matched user (opposite of the current user type)
            this.matchedUserRole = FindViewById<TextView>(Resource.Id.matchedUserRole);
            this.matchedUserRole.Text = this.userType == Request.TYPE_DRIVER ? Resources.GetString(Resource.String.user_rider) : Resources.GetString(Resource.String.user_driver);

            // Obtain the matched user's origin and destination coordinates and set the addresses
            string[] matchedUserOriginCoordinates;
            string[] matchedUserDestinationCoordinates;
            if (this.userType == Request.TYPE_DRIVER)
            {
                matchedUserOriginCoordinates = matchedRiderRequest.location.Split(',');
                matchedUserDestinationCoordinates = matchedRiderRequest.destination.Split(',');
            }
            else
            {
                matchedUserOriginCoordinates = currentRequest.location.Split(',');
                matchedUserDestinationCoordinates = currentRequest.destination.Split(',');
            }
            if(matchedUserOriginCoordinates.Length > 1)
            {
                this.matchedUserFrom = FindViewById<TextView>(Resource.Id.userMatchedOrigin);
                this.matchedUserFrom.Text = ReverseGeoCode(matchedUserOriginCoordinates[0], matchedUserOriginCoordinates[1]);
            }
            if(matchedUserDestinationCoordinates.Length > 1)
            {
                this.matchedUserGoingTo = FindViewById<TextView>(Resource.Id.userMatchedDestination);
                this.matchedUserGoingTo.Text = ReverseGeoCode(matchedUserDestinationCoordinates[0], matchedUserDestinationCoordinates[1]);
            }
            
            // Update the rating bar to the average rating the rider received
            this.matchedUserRatingBar = FindViewById<RatingBar>(Resource.Id.ratingBarRiderDestinationMatch);
            List<Models.Feedback> riderFeedbacks = UserMapper.getInstance().GetReviews(matchedUser.id);

            if (riderFeedbacks.Count > 0)
            {
                int sumStars = 0;
                double averageStars = 0;
                foreach (Models.Feedback feedback in riderFeedbacks)
                {
                    sumStars = feedback.stars;
                }
                averageStars = (double)sumStars / riderFeedbacks.Count;
                this.matchedUserRatingBar.Rating = (int)Math.Round(averageStars);
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
                if(this.userType == Request.TYPE_DRIVER)
                {
                    consoleMapper.Confirm(this.matchedRequests[currentMatchedUserIndex].riderRequests.First().requestID, this.userRequest.ID);
                }
                else
                {
                    consoleMapper.Confirm(this.matchedRequests[currentMatchedUserIndex].ID, this.userRequest.ID);
                }
                
                Toast.MakeText(ApplicationContext, Resources.GetString(Resource.String.matchRequestSent), ToastLength.Long).Show();
            }

            // Update to the view if there is a next one, otherwise finish this activity
            if (++this.currentMatchedUserIndex < this.matchedRequests.Count)
            {
                RunOnUiThread(()=> this.UpdateDisplay());
            }
            else
            {
                Toast.MakeText(ApplicationContext, Resources.GetString(Resource.String.matchNoMoreMatch), ToastLength.Long).Show();
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
            this.matchedUserLocationMap = googleMap;

            // Obtain the current matched user's request being processed
            User currentMatchedUser = null;
            string location;
            if(this.userType == Request.TYPE_DRIVER)
            {
                RiderRequest currentRiderRequest = this.matchedRequests[this.currentMatchedUserIndex].riderRequests.First();
                location = currentRiderRequest.location;
                currentMatchedUser = currentRiderRequest.rider;
            }
            else
            {
                location = this.matchedRequests[this.currentMatchedUserIndex].location;
                currentMatchedUser = this.matchedRequests[this.currentMatchedUserIndex].driver;
            }
            

            // Create a custom marker for the rider's location
            MarkerOptions userMarker = new MarkerOptions();
            string[] splitCoordinates = location.Split(',');
            LatLng riderCoordinates = new LatLng(Double.Parse(splitCoordinates[0]), Double.Parse(splitCoordinates[1]));
            userMarker.SetPosition(riderCoordinates)
                                  .SetTitle(currentMatchedUser.firstName?.ToString() + " " + currentMatchedUser.lastName?.ToString())
                                  .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.userIcon2)).Anchor(0.5f, 0.5f);
            this.matchedUserLocationMap.AddMarker(userMarker);

            // Move camera to the marker
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(riderCoordinates);
            builder.Zoom(17);
            builder.Bearing(45);
            builder.Tilt(90);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            this.matchedUserLocationMap.AnimateCamera(cameraUpdate);
        }

        /// <summary>
        /// Opens the user profile of the matched user
        /// </summary>
        private void OpenUserProfile()
        {
            int matchedUserId = 0;

            if (this.userType == Request.TYPE_DRIVER)
            {
                matchedUserId = this.matchedRequests[currentMatchedUserIndex].riderRequests.First().riderID;
            }
            else
            {
                matchedUserId = this.matchedRequests[currentMatchedUserIndex].driver.id;
            }

            Intent profileIntent = new Intent(this, typeof(UserProfileActivity));
            profileIntent.PutExtra("id", matchedUserId.ToString());
            StartActivity(profileIntent);
        }

        /// <summary>
        /// Method invoked when the chat button is clicked. Will start the chat activity with the rider.
        /// </summary>
        private void Chat()
        {
            int matchedUserId = 0;

            if(this.userType == Request.TYPE_DRIVER)
            {
                matchedUserId = this.matchedRequests[currentMatchedUserIndex].riderRequests.First().riderID;
            }
            else
            {
                matchedUserId = this.matchedRequests[currentMatchedUserIndex].driver.id;
            }
            
            Intent chatIntent = new Intent(this, typeof(ChatActivity));
            chatIntent.PutExtra("ChatName", CreateChatName(matchedUserId));
            chatIntent.PutExtra("id", matchedUserId);
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