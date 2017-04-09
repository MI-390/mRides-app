using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using mRides_app.Models;
using mRides_app.Mappers;

namespace mRides_app
{
    /// <summary>
    /// Activity for canceling a ride
    /// </summary>
    [Activity(Label = "UserCancelRideActivity", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class UserCancelRideActivity : AppCompatActivity
    {
        ListView listView;
        List<Request> ridesList = new List<Request>();

        /// <summary>
        /// Method that is called when the activity is created
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.UserCancelRide);
            Button trashCanButton = FindViewById<Button>(Resource.Id.trashcanButton);
            Button skipButton = FindViewById<Button>(Resource.Id.skipButton);

            DisplayRides();

            trashCanButton.Click += CancelRideButtonClicked;
        }

        /// <summary>
        /// Method that is called after clicking the cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CancelRideButtonClicked(object sender, EventArgs e)
        {
            View v;
            for (int i = 0; i < ridesList.Count; i++)
            {
                v = listView.Adapter.GetView(i, null, null);
                if ((v.FindViewById(Resource.Id.CancelRides_Checkbox) as CheckBox).Checked)
                {
                    //Mappers.ConsoleMapper.Cancel();
                }
            }
        }

        /// <summary>
        /// Update the list of rides when there is a change
        /// </summary>
        public void OnDataChange(object sender, EventArgs e)
        {
            DisplayRides();
        }

        /// <summary>
        /// Method to display the rides in the list view
        /// </summary>
        private void DisplayRides()
        {
            ridesList.Clear();
            int usrID = User.currentUser.id;
            ridesList=UserMapper.getInstance().GetRequests(User.currentUser.id);
            

            listView = FindViewById<ListView>(Resource.Id.list_of_rides); // get reference to the ListView in the layout
            RidesAdapter adapter = new RidesAdapter(this, ridesList);
            listView.Adapter = new RidesAdapter(this, ridesList);
        }
    }
}