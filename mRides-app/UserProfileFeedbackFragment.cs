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

namespace mRides_app
{
    class UserProfileFeedbackFragment : ListFragment
    {
        TextView dateTV;
        TextView nameTV;
        TextView reviewTV;
        RatingBar ratingBar;

        // Static method to create a new instance of this fragment
        public static UserProfileFeedbackFragment NewInstance(Bundle bundle)
        {
            UserProfileFeedbackFragment fragment = new UserProfileFeedbackFragment();
            fragment.Arguments = bundle;
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
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
            us.userRideId = 1;
            us.riderId = 10;
            us.driverId = 11;
            us.riderFeedback = "Good driver";
            us.driverFeedback = "Good rider";
            us.riderRating = 4;
            us.driverRating = 5;
            us.location = "Toronto";
            us.dateOfRide = DateTime.Today;


            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.UserProfileFeedbackFragment, container, false);
            dateTV = view.FindViewById<TextView>(Resource.Id.feedbackDateProfileFragment);
            nameTV = view.FindViewById<TextView>(Resource.Id.feedbackFragmentUserName);
            reviewTV = view.FindViewById<TextView>(Resource.Id.feedbackFragmentReview);
            ratingBar = view.FindViewById<RatingBar>(Resource.Id.feedbackFragmentRatingBar);

            return view;
        }

    }
}
