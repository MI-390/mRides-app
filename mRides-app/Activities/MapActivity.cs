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
        private Location lastUserLocation;
        private bool locationPermissionGranted;
        private Marker originMarker;
        private Marker destinationMarker;
        private Button mapButton;
        private bool mapButtonClicked;
        private Dictionary<User, Marker> usersOnMap;
        private List<LatLng> directionList;
        private List<Request> requestList;
        private DestinationJSON destinationData;
        private PlaceAutocompleteFragment autocompleteFragment;
        private Android.Gms.Maps.Model.Polyline polyline;
        private const string googleApiKey = "AIzaSyAz9p6O99w8ZWkFUbaREJXmnj01Mpm19dA";
        int numberOfPeople;
     

        protected override void OnCreate(Bundle bundle)
        {
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
            
            //var toolbar_bot = FindViewById<BottomNavigationView>(Resource.Id.toolbar_bot);
            //toolbar_bot.InflateMenu(Resource.Menu.bottombar);

            string text = mRides_app.Models.User.currentUser.firstName;

            string helloMap = GetString(Resource.String.hello_map);
            Toast.MakeText(ApplicationContext, helloMap + " " + text, ToastLength.Long).Show();
            mapButton = (Button)FindViewById(Resource.Id.mapButton);
            mapButtonClicked = false;
            mapButton.Click += OnMapButtonClick;
            //Retrieve the PlaceAutocompleteFragment.
            autocompleteFragment = (PlaceAutocompleteFragment)FragmentManager.FindFragmentById(Resource.Id.place_autocomplete_fragment);
            autocompleteFragment.SetHint("Destination?");

            // Set appropriate color to the button
            if (User.currentUser != null)
            {
                if (User.currentUser.currentType == "rider")
                {
                    mapButton.SetBackgroundResource(Resource.Drawable.green_button);
                }
                else
                {
                    mapButton.SetBackgroundResource(Resource.Drawable.red_button);
                }
            }

            //AutocompleteFilter typeFilterDestination = new AutocompleteFilter.Builder().SetTypeFilter(AutocompleteFilter.TypeFilterEstablishment).Build();
            //autocompleteFragment.SetFilter(typeFilterDestination);
            // Register a listener to receive callbacks when a place has been selected or an error has occurred.
            autocompleteFragment.SetOnPlaceSelectedListener(this);
        }

        void OnMapButtonClick(object sender, EventArgs e)
        {
            if (!mapButtonClicked)
            {
                mapButtonClicked = true;
                mapButton.Text = "Choose destination";
                autocompleteFragment.SetHint("Origin?");
            }
            else
            {
                mapButtonClicked = false;
                mapButton.Text = "Choose origin";
                autocompleteFragment.SetHint("Destination?");
            }
        }

        public void OnMapReady(Android.Gms.Maps.GoogleMap googleMap)
        {
            map = googleMap;
            map.MarkerClick += OnMarkerClick;
            map.PolylineClick += OnPolylineClick;
            if (locationPermissionGranted)
            {
                map.MyLocationEnabled = true;
                map.UiSettings.MyLocationButtonEnabled = true;
                map.UiSettings.ZoomControlsEnabled = true;
                map.MyLocationButtonClick += OnMyLocationButtonClick;
                //map.SetOnMyLocationButtonClickListener(this);              
            }
        }

        //When the user clicks on a marker
        private void OnMarkerClick(object sender, Android.Gms.Maps.GoogleMap.MarkerClickEventArgs e)
        {
            if (e.Marker.Equals(destinationMarker))
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                UserTypeFragment dialog = new UserTypeFragment();
                dialog.Show(transaction, "User type fragment");
                showNearbyUsers();
            }
            else
            if (e.Marker.Equals(originMarker))
            {
                //
            }
            else
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

        //When the user clicks on a polyline
        private void OnPolylineClick(object sender, Android.Gms.Maps.GoogleMap.PolylineClickEventArgs e)
        {
            if (usersOnMap != null)
            {
                if (usersOnMap.Count > 0)
                {
                    string pathURL;
                    string waypointString = "&waypoints=optimize:true";
                    foreach (KeyValuePair<User, Marker> option in usersOnMap)
                    {
                        waypointString += "|" + option.Value.Position.Latitude + "," + option.Value.Position.Longitude;
                    }

                    if (originMarker != null)
                    {
                        pathURL = ("https://maps.googleapis.com/maps/api/directions/json?" +
                       "origin=" + originMarker.Position.Latitude + "," + originMarker.Position.Longitude +
                       "&destination=" + destinationData.routes[0].legs[destinationData.routes[0].legs.Count - 1].end_location.lat + "," +
                       destinationData.routes[0].legs[destinationData.routes[0].legs.Count - 1].end_location.lng + waypointString +
                       "&key=" + googleApiKey);
                    }
                    else
                    {
                        pathURL = ("https://maps.googleapis.com/maps/api/directions/json?" +
                            "origin=" + User.currentUser.latitude + "," + User.currentUser.longitude +
                            "&destination=" + destinationData.routes[0].legs[destinationData.routes[0].legs.Count - 1].end_location.lat + "," +
                            destinationData.routes[0].legs[destinationData.routes[0].legs.Count - 1].end_location.lng + waypointString +
                            "&key=" + googleApiKey);
                    }

                    setDestinationData(pathURL);
                }
            }
        }

        private void OnMyLocationButtonClick(object sender, Android.Gms.Maps.GoogleMap.MyLocationButtonClickEventArgs e)
        {
            LatLng position = new LatLng(User.currentUser.latitude, User.currentUser.longitude);
            getCurrentLocation();
            UpdateCameraPosition(position);
        }

        //Update the camera position to a latitude/longitude coordinate position
        public void UpdateCameraPosition(LatLng position)
        {
            if (map != null)
            {
                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(position);
                builder.Zoom(17);
                builder.Bearing(45);
                builder.Tilt(90);
                CameraPosition cameraPosition = builder.Build();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                map.AnimateCamera(cameraUpdate);
            }
        }


        //Get users current location
        public void getCurrentLocation()
        {
            if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Granted)
                locationPermissionGranted = true;
            else
                RequestPermissions(new string[] { Android.Manifest.Permission.AccessFineLocation }, 1); // 1 is the request code for AccessFineLocation


            if (locationPermissionGranted)
            {
                locationRequest = new LocationRequest();
                locationRequest.SetPriority(100);
                locationRequest.SetFastestInterval(500);
                locationRequest.SetInterval(1000);
                lastUserLocation = LocationServices.FusedLocationApi.GetLastLocation(googleApiClient);
                LocationServices.FusedLocationApi.RequestLocationUpdates(googleApiClient, locationRequest, this);
            }
        }

        public async Task setDestinationData(string url)
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
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
                                findNearbyUsers(directionList);
                                //Update the camera position to the destination
                                UpdateCameraPosition(new LatLng(destinationData.routes[0].legs[0].end_location.lat, destinationData.routes[0].legs[0].end_location.lng));
                            }
                        }
                    }
                }
            }
        }

        //Send coordinates to the server and get a list of users
        public void findNearbyUsers(List<LatLng> directionList)
        {
            List<DestinationCoordinate> destinationCoordinates = getFormattedDirectionList();
            
            Request newRequest = new Request {
                destinationCoordinates = destinationCoordinates,
                destination = destinationCoordinates.Last().coordinate,
                location = destinationCoordinates.First().coordinate,
                type = "driver"
            };
            newRequest.destinationCoordinates = destinationCoordinates;
            User.currentUser.requestsAsDriver = ConsoleMapper.getInstance().FindRiders(newRequest);
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

        //Method to display the polyline path from the current user location to the destination
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

        public List<LatLng> getDirectionList(OverviewPolyline overview)
        {
            OverviewPolyline overviewPolyline = overview;
           // destinationData.routes[0].overview_polyline;
            String encodedPolyline = overviewPolyline.points;
            directionList = decodePolyline(encodedPolyline);
            return directionList;
        }

        // Decodes an encoded polyline path string into a list of LatLngs. This code is from the com.google.maps.android:android-maps-utils library
        // and was translated from java to C#
        public static List<LatLng> decodePolyline(string encodedPolyline)
        {
            int len = encodedPolyline.Length;

            // For speed we preallocate to an upper bound on the final length, then
            // truncate the array before returning.
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

        //Show users along a path
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

        //Used to start Google Maps Navigation to have real-time navigation system to pick up the selected user
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
                Toast.MakeText(ApplicationContext, "Google Maps is not installed", ToastLength.Long).Show();
            }
        }
          
        //Interface methods below

        protected override void OnResume()
        {
            base.OnResume();
            if (googleApiClient.IsConnected)
                getCurrentLocation();

            //MenuBar
            var chatMenuButton = FindViewById<Button>(Resource.Id.menu_chat);
            chatMenuButton.Click += delegate
            {
                Intent i = new Intent(this, typeof(ChatListActivity));
                StartActivity(i);
            };

            //MenuBar
            var userProfileButton = FindViewById<Button>(Resource.Id.menu_user);
            userProfileButton.Click += delegate
            {
                Intent i = new Intent(this, typeof(UserProfileActivity));
                i.PutExtra("id", User.currentUser.id.ToString()); 
                StartActivity(i);
            };

        }

        //When Google API Client is connected
        public void OnConnected(Bundle bundle)
        {
            //GetMapAsync(this) invokes the OnMapReady operation
            if (map == null)
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            getCurrentLocation();
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
            if (lastUserLocation == null)
                UpdateCameraPosition(new LatLng(location.Latitude, location.Longitude));
            lastUserLocation = location;
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
            if (!mapButtonClicked)
            {
                string destination = place.NameFormatted.ToString();
                string usr_destination = GetString(Resource.String.dest);
                string pathURL;

                Toast.MakeText(ApplicationContext, usr_destination + " : " + destination, ToastLength.Long).Show();

                if (destinationMarker != null)
                {
                    destinationMarker.Position = place.LatLng;
                    destinationMarker.Title = destination;
                }                  
                else
                    destinationMarker = map.AddMarker(new MarkerOptions().SetPosition(place.LatLng).SetTitle(destination));
                if (originMarker != null)
                   pathURL = ("https://maps.googleapis.com/maps/api/directions/json?" +
                                "origin=" + originMarker.Position.Latitude + "," + originMarker.Position.Longitude +
                                "&destination=" + place.LatLng.Latitude + "," + place.LatLng.Longitude +
                                "&key=" + googleApiKey);
                else
                    pathURL = ("https://maps.googleapis.com/maps/api/directions/json?" +
                                  "origin=" + User.currentUser.latitude + "," + User.currentUser.longitude +
                                  "&destination=" + place.LatLng.Latitude + "," + place.LatLng.Longitude +
                                  "&key=" + googleApiKey);

                setDestinationData(pathURL);
            }

            if (mapButtonClicked)
            {
                string origin = place.NameFormatted.ToString();
                string pathURL;
                Toast.MakeText(ApplicationContext, "Origin" + " : " + origin, ToastLength.Long).Show();

                if (originMarker != null)
                {
                    originMarker.Position = place.LatLng;
                    originMarker.Title = origin;
                }
                else
                    originMarker = map.AddMarker(new MarkerOptions().SetPosition(place.LatLng).SetTitle(origin));

                UpdateCameraPosition(originMarker.Position);

                if (destinationMarker != null)
                {
                    pathURL = ("https://maps.googleapis.com/maps/api/directions/json?" +
                              "origin=" + originMarker.Position.Latitude + "," + originMarker.Position.Longitude +
                              "&destination=" + destinationMarker.Position.Latitude + "," + destinationMarker.Position.Longitude +
                              "&key=" + googleApiKey);
                    setDestinationData(pathURL);                   
                }
            }
        }

        public void OnOriginSelected(IPlace place)
        {

        }


        //Overriden method from interface of UserTypeFragment.cs
        public void updateUserSelection(string type, int number)
        {
            User.currentUser.currentType = type;
            numberOfPeople = number;
            string typeDisplayed = "";

            // Get the list of coordinates
            List<DestinationCoordinate> destinationCoordinates = this.getFormattedDirectionList();

            // For multilingual
            string usrType = GetString(Resource.String.user_type);
            string userDriver = GetString(Resource.String.user_driver);
            string userRider = GetString(Resource.String.user_rider);
            string numOfPeople = GetString(Resource.String.number_of_people);

            if (type == mRides_app.Models.Request.TYPE_DRIVER || type == mRides_app.Models.Request.TYPE_RIDER)
            {
                if (type == mRides_app.Models.Request.TYPE_DRIVER)
                {
                    typeDisplayed = userDriver;
                    Toast.MakeText(ApplicationContext, usrType + " : " + typeDisplayed, ToastLength.Long).Show();
                    // Manually setting the theme color since you can only set the theme when creating a new activity
                    Window.SetNavigationBarColor(new Android.Graphics.Color(Color.ParseColor("#ba3c39")));
                    Window.SetStatusBarColor(new Android.Graphics.Color(Color.ParseColor("#ba3c39")));
                    ActionBar.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Color.ParseColor("#ba3c39")));
                    mapButton.SetBackgroundResource(Resource.Drawable.red_button);
                }
                else
                {
                    typeDisplayed = userRider;
                    Toast.MakeText(ApplicationContext, usrType + " : " + typeDisplayed + " " + numOfPeople + " : " + numberOfPeople, ToastLength.Long).Show();
                    Window.SetNavigationBarColor(new Android.Graphics.Color(Color.ParseColor("#008000")));
                    Window.SetStatusBarColor(new Android.Graphics.Color(Color.ParseColor("#008000")));
                    ActionBar.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Color.ParseColor("#26A65B")));
                    mapButton.SetBackgroundResource(Resource.Drawable.green_button);
                }

                // Prepare the next activity
                Intent matchActivity = new Intent(this, typeof(MatchActivity));
                matchActivity.PutExtra(Constants.IntentExtraNames.RouteCoordinatesJson, JsonConvert.SerializeObject(destinationCoordinates));
                matchActivity.PutExtra(Constants.IntentExtraNames.RequestType, type);
                StartActivity(matchActivity);
            }
            
        }
    }
}