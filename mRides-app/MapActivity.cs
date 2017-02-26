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

namespace mRides_app
{
    [Activity(Label = "MapActivity")]
    public class MapActivity : Activity, IOnMapReadyCallback, Android.Gms.Location.ILocationListener,
        GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, IPlaceSelectionListener, IEditUserSelectionListener
    {
        private const double MATCH_DISTANCE = 1.0;
        private Android.Gms.Maps.GoogleMap map;
        private GoogleApiClient googleApiClient;
        private LocationRequest locationRequest;
        private Location lastUserLocation;
        private bool locationPermissionGranted;
        private string destination;

        private Marker destinationMarker;
        private Dictionary<User, Marker> usersOnMap;
        private List<LatLng> directionList;
        private List<Request> requestList;
        private DestinationJSON destinationData;
        const string googleApiKey = "AIzaSyAz9p6O99w8ZWkFUbaREJXmnj01Mpm19dA";
        string userType;
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

            string text = mRides_app.Models.User.currentUser.firstName;
            Toast.MakeText(ApplicationContext, "Hello " + text, ToastLength.Long).Show();

            // Retrieve the PlaceAutocompleteFragment.
            PlaceAutocompleteFragment autocompleteFragment = (PlaceAutocompleteFragment)FragmentManager.FindFragmentById(Resource.Id.place_autocomplete_fragment);
            AutocompleteFilter typeFilter = new AutocompleteFilter.Builder().SetTypeFilter(AutocompleteFilter.TypeFilterEstablishment).Build();
            autocompleteFragment.SetHint("Destination?");
            autocompleteFragment.SetFilter(typeFilter);

            // Register a listener to receive callbacks when a place has been selected or an error has occurred.
            autocompleteFragment.SetOnPlaceSelectedListener(this);

            string userType;
            int numberOfPeople;
        }

        public void OnMapReady(Android.Gms.Maps.GoogleMap googleMap)
        {
            map = googleMap;
            map.MarkerClick += OnMarkerClick;
           //map.PolylineClick += OnPolylineClick;
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
                findUsers();
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
                        FragmentTransaction transaction = FragmentManager.BeginTransaction();
                        UserProfileFragment dialog = new UserProfileFragment();
                        dialog.Arguments = args;
                        dialog.Show(transaction, "User profile fragment");
                        //e.Marker.ShowInfoWindow();
                    }
                }
            }
        }

        //When the user clicks on a polyline
        //private void OnPolylineClick(object sender, Android.Gms.Maps.GoogleMap.PolylineClickEventArgs e)
        //{
        //    findUsers();
        //    if (usersOnMap != null)
        //    {
        //        foreach (KeyValuePair<User, MarkerOptions> option in usersOnMap)
        //            map.AddMarker(option.Value);
                
        //    }
        //}

        private void OnMyLocationButtonClick(object sender, Android.Gms.Maps.GoogleMap.MyLocationButtonClickEventArgs e)
        {
            LatLng position = new LatLng(lastUserLocation.Latitude, lastUserLocation.Longitude);
            PlaceAutocompleteFragment autocompleteFragment = (PlaceAutocompleteFragment)FragmentManager.FindFragmentById(Resource.Id.place_autocomplete_fragment);
            UpdateCameraPosition(position);
            if (usersOnMap != null)
            {
                findUsers();
            }
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

        async Task setDestinationData(string url)
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
                                //Show the polyline directions on the map
                                showDirections();
                                sendCoordinatesToServer();
                                //Update the camera position to the destination
                                UpdateCameraPosition(new LatLng(destinationData.routes[0].legs[0].end_location.lat, destinationData.routes[0].legs[0].end_location.lng));
                            }
                        }
                    }
                }
            }
        }

        //Send coordinates to the server and get a list of users
        public void sendCoordinatesToServer()
        {
            List<string> destinationCoordinates = new List<string>();
            for (int i = 0; i < directionList.Count; i += 10)
            {
                destinationCoordinates.Add(directionList[i].Latitude.ToString() + "," + directionList[i].Longitude.ToString());
            }

            User.currentUser = UserMapper.getInstance().CreateUser(new User());
            Request newRequest = new Request {
                destinationCoordinates = destinationCoordinates,
                destination= destinationCoordinates[destinationCoordinates.Count-1],
                location= destinationCoordinates[0],
                type="driver"
        };
            newRequest.destinationCoordinates = destinationCoordinates;
            requestList = ConsoleMapper.getInstance().FindRiders(newRequest);
        }

        //Method to display the polyline path from the current user location to the destination
        public void showDirections()
        {
            OverviewPolyline overviewPolyline = destinationData.routes[0].overview_polyline;
            String encodedPolyline = overviewPolyline.points;
            directionList = decodePolyline(encodedPolyline);
            PolylineOptions polylineOptions = new PolylineOptions().Geodesic(true).InvokeColor(Color.Blue).InvokeWidth(5).Clickable(true);
            for (int i = 0; i < directionList.Count - 1; i++)
            {
                LatLng start = directionList[i];
                LatLng destination = directionList[i + 1];
                polylineOptions.Add(start, destination);
            }

            Android.Gms.Maps.Model.Polyline polyline = map.AddPolyline(polylineOptions);
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

        //Find users along a path
        public void findUsers()
        {
            //Instantiate a new dictionary for the new destination
            usersOnMap = new Dictionary<User, Marker>();

            if (requestList != null)
            {               
                foreach (Request request in requestList)
                {
                    Android.Gms.Maps.Model.MarkerOptions userMarker = new Android.Gms.Maps.Model.MarkerOptions();
                    string[] splitCoordinates = request.riderRequests.First().location.Split(',');
                    userMarker.SetPosition(new LatLng(Double.Parse(splitCoordinates[0]), Double.Parse(splitCoordinates[1]))).SetTitle(request.riderRequests.First().rider.firstName.ToString() + " " + request.riderRequests.First().rider.lastName.ToString())
                              .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.userIcon2)).Anchor(0.5f, 0.5f);
                    Marker marker = map.AddMarker(userMarker);
                    usersOnMap.Add(request.riderRequests.First().rider, marker);                    
                }
            }
        }

        //Calculates distance between two degree coordinates using the Haversine formula
        //public double distanceBetweenTwoCoordinates(LatLng latlng1, LatLng latlng2)
        //{
        //    //Mean radius of the earth in km
        //    int earthRadius = 6371;
        //    double dLat = degreeToRadians(latlng2.Latitude - latlng1.Latitude);
        //    double dLng = degreeToRadians(latlng2.Longitude - latlng1.Longitude);
        //    double a = Math.Pow(Math.Sin(dLat / 2), 2) +
        //               Math.Cos(degreeToRadians(latlng1.Latitude)) * Math.Cos(degreeToRadians(latlng2.Latitude)) *
        //               Math.Pow(Math.Sin(dLng / 2), 2);
        //    double angle = 2 * Math.Asin(Math.Sqrt(a));
        //    return angle * earthRadius;
        //}

        ////Transforms degrees to radians unit
        //public double degreeToRadians(double degree)
        //{
        //    return degree * (Math.PI / 180);
        //}
        

        //Interface methods below
        protected override void OnResume()
        {
            base.OnResume();
            if (googleApiClient.IsConnected)
                getCurrentLocation();
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
            PlaceAutocompleteFragment autocompleteFragment = FragmentManager.FindFragmentById<PlaceAutocompleteFragment>(Resource.Id.place_autocomplete_fragment);
            autocompleteFragment.SetBoundsBias(new LatLngBounds(new LatLng(lastUserLocation.Latitude - 0.2, lastUserLocation.Longitude - 0.2), new LatLng(lastUserLocation.Latitude + 0.2, lastUserLocation.Longitude + 0.2)));
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
            destination = place.NameFormatted.ToString();
            string str1 = GetString(Resource.String.dest);
            Toast.MakeText(ApplicationContext, str1 + " : " + destination, ToastLength.Long).Show();

           
        
            if (destinationMarker != null)
                map.Clear();
            destinationMarker = map.AddMarker(new MarkerOptions().SetPosition(place.LatLng).SetTitle(destination));

            string pathURL = ("https://maps.googleapis.com/maps/api/directions/json?" +
                              "origin=" + lastUserLocation.Latitude + "," + lastUserLocation.Longitude +
                              "&destination=" + place.LatLng.Latitude + "," + place.LatLng.Longitude +
                              "&key=" + googleApiKey);

            setDestinationData(pathURL);

            //To add waypoints to the path
            /**&waypoints=optimize:true|via:-37.81223%2C144.96254%7Cvia:-34.92788%2C138.60008 */

            //To find the country code of the selection
            /**Geocoder geocoder = new Geocoder(this);
            //IList<Address> addresses = null;
            //addresses = geocoder.GetFromLocation(place.LatLng.Latitude, place.LatLng.Longitude, 1);
            //string countryCode = addresses[0].CountryCode;*/
        }


        //Overriden method from interface of UserTypeFragment.cs
        public void updateUserSelection(string type, int number)
        {
            userType = type;
            numberOfPeople = number;
            string str2 = GetString(Resource.String.user_type);
            string str3 = GetString(Resource.String.number_of_people);
            Toast.MakeText(ApplicationContext, str2 + " : " + userType + " " + str3 + " : " + numberOfPeople, ToastLength.Long).Show();
        }
    }
}