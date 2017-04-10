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
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using mRides_app.Models;
using Android.Content.PM;
using mRides_app.Mappers;
using Android.Graphics;
using static mRides_app.Models.Request;

namespace mRides_app.Activities
{
    [Activity(Label = "RideActivity")]
    public class RideActivity : Activity, IOnMapReadyCallback, IStartDrivingModeListener, ILeaveReviewListener
    {
        private GoogleMap map;
        private Marker originMarker;
        private Marker destinationMarker;
        private Request request;
        private DestinationJSON destinationData;
        private List<LatLng> directionList;
        private List<RiderRequest> riderRequestsList;
        private Dictionary<RiderRequest, Marker> ridersOnMap;
        private Ride ride;
        private Android.Gms.Maps.Model.Polyline polyline;
        private const string googleApiKey = "AIzaSyAz9p6O99w8ZWkFUbaREJXmnj01Mpm19dA";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RideMap);
            FragmentManager.FindFragmentById<MapFragment>(Resource.Id.rideMap).GetMapAsync(this);
        }

        /// <summary>
        /// Gets the ride selected by the user to display it on the map
        /// </summary>
        private void getRideRequest()
        {
            //Get the ride request
            int requestId = Int32.Parse(Intent.GetStringExtra("id"));
            request = ConsoleMapper.getInstance().GetRequest(requestId);
            //Convert Request to Ride via Json
            ride =JsonConvert.DeserializeObject<Ride>(JsonConvert.SerializeObject(request));
            ride.ID = 0;
            ride.UserRides = JsonConvert.DeserializeObject<List<UserRides>>(JsonConvert.SerializeObject(request.riderRequests));

            string[] splitLocationCoordinates = request.location.Split(',');
            string[] splitDestinationCoordinates = request.destination.Split(',');
            LatLng location = new LatLng(Double.Parse(splitLocationCoordinates[0]), Double.Parse(splitLocationCoordinates[1]));
            LatLng destination = new LatLng(Double.Parse(splitDestinationCoordinates[0]), Double.Parse(splitDestinationCoordinates[1]));

            originMarker = map.AddMarker(new MarkerOptions().SetPosition(location).SetTitle(GetString(Resource.String.ride_request_location)));
            destinationMarker = map.AddMarker(new MarkerOptions().SetPosition(destination).SetTitle(GetString(Resource.String.ride_request_location)));
            riderRequestsList = new List<RiderRequest>();
            foreach (RiderRequest riderRequest in request.riderRequests)
                riderRequestsList.Add(riderRequest);

            showNearbyUsers();
        }

        public void OnMapReady(Android.Gms.Maps.GoogleMap googleMap)
        {
            map = googleMap;
            map.MarkerClick += OnMarkerClick;
            map.MyLocationEnabled = true;
            map.UiSettings.MyLocationButtonEnabled = true;
            map.UiSettings.SetAllGesturesEnabled(true);
            map.MyLocationButtonClick += OnMyLocationButtonClick;
            getRideRequest();
            UpdateCameraPositionToMarkers();
        }

        //When the user clicks on a marker
        private void OnMarkerClick(object sender, Android.Gms.Maps.GoogleMap.MarkerClickEventArgs e)
        {
            if (!e.Marker.Equals(destinationMarker) && !e.Marker.Equals(originMarker))
            {
                foreach (KeyValuePair<RiderRequest, Marker> option in ridersOnMap)
                {
                    if (e.Marker.Equals(option.Value))
                    {
                        Bundle args = new Bundle();
                        args.PutString("name", e.Marker.Title);
                        args.PutString("id", option.Key.riderID.ToString());
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
            LatLng position = new LatLng(originMarker.Position.Latitude, originMarker.Position.Longitude);
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
        /// Get the optimized path with all the waypoints for the users participating in the ride
        /// </summary>
        private void getOptimizedPath()
        {
            if (ridersOnMap != null)
            {
                if (ridersOnMap.Count > 0)
                {
                    string pathURL = "";
                    string waypointString = "&waypoints=optimize:true";
                    foreach (KeyValuePair<RiderRequest, Marker> option in ridersOnMap)
                    {
                        waypointString += "|" + option.Value.Position.Latitude + "," + option.Value.Position.Longitude;
                    }

                    if (originMarker != null)
                    {
                        pathURL = ("https://maps.googleapis.com/maps/api/directions/json?" +
                       "origin=" + originMarker.Position.Latitude + "," + originMarker.Position.Longitude +
                       "&destination=" + destinationMarker.Position.Latitude + "," +
                      destinationMarker.Position.Longitude + waypointString +
                       "&key=" + googleApiKey);
                    }

                    setDestinationList(pathURL);
                }
            }
        }

        /// <summary>
        /// Method user to get the list of directions from the deserialized JSON obtained from the Google Directions API result.
        /// </summary>
        /// <param name="overview"> JSON Object of encoded polyline points.</param>
        public List<LatLng> getDirectionList(OverviewPolyline overview)
        {
            OverviewPolyline overviewPolyline = overview;
            String encodedPolyline = overviewPolyline.points;
            directionList = MapActivity.decodePolyline(encodedPolyline);
            return directionList;
        }

        /// <summary>
        /// Show users on the map along the polyline path.
        /// </summary>
        public void showNearbyUsers()
        {
            //Instantiate a new dictionary for the destination
            ridersOnMap = new Dictionary<RiderRequest, Marker>();
            foreach (RiderRequest request in riderRequestsList)
                {
                    if (request.rider.firstName != null && request.rider.lastName != null)
                    {
                        Android.Gms.Maps.Model.MarkerOptions userMarker = new Android.Gms.Maps.Model.MarkerOptions();
                        string[] splitCoordinates = request.location.Split(',');
                        userMarker.SetPosition(new LatLng(Double.Parse(splitCoordinates[0]), Double.Parse(splitCoordinates[1])))
                                  .SetTitle(request.rider.firstName.ToString() + " " + request.rider.lastName.ToString())
                                  .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.userIcon2)).Anchor(0.5f, 0.5f);
                        if (map != null)
                        {
                            Marker marker = map.AddMarker(userMarker);
                            ridersOnMap.Add(request, marker);
                        }
                    }
                }
            getOptimizedPath();
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
        /// Used to start Google Maps Navigation to have real-time navigation system to pick up the selected user.
        /// </summary>
        /// <param name="latitude"> Destination latitude.</param>
        /// <param name="longitude"> Destination longitude.</param>
        public void enterDriverMode(double latitude, double longitude)
        {
            try
            {
                ApplicationInfo info = PackageManager.GetApplicationInfo("com.google.android.apps.maps", 0);
                Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://maps.google.com/maps?" + "saddr=" + originMarker.Position.Latitude + "," +
                originMarker.Position.Longitude + "&daddr=" + latitude + "," + longitude));
                intent.SetClassName("com.google.android.apps.maps", "com.google.android.maps.MapsActivity");
                StartActivity(intent);
            }
            catch (PackageManager.NameNotFoundException e)
            {
                Toast.MakeText(ApplicationContext, "Google Maps is not installed", ToastLength.Long).Show();
            }
        }

        /// <summary>
        /// This method is used to remove a rider from the map after being dropped
        /// </summary>
        /// <param name="id">The id of the rider</param>
        public void droppedUser(int id)
        {
            RiderRequest riderToRemove = null;
            foreach (KeyValuePair<RiderRequest, Marker> option in ridersOnMap)
            {
                if (option.Key.riderID == id)
                {
                    riderToRemove = option.Key;
                }
            }

            if (riderToRemove != null)
            {
                ridersOnMap[riderToRemove].Visible = false;
                ridersOnMap.Remove(riderToRemove);
            }
        }

        /// <summary>
        /// After the origin and destination were selected, setDestinationData gets asynchronously in JSON format
        /// the response from the Google Web Service DirectionAPI.
        /// </summary>
        /// <param name="url"> The url used to call the Google web service.</param>
        public async Task setDestinationList(string pathURL)
        {
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

                            ConsoleMapper.getInstance().setDistanceTravelled(Int32.Parse(Intent.GetStringExtra("id")), (double)destinationData.routes[0].legs[0].distance.value);
                            ConsoleMapper.getInstance().setDurationTime(Int32.Parse(Intent.GetStringExtra("id")), (long)destinationData.routes[0].legs[0].duration.value);

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
        /// Overriden interface method when a review was made from the fragment
        /// </summary>
        /// <param name="userID">Id of the user who made the review</param>
        /// <param name="rating">Rating given</param>
        /// <param name="review">Review given</param>
        public void reviewWasMade(int userID, int rating, string review)
        {
            foreach (UserRides ride in ride.UserRides)
            {
                if (ride.RiderId == userID)
                {
                    ride.riderFeedback = review;
                    ride.riderStars = rating;
                }
            }
        }
    }
}