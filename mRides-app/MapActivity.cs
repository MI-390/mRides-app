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

namespace mRides_app
{
    [Activity(Label = "MapActivity")]
    public class MapActivity : Activity, IOnMapReadyCallback, Android.Gms.Location.ILocationListener,
        GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, IPlaceSelectionListener, IEditUserSelectionListener
    {
        private Android.Gms.Maps.GoogleMap map;
        private GoogleApiClient googleApiClient;
        private LocationRequest locationRequest;
        private Location lastUserLocation;
        private bool locationPermissionGranted;
        private string destination;
        private Marker destinationMarker;
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

            string text = Intent.GetStringExtra("Profile Info") ?? "Data not available";
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
            if (locationPermissionGranted)
            {
                map.MyLocationEnabled = true;
                map.UiSettings.MyLocationButtonEnabled = true;
                map.UiSettings.ZoomControlsEnabled = true;
                if (lastUserLocation != null)
                {
                    map.MyLocationButtonClick += OnMyLocationButtonClick;
                }
            }
        }

        private void OnMarkerClick(object sender, Android.Gms.Maps.GoogleMap.MarkerClickEventArgs e)
        {
            if (e.Marker.Equals(destinationMarker))
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                UserTypeFragment dialog = new UserTypeFragment();
                dialog.Show(transaction, "User type fragment");
            }
        }

        private void OnMyLocationButtonClick(object sender, Android.Gms.Maps.GoogleMap.MyLocationButtonClickEventArgs e)
        {
            LatLng position = new LatLng(lastUserLocation.Latitude, lastUserLocation.Longitude);
            PlaceAutocompleteFragment autocompleteFragment = (PlaceAutocompleteFragment)FragmentManager.FindFragmentById(Resource.Id.place_autocomplete_fragment);
            UpdateCameraPosition(position);
        }

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

        protected override void OnResume()
        {
            base.OnResume();
            if (googleApiClient.IsConnected)
                getCurrentLocation();
        }

        ////Interface methods

        public void OnConnected(Bundle bundle)
        {
            if (map == null)
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            getCurrentLocation();
        }

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
            Toast.MakeText(ApplicationContext, "Destination : " + destination, ToastLength.Long).Show();
            if (destinationMarker != null)
                map.Clear();
            destinationMarker = map.AddMarker(new MarkerOptions().SetPosition(place.LatLng).SetTitle(destination));

            string pathURL = ("https://maps.googleapis.com/maps/api/directions/json?" +
                              "origin=" + lastUserLocation.Latitude + "," + lastUserLocation.Longitude +
                              "&destination=" + place.LatLng.Latitude + "," + place.LatLng.Longitude +
                              "&key=" + googleApiKey);

            setDestinationData(pathURL);
            //&waypoints=optimize:true|via:-37.81223%2C144.96254%7Cvia:-34.92788%2C138.60008
            //Geocoder geocoder = new Geocoder(this);
            //IList<Address> addresses = null;
            //addresses = geocoder.GetFromLocation(place.LatLng.Latitude, place.LatLng.Longitude, 1);
            //string countryCode = addresses[0].CountryCode;
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
                                //Update the camera position to the destination
                                UpdateCameraPosition(new LatLng(destinationData.routes[0].legs[0].end_location.lat, destinationData.routes[0].legs[0].end_location.lng));
                            }
                        }
                    }              
                }
            }
        }

        public void showDirections()
        {
            OverviewPolyline overviewPolyline = destinationData.routes[0].overview_polyline;
            String encodedPolyline = overviewPolyline.points;
            List<LatLng> directionList = decodePolyline(encodedPolyline);
            PolylineOptions polylineOptions = new PolylineOptions().Geodesic(true).InvokeColor(Color.Blue).InvokeWidth(5);
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


        public void updateUserSelection(string type, int number)
        {
            userType = type;
            numberOfPeople = number;
            Toast.MakeText(ApplicationContext, "User type: " + userType + " Number of people: " + numberOfPeople, ToastLength.Long).Show();
        }


    }
}