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
using System.Threading;
using System.Threading.Tasks;
using mRides_app.Tasks.Callbacks;
using Android.Locations;

namespace mRides_app
{
    /// <summary>
    /// This activity takes in the intent a list of strings representing mulitple coordinates
    /// which makes up a route.
    /// </summary>
    [Activity(Label ="MatchActivity")]
    public class MatchActivity : Activity, IOnMapReadyCallback, IOnFindMatchCompleteCallback, IOnGetReviewsAverageCompleteCallback
    {
        // Link to the google api to obtain geocoding
        private const string REVERSE_GEOCODING_BASE_URL = "https://maps.googleapis.com/maps/api/geocode/";

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

        /// <summary>
        /// Method called upon creation of the activity. The page will be loaded, and elements
        /// of importance contained in the page will be captured. A request to the server
        /// is then sent to obtain the list of matched users.
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            // Obtain the mapper instances
            this.userMapper = UserMapper.getInstance();
            this.consoleMapper = ConsoleMapper.getInstance();

            // Set the theme
            this.userMapper.setTheme(this);

            // Display view
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Match);

            // While loading, set the texts to loading
            this.matchedUserName = FindViewById<TextView>(Resource.Id.matchedUserName);
            this.matchedUserName.Text = Resources.GetString(Resource.String.matchLoading);

            this.matchedUserPicture = FindViewById<ImageView>(Resource.Id.matchedUserPicture);
            this.matchedUserPicture.Visibility = ViewStates.Invisible;

            this.matchedUserRatingBar = FindViewById<RatingBar>(Resource.Id.ratingBarRiderDestinationMatch);
            this.matchedUserRatingBar.Rating = 0;

            this.matchedUserGoingTo = FindViewById<TextView>(Resource.Id.userMatchedDestination);
            this.matchedUserGoingTo.Text = Resources.GetString(Resource.String.matchLoading);

            this.matchedUserFrom = FindViewById<TextView>(Resource.Id.userMatchedOrigin);
            this.matchedUserFrom.Text = Resources.GetString(Resource.String.matchLoading);

            // Capture the accept button
            this.acceptButton = FindViewById<Button>(Resource.Id.userMatchButtonAccept);
            this.acceptButton.Visibility = ViewStates.Invisible;
            this.acceptButton.Click += delegate { this.Proceed(true); };

            // Capture the decline button
            this.declineButton = FindViewById<Button>(Resource.Id.userMatchButtonDecline);
            this.declineButton.Visibility = ViewStates.Invisible;
            this.declineButton.Click += delegate { this.Proceed(false); };

            // Capture the done button
            this.doneButton = FindViewById<Button>(Resource.Id.userMatchButtonDone);
            this.doneButton.Click += delegate { this.Done(); };

            // Capture the chat button
            this.chatButton = FindViewById<Button>(Resource.Id.userMatchingChatButton);
            this.chatButton.Click += delegate { this.Chat(); };
            this.chatButton.Visibility = ViewStates.Invisible;
            LinearLayout layout = FindViewById<LinearLayout>(Resource.Id.matchingLinearLayout3);
            
            // Set button colors to the right color
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
            
            // Obtain the type of the user for this request and deserialize the list of coordinates
            this.userType = Intent.GetStringExtra(Constants.IntentExtraNames.RequestType);
            string json = Intent.GetStringExtra(IntentExtraNames.RouteCoordinatesJson);
            List <DestinationCoordinate> coordinates = JsonConvert.DeserializeObject<List<DestinationCoordinate>>(Intent.GetStringExtra(IntentExtraNames.RouteCoordinatesJson));

            // Display the role of the matched user (opposite of the current user type)
            this.matchedUserRole = FindViewById<TextView>(Resource.Id.matchedUserRole);
            this.matchedUserRole.Text = this.userType == Request.TYPE_DRIVER ? Resources.GetString(Resource.String.user_rider) : Resources.GetString(Resource.String.user_driver);
            
            // Send an async request to find matches
            this.userRequest = new Request
            {
                destinationCoordinates = coordinates,
                destination = coordinates.Last().coordinate,
                location = coordinates.First().coordinate,
                type = this.userType
            };
            FindMatchAsyncTask findMatchTask = new FindMatchAsyncTask(this.userRequest, this);
            findMatchTask.Execute(); // Upon completion, the OnFindMatchComplete method is invoked.
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
            
            // Display the rider's location
            this.mapFragment.GetMapAsync(this);
            
            // Set the profile picture
            this.matchedUserPicture.Click += delegate { this.OpenUserProfile(); };
            this.SetMatchedUserProfilePicture(matchedUser.facebookID);

            // Update the rating bar to the average rating the rider received
            GetReviewsAverageAsyncTask getReviewsAvgAsyncTask = new GetReviewsAverageAsyncTask(matchedUser.id, this);
            getReviewsAvgAsyncTask.Execute();

            // Display the matched user's name
            this.matchedUserName.Text = matchedUser.firstName + " " + matchedUser.lastName;

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
                this.SetMatchedUserLocation(Double.Parse(matchedUserOriginCoordinates[0]), Double.Parse(matchedUserOriginCoordinates[1]));
            }
            if(matchedUserDestinationCoordinates.Length > 1)
            {
                this.SetMatchedUserDestination(Double.Parse(matchedUserDestinationCoordinates[0]), Double.Parse(matchedUserDestinationCoordinates[1]));
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
                Toast.MakeText(ApplicationContext, Resources.GetString(Resource.String.matchRequestSent), ToastLength.Short).Show();
                if (this.userType == Request.TYPE_DRIVER)
                {
                    consoleMapper.Confirm(this.matchedRequests[currentMatchedUserIndex].riderRequests.First().requestID, this.userRequest.ID);
                    this.LoadNextMatchedRequest();
                }
                else
                {
                    consoleMapper.Confirm(this.userRequest.ID, this.matchedRequests[currentMatchedUserIndex].ID);
                    this.Done();
                }
            }
            else
            {
                Toast.MakeText(ApplicationContext, Resources.GetString(Resource.String.matchDeclined), ToastLength.Short).Show();
                this.LoadNextMatchedRequest();
            }            
        }

        /// <summary>
        /// Update to the view if there is a next one, otherwise finish this activity
        /// </summary>
        private void LoadNextMatchedRequest()
        {
            if (++this.currentMatchedUserIndex < this.matchedRequests.Count)
            {
                RunOnUiThread(() => this.UpdateDisplay());
            }
            else
            {
                Toast.MakeText(ApplicationContext, Resources.GetString(Resource.String.matchNoMoreMatch), ToastLength.Long).Show();
                this.Done();
            }
        }

        /// <summary>
        /// Goes to the UserCancelRideActivity when the matching session is finished
        /// </summary>
        private void Done()
        {
            Intent intent = new Intent(this, typeof(UserCancelRideActivity));
            this.StartActivity(intent);
            this.Finish();
        }

        
        
        // ---------------------------------------------------------------------------------------------------------
        // Event methods that require changing activity
        // ---------------------------------------------------------------------------------------------------------

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
            chatIntent.PutExtra("id", matchedUserId.ToString());
            StartActivity(chatIntent);
        }



        // ---------------------------------------------------------------------------------------------------------
        // Supportive methods
        // ---------------------------------------------------------------------------------------------------------

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
        

        public string GetReverseGeoCodeEndApi(string latitude, string longitude)
        {
            return "json?latlng=" + latitude + "," + longitude;
        }

        /// <summary>
        /// Sets the clickable elements that relies on the response of the asynchronous call to be
        /// visible. 
        /// </summary>
        public void SetClickableElementsVisible()
        {
            // Enable back the clicks on the elements
            this.matchedUserPicture.Visibility = ViewStates.Visible;
            this.chatButton.Visibility = ViewStates.Visible;
            this.acceptButton.Visibility = ViewStates.Visible;
            this.declineButton.Visibility = ViewStates.Visible;
        }



        // ---------------------------------------------------------------------------------------------------------
        // Methods that communicate with external actors and that may require more time to complete, thus
        // asynchronous
        // ---------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Sets the user profile picture given its Facebook ID.
        /// </summary>
        /// <param name="facebookId">Facebook ID of the user whose profile picture is to be set</param>
        public async void SetMatchedUserProfilePicture(long facebookId)
        {
            Bitmap userPicture = await userMapper.GetUserFacebookProfilePictureAsync(facebookId);
            if (userPicture != null)
            {
                RunOnUiThread(() => this.matchedUserPicture.SetImageBitmap(userPicture));
            }
        }

        /// <summary>
        /// Sets the address of the matched user's location
        /// </summary>
        /// <param name="latitude">Latitude coordinate</param>
        /// <param name="longitude">Longitude coordinate</param>
        public async void SetMatchedUserLocation(double latitude, double longitude)
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addresses = await geocoder.GetFromLocationAsync(latitude, longitude, 1);
            RunOnUiThread(() => this.matchedUserFrom.Text = addresses[0].GetAddressLine(0));
        }

        /// <summary>
        /// Sets the address of the matched user's destination
        /// </summary>
        /// <param name="latitude">Latitude coordinate</param>
        /// <param name="longitude">Longitude coordinate</param>
        public async void SetMatchedUserDestination(double latitude, double longitude)
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addresses = await geocoder.GetFromLocationAsync(latitude, longitude, 1);
            RunOnUiThread(() => this.matchedUserGoingTo.Text = addresses[0].GetAddressLine(0));
        }



        // ---------------------------------------------------------------------------------------------------------
        // Callback methods, invoked when an asynchronous task is complete
        // ---------------------------------------------------------------------------------------------------------

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
            if (this.userType == Request.TYPE_DRIVER)
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
            builder.Zoom(15);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            this.matchedUserLocationMap.AnimateCamera(cameraUpdate);
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
                RunOnUiThread(() => {
                    this.SetClickableElementsVisible();
                    this.UpdateDisplay();
                });
            }
            else
            {
                // Display some message to the user
                Toast.MakeText(ApplicationContext, Resources.GetString(Resource.String.matchNoMatchFound), ToastLength.Long).Show();

                // Go back to the main menu, clearing the activity stack
                Intent intent = new Intent(this, typeof(MainMenuActivity));
                intent.AddFlags(ActivityFlags.NewTask);
                intent.AddFlags(ActivityFlags.ClearTask);
                this.StartActivity(intent);
                Finish();
            }
        }

        /// <summary>
        /// Method called when the average number of stars is computed
        /// </summary>
        /// <param name="feedbacks"></param>
        public void OnGetReviewsComplete(double feedbacksAverage)
        {
            RunOnUiThread(() => this.matchedUserRatingBar.Rating = (int)Math.Round(feedbacksAverage));
        }
    }
}