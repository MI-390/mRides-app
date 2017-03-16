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
        Button submitButton;
        EditText reviewEditText;

        int userID;
        int rating;
        string review;

        // Method that will be executed when the fragment is created
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Bundle args = Arguments;
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.LeaveReview, container, false);

            // Get the user id
            userID = Int32.Parse(args.GetString("id"));

            // Get views by Id
            submitButton = view.FindViewById<Button>(Resource.Id.submitFeedback1);
            ratingBar = view.FindViewById<RatingBar>(Resource.Id.reviewRatingBar1);
            reviewEditText = view.FindViewById<EditText>(Resource.Id.reviewEdit1);

            // For clicking on the submit button
            submitButton.Click += submitFeedback;

            return view;
        }

        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
        }

        // Method that is called when the user clicks on 'Submit' button
        void submitFeedback(object sender, EventArgs e)
        {
            // Get the review and rating
            review = getCurrentReviewText();
            rating = getCurrentRating();

            UserMapper.getInstance().LeaveReview(1, userID, rating, review);
            Dismiss();

            // To debug
            Toast.MakeText(Activity, "Rating: " + rating + "\nReview: " + review, ToastLength.Short).Show();
        }

        // Method to get the current text entered by the user in the EditText
        // These was separated for testing purpose
        String getCurrentReviewText()
        {
            return reviewEditText.Text;
        }

        // Method to get the current star rating entered by the user
        int getCurrentRating()
        {
            return (Int32)ratingBar.Rating;
        }

        // Method to set the review to an arbitrary text (for testing purpose)
        void setCurrentReviewText(String str)
        {
            review = str;
        }

        // Method to set the review to an arbitrary rating (for testing purpose)
        void setCurrentRating(int ratingNum)
        {
            this.rating = ratingNum;
        }
    }
}