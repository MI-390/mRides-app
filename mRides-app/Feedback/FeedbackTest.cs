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
using Android.Util;

namespace mRides_app.Feedback
{
    [Activity(Label = "FeedbackTest")]
    public class FeedbackTest : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Alert);
            var feedbackButton = FindViewById<Button>(Resource.Id.openFeedbackAlert);

            // To be used when adding reviews at the end of a ride
            feedbackButton.Click += (sender, args) =>
            {
                Android.App.AlertDialog.Builder builder = new AlertDialog.Builder(this);
                AlertDialog ad = builder.Create();
                ad.SetTitle("Feedback");
                ad.SetIcon(Resource.Drawable.Icon);
                ad.SetMessage("Would you like to leave a feedback for this ride?");
                // Positive
                ad.SetButton("Yes", (s, e) => {
                    Console.WriteLine("Yes button clicked, alert dismissed");
                    openLeaveReviewFragment();
                });
                // Negative
                ad.SetButton2("No", (s, e) => { Console.WriteLine("No button clicked, alert dismissed"); });
                // Neutral
                ad.SetButton3("Later", (s, e) => { Console.WriteLine("Later button clicked, alert dismissed"); });
                ad.Show();
            };
        }

        // Method to open the driver fragment to make a review for a Driver
        void openLeaveReviewFragment()
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            LeaveReviewFragment dialog = new LeaveReviewFragment();
            dialog.Show(transaction, "Review Fragment");
        }
    }
}