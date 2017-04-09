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
using Android.Graphics;
using Android.Gms.Common.Apis;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Location;
using Android.Gms.Common;
using Android.Locations;
using Android.Gms.Location.Places;
using Android.Gms.Location.Places.UI;
using Android.Util;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Json;
using Newtonsoft.Json;
using mRides_app.Models;
using mRides_app.Mappers;
using mRides_app.Constants;
using Android.Content.PM;
using static mRides_app.Models.Request;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace mRides_app
{
    [Activity(Label = "MapActivity")]
    public class MapActivity : Activity, IOnMapReadyCallback, Android.Gms.Location.ILocationListener,
        GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, IPlaceSelectionListener,
        IEditUserSelectionListener, IStartDrivingModeListener
    {
        private Android.Gms.Maps.GoogleMap map;
        private GoogleApiClient googleApiClient;
        private LocationRequest locationRequest;
        private bool locationPermissionGranted;
        private Marker originMarker;
        private Marker destinationMarker;
        private Button modifyDestinationButton;
        private Button confirmRideButton;
        private LatLng userLocation;
        private bool mapButtonClicked;
        private Dictionary<User, Marker> usersOnMap;
        private List<LatLng> directionList;
        private List<Request> requestList;
        private DestinationJSON destinationData;
        private PlaceAutocompleteFragment autocompleteFragment;
        private Android.Gms.Maps.Model.Polyline polyline;
        private const string googleApiKey = "AIzaSyAz9p6O99w8ZWkFUbaREJXmnj01Mpm19dA";
        private bool selectingDestination;
        private bool selectingOrigin;
        private bool confirmingRide;
        int numberOfPeople;
        string pathURL;


        protected override void OnCreate(Bundle bundle)
        {
            User.currentUser.currentType = mRides_app.Models.Request.TYPE_DRIVER;
            UserMapper.getInstance().setTheme(this);

            base.OnCreate(bundle);

            // Create an instance of GoogleAPIClient.
            if (googleApiClient == null)
            {
                googleApiClient = new GoogleApiClient.Builder(this, this, this)
                     .AddApi(LocationServices.API)
                     .AddApi(PlacesClass.GEO_DATA_API)
                     .AddApi(PlacesClass.PLACE_DETECTION_API)
                     .Build();
                // generate a location request that we will pass into a call for location updates
                googleApiClient.Connect();
            }

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Destination);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "mRides";

            // Retrieve the PlaceAutocompleteFragment.
            autocompleteFragment = (PlaceAutocompleteFragment)FragmentManager.FindFragmentById(Resource.Id.place_autocomplete_fragment);
            // Register a listener to receive callbacks when a place has been selected or an error has occurred.
            autocompleteFragment.SetOnPlaceSelectedListener(this);
            autocompleteFragment.SetHint(GetString(Resource.String.dest) + "?");

            // Alert Dialog
            openDestinationAlertDialog();

            modifyDestinationButton = (Button)FindViewById(Resource.Id.modifyDestinationButton);
            modifyDestinationButton.Visibility = ViewStates.Invisible;

            confirmRideButton = (Button)FindViewById(Resource.Id.confirmRideButton);
            confirmRideButton.Visibility = ViewStates.Invisible;

            modifyDestinationButton.Click += OnModifyDestinationButtonClick;
            confirmRideButton.Click += OnConfirmRideButtonClick;

        }

        void OnModifyDestinationButtonClick(object sender, EventArgs e)
        {
            if (selectingOrigin)
            {
                selectingOrigin = false;
                confirmingRide = false;
                confirmRideButton.Visibility = ViewStates.Invisible;
                modifyDestinationButton.Visibility = ViewStates.Invisible;
                autocompleteFragment.SetHint(GetString(Resource.String.dest) + "?");
                autocompleteFragment.SetText("");
                destinationMarker.Visible = false;
                openDestinationAlertDialog();
            }
        }

        void OnConfirmRideButtonClick(object sender, EventArgs e)
        {
            confirmRideButton.Visibility = ViewStates.Invisible;
            modifyDestinationButton.Visibility = ViewStates.Invisible;
            selectingOrigin = false;
            confirmingRide = false;

            setDestinationList();

            // Prepare the match activity
            List<DestinationCoordinate> destinationCoordinates = this.getFormattedDirectionList();
            Intent matchActivity = new Intent(this, typeof(MatchActivity));
            matchActivity.PutExtra(Constants.IntentExtraNames.RouteCoordinatesJson, JsonConvert.SerializeObject(destinationCoordinates));
            matchActivity.PutExtra(Constants.IntentExtraNames.RequestType, User.currentUser.currentType);
            StartActivity(matchActivity);
        }

        public void OnMapReady(Android.Gms.Maps.GoogleMap googleMap)
        {
            map = googleMap;
            map.MarkerClick += OnMarkerClick;
            if (locationPermissionGranted)
            {
                map.MyLocationEnabled = true;
                map.UiSettings.MyLocationButtonEnabled = true;
                map.UiSettings.SetAllGesturesEnabled(true);
                map.SetPadding(0, 250, 0, 0);
                map.MyLocationButtonClick += OnMyLocationButtonClick;
            }
            if (User.currentUser.latitude != null && User.currentUser.longitude != null)
            UpdateCameraPosition(new LatLng(User.currentUser.latitude, User.currentUser.longitude));
        }

        //When the user clicks on a marker
        private void OnMarkerClick(object sender, Android.Gms.Maps.GoogleMap.MarkerClickEventArgs e)
        {
            if (!e.Marker.Equals(destinationMarker) && !e.Marker.Equals(originMarker))
            {
                foreach (KeyValuePair<User, Marker> option in usersOnMap)
                {
                    if (e.Marker.Equals(option.Value))
                    {
                        Bundle args = new Bundle();
                        args.PutString("name", e.Marker.Title);
                        args.PutString("id", option.Key.id.ToString());
                        args.PutString("location", option.Value.Position.Latitude.ToString() + "," + option.Value.Position.Longitude.ToString());
                        FragmentTransaction transaction = FragmentManager.BeginTransaction();
                        UserProfileFragment dialog = new UserProfileFragment();
                        dialog.Arguments = args;
                        dialog.Show(transaction, "User profile fragment");
                    }
                }
            }
        }

        private void OnMyLocationButtonClick(object sender, Android.Gms.Maps.GoogleMap.MyLocationButtonClickEventArgs e)
        {
            LatLng position = new LatLng(User.currentUser.latitude, User.currentUser.longitude);
            UpdateCameraPosition(position);
        }

        /// <summary>
        /// Update the camera position to a latitude/longitude coordinate position
        /// </summary>
        /// <param name="position"> The position to move the camera to.</param>
        private void UpdateCameraPosition(LatLng position)
        {
            if (map != null && position != null)
            {
                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(position);
                builder.Zoom(15);
                CameraPosition cameraPosition = builder.Build();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                map.AnimateCamera(cameraUpdate);
            }
        }

        /// <summary>
        /// Update the camera position to see all the markers shown on the map
        /// </summary>
        private void UpdateCameraPositionToMarkers()
        {
            int padding = 100; // offset from edges of the map in pixels
            LatLngBounds.Builder builder = new LatLngBounds.Builder();
            builder.Include(destinationMarker.Position);
            if (originMarker != null)
                builder.Include(originMarker.Position);
            else
                builder.Include(new LatLng(User.currentUser.latitude, User.currentUser.longitude));
            LatLngBounds bounds = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngBounds(bounds, padding);
            map.AnimateCamera(cameraUpdate);
        }

        /// <summary>
        /// Get the user's current location using the GoogleApiClient
        /// </summary>
        private void getCurrentLocation()
        {
            if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Granted)
                locationPermissionGranted = true;

            if (locationPermissionGranted)
            {
                locationRequest = LocationRequest.Create();
                locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy); //HIGH ACCURACY
                locationRequest.SetFastestInterval(500);
                locationRequest.SetInterval(1000);
                LocationServices.FusedLocationApi.RequestLocationUpdates(googleApiClient, locationRequest, this);
                Location lastUserLocation = LocationServices.FusedLocationApi.GetLastLocation(googleApiClient);
                if (lastUserLocation != null)
                    userLocation = new LatLng(lastUserLocation.Latitude, lastUserLocation.Longitude);
            }
        }

        /// <summary>
        /// After the origin and destination were selected, setDestinationData gets asynchronously in JSON format
        /// the response from the Google Web Service DirectionAPI.
        /// </summary>
        /// <param name="url"> The url used to call the Google web service.</param>
        public async Task setDestinationList()
        {
            string pathURL = ("https://maps.googleapis.com/maps/api/directions/json?" +
           "origin=" + originMarker.Position.Latitude + "," + originMarker.Position.Longitude +
           "&destination=" + destinationMarker.Position.Latitude + "," + destinationMarker.Position.Longitude +
           "&key=" + googleApiKey);
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(pathURL));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (WebResponse response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response:
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        using (Newtonsoft.Json.JsonReader jsonReader = new JsonTextReader(streamReader))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            destinationData = serializer.Deserialize<DestinationJSON>(jsonReader);
                            if (destinationData != null)
                            {
                                directionList = getDirectionList(destinationData.routes[0].overview_polyline);
                                //Show the polyline directions on the map
                                displayPathOnMap(directionList);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Send coordinates to the server to get a list of users and display them on the map
        /// </summary>
        /// <param name="directionList"> The formatted LatLng list of direction coordinates obtained from the Google Directions API.</param>
        public void findNearbyUsers(List<LatLng> directionList)
        {
            List<DestinationCoordinate> destinationCoordinates = getFormattedDirectionList();

            Request newRequest = new Request
            {
                destinationCoordinates = destinationCoordinates,
                destination = destinationCoordinates.Last().coordinate,
                location = destinationCoordinates.First().coordinate,
                type = "driver"
            };
            newRequest.destinationCoordinates = destinationCoordinates;
            User.currentUser.requestsAsDriver = ConsoleMapper.getInstance().FindRiders(newRequest);
            showNearbyUsers();
        }

        /// <summary>
        /// Obtain the formatted list of coordinates.
        /// </summary>
        /// <returns>List of string representing the coordinates of the directions</returns>
        public List<DestinationCoordinate> getFormattedDirectionList()
        {
            List<DestinationCoordinate> destinationCoordinates = new List<DestinationCoordinate>();

            for (int i = 0; i < directionList.Count; i += 10)
            {
                DestinationCoordinate destinationCoordinate = new DestinationCoordinate
                {
                    coordinate = directionList[i].Latitude.ToString() + "," + directionList[i].Longitude.ToString()
                };
                destinationCoordinates.Add(destinationCoordinate);
            }
            return destinationCoordinates;
        }

        /// <summary>
        /// Method to display the polyline path from the current user location to the destination.
        /// </summary>
        /// <param name="directionList">  The formatted LatLng list of direction coordinates obtained from the Google Directions API.</param>
        public void displayPathOnMap(List<LatLng> directionList)
        {
            PolylineOptions polylineOptions = new PolylineOptions().Geodesic(true).InvokeColor(Color.Blue).InvokeWidth(5).Clickable(true);
            for (int i = 0; i < directionList.Count - 1; i++)
            {
                LatLng start = directionList[i];
                LatLng destination = directionList[i + 1];
                polylineOptions.Add(start, destination);
            }

            if (polyline != null)
                polyline.Remove();
            if (map != null)
                polyline = map.AddPolyline(polylineOptions);
        }

        /// <summary>
        /// Method user to get the list of directions from the deserialized JSON obtained from the Google Directions API result.
        /// </summary>
        /// <param name="overview"> JSON Object of encoded polyline points.</param>
        public List<LatLng> getDirectionList(OverviewPolyline overview)
        {
            OverviewPolyline overviewPolyline = overview;
            String encodedPolyline = overviewPolyline.points;
            directionList = decodePolyline(encodedPolyline);
            return directionList;
        }

        /// <summary>
        /// Decodes an encoded polyline path string into a list of LatLngs. This code is from the com.google.maps.android:android-maps-utils library
        /// and was translated from java to C#.
        /// </summary>
        /// <param name="overview"> JSON Object of encoded polyline points</param>
        public static List<LatLng> decodePolyline(string encodedPolyline)
        {
            int len = encodedPolyline.Length;

            // For speed we preallocate to an upper bound on the final length, then truncate the array before returning.
            List<LatLng> path = new List<LatLng>();
            int index = 0;
            int lat = 0;
            int lng = 0;

            while (index < len)
            {
                int result = 1;
                int shift = 0;
                int b;
                do
                {
                    b = encodedPolyline[index++] - 63 - 1;
                    result += b << shift;
                    shift += 5;
                } while (b >= 0x1f);
                lat += (result & 1) != 0 ? ~(result >> 1) : (result >> 1);

                result = 1;
                shift = 0;
                do
                {
                    b = encodedPolyline[index++] - 63 - 1;
                    result += b << shift;
                    shift += 5;
                } while (b >= 0x1f);
                lng += (result & 1) != 0 ? ~(result >> 1) : (result >> 1);

                path.Add(new LatLng(lat * 1e-5, lng * 1e-5));
            }

            return path;
        }

        /// <summary>
        /// SHow users on the map along the polyline path.
        /// </summary>
        public void showNearbyUsers()
        {
            //Instantiate a new dictionary for the new destination
            usersOnMap = new Dictionary<User, Marker>();

            if (User.currentUser.requestsAsDriver != null)
            {
                foreach (Request request in User.currentUser.requestsAsDriver)
                {
                    if (request.riderRequests.First().rider.firstName != null && request.riderRequests.First().rider.lastName != null)
                    {
                        Android.Gms.Maps.Model.MarkerOptions userMarker = new Android.Gms.Maps.Model.MarkerOptions();
                        string[] splitCoordinates = request.riderRequests.First().location.Split(',');
                        userMarker.SetPosition(new LatLng(Double.Parse(splitCoordinates[0]), Double.Parse(splitCoordinates[1])))
                                  .SetTitle(request.riderRequests.First().rider.firstName.ToString() + " " + request.riderRequests.First().rider.lastName.ToString())
                                  .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.userIcon2)).Anchor(0.5f, 0.5f);
                        if (map != null)
                        {
                            Marker marker = map.AddMarker(userMarker);
                            usersOnMap.Add(request.riderRequests.First().rider, marker);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Used to start Google Maps Navigation to have real-time navigation system to pick up the selected user.
        /// </summary>
        /// <param name="latitude"> Destination latitude.</param>
        /// <param name="longitude"> Destination longitude.</param>
        public void enterDriverMode(double latitude, double longitude)
        {
            try
            {
                ApplicationInfo info = PackageManager.GetApplicationInfo("com.google.android.apps.maps", 0);
                Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://maps.google.com/maps?" + "saddr=" + User.currentUser.latitude + "," +
                User.currentUser.longitude + "&daddr=" + latitude + "," + longitude));
                intent.SetClassName("com.google.android.apps.maps", "com.google.android.maps.MapsActivity");
                StartActivity(intent);
            }
            catch (PackageManager.NameNotFoundException e)
            {
                Toast.MakeText(ApplicationContext, GetString(Resource.String.google_not_installed), ToastLength.Long).Show();
            }
        }

        /// <summary>
        /// Used to open the dialog to ask the users their preference on custom location or default location.
        /// </summary>
        private void openOriginAlertDialog()
        {
            string customMessage = "";
            AlertDialog.Builder originAlert = new AlertDialog.Builder(this, Resource.Style.AlertDialogCustom);

            //When user clicks on "Custom Location"
            originAlert.SetPositiveButton(GetString(Resource.String.custom_location), new EventHandler<DialogClickEventArgs>((senderAlert, args) =>
            {
                UpdateCameraPositionToMarkers();

                if (originMarker != null)
                {
                    confirmRideButton.Visibility = ViewStates.Visible;
                    modifyDestinationButton.Visibility = ViewStates.Visible;
                }
            }));

            //When user clicks on "Default Location"
            originAlert.SetNeutralButton(GetString(Resource.String.current_location), new EventHandler<DialogClickEventArgs>((senderAlert, args) =>
            {
                modifyDestinationButton.Visibility = ViewStates.Visible;
                confirmRideButton.Visibility = ViewStates.Visible;

                if (originMarker != null)
                {
                    originMarker.Position = new LatLng(User.currentUser.latitude, User.currentUser.longitude);
                    originMarker.Title = GetString(Resource.String.ride_request_location);
                }
                else
                    originMarker = map.AddMarker(new MarkerOptions().SetPosition(new LatLng(User.currentUser.latitude, User.currentUser.longitude)).SetTitle(GetString(Resource.String.ride_request_location)));

                if (destinationMarker != null && originMarker != null)
                    setDestinationList();

                UpdateCameraPositionToMarkers();
            }));

            if (User.currentUser.currentType == mRides_app.Models.Request.TYPE_RIDER)
                customMessage = GetString(Resource.String.choose_pickup);

            if (User.currentUser.currentType == mRides_app.Models.Request.TYPE_DRIVER)
                customMessage = GetString(Resource.String.choose_start);

            originAlert.SetTitle(GetString(Resource.String.origin));
            originAlert.SetMessage(GetString(Resource.String.location_question) + customMessage);
          
            AlertDialog originDialog = originAlert.Create();
            originDialog.SetCancelable(false);
            originDialog.SetCanceledOnTouchOutside(false);
            originDialog.Show();
            styleDialog(originDialog);
        }

        /// <summary>
        /// Used to open the dialog to welcome the user and proceed to select a destination.
        /// </summary>
        private void openDestinationAlertDialog()
        {
            AlertDialog.Builder destinationAlert = new AlertDialog.Builder(this, Resource.Style.AlertDialogCustom);
            destinationAlert.SetTitle(GetString(Resource.String.create_newride));
            destinationAlert.SetMessage(GetString(Resource.String.hello_map) + " " + mRides_app.Models.User.currentUser.firstName + ".\n" + GetString(Resource.String.choose_destination));

            //When user clicks on "Proceed"
            destinationAlert.SetPositiveButton(GetString(Resource.String.next), new EventHandler<DialogClickEventArgs>((senderAlert, args) =>
            {
                selectingDestination = true;
            }));

            AlertDialog destinationDialog = destinationAlert.Create();
            destinationDialog.SetCancelable(false);
            destinationDialog.SetCanceledOnTouchOutside(false);
            destinationDialog.Show();
            styleDialog(destinationDialog);
        }

        /// <summary>
        /// Used to open the dialog to ask the users the confirmation for the selected destination.
        /// </summary>
        private void openDestinationSelectionAlertDialog(string destination, LatLng coordinates)
        {
            AlertDialog.Builder destinationChoiceAlert = new AlertDialog.Builder(this, Resource.Style.AlertDialogCustom);
            destinationChoiceAlert.SetTitle(GetString(Resource.String.dest));
            destinationChoiceAlert.SetMessage(GetString(Resource.String.set_destination) + destination + "?");

            //When user clicks on "Yes"
            destinationChoiceAlert.SetPositiveButton(GetString(Resource.String.yes), new EventHandler<DialogClickEventArgs>((senderAlert, args) =>
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                UserTypeFragment dialog = new UserTypeFragment();
                dialog.Show(transaction, "User type fragment");

                if (destinationMarker != null)
                {
                    destinationMarker.Visible = true;
                    destinationMarker.Position = coordinates;
                    destinationMarker.Title = destination;
                }
                else
                    destinationMarker = map.AddMarker(new MarkerOptions().SetPosition(coordinates).SetTitle(destination));

                if (destinationMarker != null && originMarker != null)
                    setDestinationList();

                modifyDestinationButton.Visibility = ViewStates.Visible;
            }));

            //When user clicks on "No"
            destinationChoiceAlert.SetNegativeButton(GetString(Resource.String.no), new EventHandler<DialogClickEventArgs>((senderAlert, args) =>
            {
                return;
            }));

            AlertDialog destinationDialog = destinationChoiceAlert.Create();
            destinationDialog.SetCanceledOnTouchOutside(false);
            destinationDialog.SetCancelable(false);
            destinationDialog.Show();
            styleDialog(destinationDialog);
        }

        /// <summary>
        /// Used to open the dialog to ask the users the confirmation for the selected origin.
        /// </summary>
        private void openOriginSelectionAlertDialog(string origin, LatLng coordinates)
        {
            AlertDialog.Builder originChoiceAlert = new AlertDialog.Builder(this, Resource.Style.AlertDialogCustom);
            originChoiceAlert.SetTitle(GetString(Resource.String.origin));
            originChoiceAlert.SetMessage(GetString(Resource.String.set_origin) + origin + "?");

            //When user clicks on "Yes"
            originChoiceAlert.SetPositiveButton(GetString(Resource.String.yes), new EventHandler<DialogClickEventArgs>((senderAlert, args) =>
            {
                if (originMarker != null)
                {
                    originMarker.Position = coordinates;
                    originMarker.Title = origin;
                }
                else
                    originMarker = map.AddMarker(new MarkerOptions().SetPosition(coordinates).SetTitle(GetString(Resource.String.ride_request_location)));

                if (destinationMarker != null && originMarker != null)
                    setDestinationList();

                confirmRideButton.Visibility = ViewStates.Visible;
                UpdateCameraPositionToMarkers();
            }));

            //When user clicks on "No"
            originChoiceAlert.SetNegativeButton(GetString(Resource.String.no), new EventHandler<DialogClickEventArgs>((senderAlert, args) =>
            {
                return;
            }));

            AlertDialog originDialog = originChoiceAlert.Create();
            originDialog.SetCanceledOnTouchOutside(false);
            originDialog.SetCancelable(false);
            originDialog.Show();
            styleDialog(originDialog);
        }

        /// <summary>
        /// Used to style the dialog buttons according to the theme of driver/ride.
        /// </summary>
        /// <param name="dialog">The target dialog to style.</param>
        void styleDialog(AlertDialog dialog)
        {
            Button positiveButton = dialog.GetButton(-1);
            Button negativeButton = dialog.GetButton(-2);
            Button neutralButton = dialog.GetButton(-3);

            if (User.currentUser.currentType == mRides_app.Models.Request.TYPE_DRIVER || User.currentUser.currentType == mRides_app.Models.Request.TYPE_RIDER)
            {
                if (User.currentUser.currentType == mRides_app.Models.Request.TYPE_DRIVER)
                {
                    if (neutralButton != null)
                        neutralButton.SetBackgroundResource(Resource.Drawable.red_button);
                    if (positiveButton != null)
                        positiveButton.SetBackgroundResource(Resource.Drawable.red_button);
                    if (negativeButton != null)
                        negativeButton.SetBackgroundResource(Resource.Drawable.red_button);
                }
                else
                {
                    if (neutralButton != null)
                        neutralButton.SetBackgroundResource(Resource.Drawable.green_button);
                    if (positiveButton != null)
                        positiveButton.SetBackgroundResource(Resource.Drawable.green_button);
                    if (negativeButton != null)
                        negativeButton.SetBackgroundResource(Resource.Drawable.green_button);
                }
            }
            else
            {
                if (neutralButton != null)
                    neutralButton.SetBackgroundResource(Resource.Drawable.red_button);
                if (positiveButton != null)
                    positiveButton.SetBackgroundResource(Resource.Drawable.red_button);
                if (negativeButton != null)
                    negativeButton.SetBackgroundResource(Resource.Drawable.red_button);
            }
        }

        //Interface methods below

        protected override void OnResume()
        {
            base.OnResume();

            //Menu bottom bar
            var chatMenuButton = FindViewById<ImageButton>(Resource.Id.menu_chat);
            chatMenuButton.Click += delegate
            {
                Intent i = new Intent(this, typeof(ChatListActivity));
                StartActivity(i);
            };

            var userProfileButton = FindViewById<ImageButton>(Resource.Id.menu_user);
            userProfileButton.Click += delegate
            {
                Intent i = new Intent(this, typeof(UserProfileActivity));
                i.PutExtra("id", User.currentUser.id.ToString());
                StartActivity(i);
            };

            var mainMenuButton = FindViewById<ImageButton>(Resource.Id.menu_home);
            mainMenuButton.Click += delegate
            {
                Intent i = new Intent(this, typeof(MainMenuActivity));
                StartActivity(i);
            };

        }

        //When Google API Client is connected
        public void OnConnected(Bundle bundle)
        {
            getCurrentLocation();
            //GetMapAsync(this) invokes the OnMapReady operation when ready
            if (map == null)
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);

            if (userLocation != null)
            {
                User.currentUser.latitude = userLocation.Latitude;
                User.currentUser.longitude = userLocation.Longitude;
            }
        }

        //When Google API Client is disconnected
        public void OnDisconnected()
        {
        }

        public void OnConnectionFailed(ConnectionResult bundle)
        {
        }

        public void OnLocationChanged(Location location)
        {
            LatLng newLocation = new LatLng(location.Latitude, location.Longitude);
            User.currentUser.latitude = location.Latitude;
            User.currentUser.longitude = location.Longitude;

            autocompleteFragment.SetBoundsBias(new LatLngBounds(new LatLng(User.currentUser.latitude - 0.2, User.currentUser.longitude - 0.2),
                                                                new LatLng(User.currentUser.latitude + 0.2, User.currentUser.longitude + 0.2)));
        }

        public void OnConnectionSuspended(int i)
        {
        }

        public void OnError(Statuses status)
        {
            Log.Info("Xamarin ", "Error has occured");
        }

        public void OnPlaceSelected(IPlace place)
        {
            if (selectingDestination)
            {
                string destination = place.NameFormatted.ToString();
                string usr_destination = GetString(Resource.String.dest);
                string pathURL;
                // Alert Dialog
                openDestinationSelectionAlertDialog(destination, place.LatLng);
            }
            else
            if (selectingOrigin)
            {
                string origin = place.NameFormatted.ToString();
                string pathURL;
                //Alert Dialog
                openOriginSelectionAlertDialog(origin, place.LatLng);
            }
        }

        /// <summary>
        /// Overriden method from interface of UserTypeFragment.cs. Used to update the user selection on whether to continue as a rider
        /// or a driver and the number of people in his company.
        /// </summary>
        /// <param name="number">The number of people with the user.</param>
        /// <param name="type">The type of the user (driver or ride).</param>
        public void updateUserSelection(string type, int number)
        {
            //Set user to selected type
            User.currentUser.currentType = type;
            numberOfPeople = number;
            openOriginAlertDialog();
            selectingOrigin = true;
            selectingDestination = false;
            autocompleteFragment.SetText("");
            autocompleteFragment.SetHint(GetString(Resource.String.origin) + "?");
            //// Get the list of coordinates

            // For multilingual purposes
            string typeDisplayed = "";
            string usrType = GetString(Resource.String.user_type);
            string userDriver = GetString(Resource.String.user_driver);
            string userRider = GetString(Resource.String.user_rider);
            string numOfPeople = GetString(Resource.String.number_of_people);

            if (type == mRides_app.Models.Request.TYPE_DRIVER || type == mRides_app.Models.Request.TYPE_RIDER)
            {
                if (type == mRides_app.Models.Request.TYPE_DRIVER)
                {
                    typeDisplayed = userDriver;
                    //Manually setting the theme color since you can only set the theme when creating a new activity
                    Window.SetNavigationBarColor(new Android.Graphics.Color(Color.ParseColor("#ba3c39")));
                    Window.SetStatusBarColor(new Android.Graphics.Color(Color.ParseColor("#ba3c39")));
                    ActionBar.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Color.ParseColor("#EF5350")));
                    confirmRideButton.SetBackgroundResource(Resource.Drawable.red_button);
                    modifyDestinationButton.SetBackgroundResource(Resource.Drawable.red_button);
                }
                else
                {
                    typeDisplayed = userRider;
                    Window.SetNavigationBarColor(Android.Graphics.Color.DarkGreen);
                    Window.SetStatusBarColor(Android.Graphics.Color.DarkGreen);
                    ActionBar.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Color.ParseColor("#26A65B")));
                    confirmRideButton.SetBackgroundResource(Resource.Drawable.green_button);
                    modifyDestinationButton.SetBackgroundResource(Resource.Drawable.green_button);
                }
            }
        }
    }
}
