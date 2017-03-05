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
using mRides_app.Mappers;

namespace mRides_app
{
    class LeaveReviewFragment : DialogFragment
    {
        RatingBar ratingBar;
        Button sendReviewButton;
        EditText riderReview;
        int userID;
        int rating;
        string review;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            Bundle args = Arguments;
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.RiderReview, container, false);
            userID = Int32.Parse(args.GetString("id"));
            sendReviewButton = view.FindViewById<Button>(Resource.Id.submitFeedback2);
            ratingBar = view.FindViewById<RatingBar>(Resource.Id.ratingBarRider);
            riderReview = view.FindViewById<EditText>(Resource.Id.riderReviewEdit);
            ratingBar.Clickable = true;
            ratingBar.RatingBarChange += (o, e) =>
            {
                rating = (Int32)ratingBar.Rating;
            };
                sendReviewButton.Click += SendReviewButtonClicked;
            //reviewButton.Click += ReviewButtonClicked;
            return view;
        }

        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
        }

        // Load a new activity and transfer the data to the new one
        void SendReviewButtonClicked(object sender, EventArgs e)
        {            
            //rideID : 2
            UserMapper.getInstance().LeaveReview(1, userID, rating, riderReview.Text);
            Dismiss();
        }
    }
}