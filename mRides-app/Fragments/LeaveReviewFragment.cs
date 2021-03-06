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
using mRides_app.Models;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace mRides_app
{
    /// <summary>
    /// Fragment for leaving a review
    /// </summary>
    class LeaveReviewFragment : DialogFragment
    {
        RatingBar ratingBar;
        Button submitButton;
        Button closeReview;
        EditText reviewEditText;

        int userID;
        int rating;
        string review;

        /// <summary>
        /// Method that is invoked upon creation of the fragment
        /// </summary>
        /// <param name="inflater">A LayoutInflater</param>
        /// <param name="container">A ViewGroup</param>
        /// <param name="savedInstanceState">A Bundle</param>
        /// <returns></returns>
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
            closeReview = view.FindViewById<Button>(Resource.Id.closeFeedback1);

            // Set submit button color to the right color
            if (User.currentUser != null)
            {
                if (User.currentUser.currentType == "rider")
                {
                    submitButton.SetBackgroundResource(Resource.Drawable.green_button);
                }
                else
                {
                    submitButton.SetBackgroundResource(Resource.Drawable.red_button);
                }
            }

            // For clicking on the submit button
            submitButton.Click += SubmitFeedback;
            closeReview.Click += CloseFeedback;
            return view;
        }

        /// <summary>
        /// Method invoked when activity is attached to the fragment
        /// </summary>
        /// <param name="activity"></param>
        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
        }

        /// <summary>
        /// Method that is invoked when the user clicks on 'Submit' button
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">An EventArgs argument</param>
        void SubmitFeedback(object sender, EventArgs e)
        {
            // Get the review and rating
            review = reviewEditText.Text;
            rating = (Int32)ratingBar.Rating;

            UserMapper.getInstance().LeaveReview(1, userID, rating, review);

            long gsdAmount = UserMapper.getInstance().GetGSD(userID);
            if (rating == 5)
                gsdAmount += 100;
            else if (rating == 4)
                gsdAmount += 75;
            else if (rating == 3)
                gsdAmount += 50;
            else if (rating == 2)
                gsdAmount += 25;
            else if (rating < 2)
                gsdAmount += 0; // Change this if necessary, for now GSD doesn't increase if rating is below 2/5

            UserMapper.getInstance().setGSD(userID, gsdAmount);

            Dismiss();

            // To debug
            Toast.MakeText(Activity, "Thank you for leaving a feedback!", ToastLength.Short).Show();
        }

        /// <summary>
        /// Method that will close the feedback fragment after clicking the 'Close' button
        /// </summary>
        /// <param name="sender">A sender object</param>
        /// <param name="e">An EventArgs argument</param>
        void CloseFeedback(object sender, EventArgs e)
        {
            Dismiss();
        }
    }
}