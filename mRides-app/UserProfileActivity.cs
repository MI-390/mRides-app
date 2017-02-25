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

namespace mRides_app
{
    [Activity(Label = "UserProfileActivity")]
    public class UserProfileActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //Displaying username based on name sent after authentication
            //In the future, can be changed to get username from DB
            TextView usernameText;
            String username = Intent.GetStringExtra("Profile Info") ?? "Data not available";
            SetContentView(Resource.Layout.UserProfile);
            usernameText = FindViewById<TextView>(Resource.Id.userName);
            usernameText.Text = username;


            //for testing purposes
            User myuser = new User();
            myuser.id = 10;
            myuser.firstName = "Nassim";
            myuser.lastName = "ES";
            myuser.gsd = 0;
            myuser.hasLuggage = false;
            myuser.isHandicap = true;
            myuser.isSmoker = true;
            myuser.preferredLanguage = "French";
            myuser.numOfFeedback = 1;

            //testing purposes
            UserRides us = new UserRides();
            us.rideId = 1;
            userRideId = 1;
            us.riderId = 10;
            us.driverId = 11;
            us.riderFeedback = "Good driver";
            us.driverFeedback = "Good rider";
            us.riderRating = 4;
            us.driverRating = 5;
            us.location = "Toronto";
            us.dateOfRide = DateTime.Today;

            //display as many feedback fragments as there are feedbacks for user
            for (int i = 0; i < myuser.numOfFeedback; i++)
            {
                //If user has reviews in the database, display the fragment for it
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                UserTypeFragment dialog = new UserProfileFeedbackFragment();
                dialog.Show(transaction, "User Profile Feedback Fragment");

                transaction.add(Resource.Id.scrollingReviews);
            }

        }
    }
}