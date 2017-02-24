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
using Android.Gms.Common.Apis;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Location;
using Android.Gms.Common;
using Android.Locations;
using Android.Gms.Location.Places;
using Android.Gms.Location.Places.UI;
using Android.Util;

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
            string str1 = GetString(Resource.String.dest);
            Toast.MakeText(ApplicationContext, str1 + " : " + destination, ToastLength.Long).Show();

           
        
            if (destinationMarker != null)
                destinationMarker.Dispose();
            destinationMarker = map.AddMarker(new MarkerOptions().SetPosition(place.LatLng).SetTitle(destination));
            UpdateCameraPosition(place.LatLng);
            //Geocoder geocoder = new Geocoder(this);
            //IList<Address> addresses = null;
            //addresses = geocoder.GetFromLocation(place.LatLng.Latitude, place.LatLng.Longitude, 1);
            //string countryCode = addresses[0].CountryCode;
        }

        public void updateUserSelection(string type, int number)
        {
            userType = type;
            numberOfPeople = number;
            string str2 = GetString(Resource.String.user_type);
            string str3 = GetString(Resource.String.number_of_people);
            Toast.MakeText(ApplicationContext, str2 + ": " + userType + str3 + " : " + numberOfPeople, ToastLength.Long).Show();
        }


    }
}