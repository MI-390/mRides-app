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
    public class RiderReview : DialogFragment
    {
        String review = "";
        EditText reviewText;
        RatingBar ratingBarRider;
        Button submit;
        int rating;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.RiderReview, container, false);
            ratingBarRider = view.FindViewById<RatingBar>(Resource.Id.ratingBarRider);
            reviewText = view.FindViewById<EditText>(Resource.Id.riderReviewEdit);
            submit = view.FindViewById<Button>(Resource.Id.submitFeedback2);

            submit.Click += submitFeedback;
            return view;
        }

        // Method that is called when the user clicks on 'Submit' button
        void submitFeedback(object sender, EventArgs e)
        {
            review = getCurrentReviewText();
            rating = getCurrentRating();
            Dismiss();
            Toast.MakeText(Activity, "Rating: " + rating + "\nReview: " + review, ToastLength.Short).Show();
        }

        // Method to get the current text entered by the user in the EditText
        String getCurrentReviewText()
        {
            return reviewText.Text;
        }

        // Method to get the current star rating entered by the user
        int getCurrentRating()
        {
            return Int32.Parse(ratingBarRider.Rating.ToString());
        }
    }
}