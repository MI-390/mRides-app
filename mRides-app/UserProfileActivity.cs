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

            //For testing
            FeedbackForNow fb = new FeedbackForNow();
            fb.dateOfFeedback = DateTime.Now;
            fb.rating = 3;
            fb.review = "Awesome";
            fb.userIdForNow = 2;
            fb.username = "Nassim";

            FeedbackForNow fb2 = new FeedbackForNow();
            fb.dateOfFeedback = DateTime.Now;
            fb.rating = 5;
            fb.review = "Cool";
            fb.userIdForNow = 3;
            fb.username = "Hannah";

            List<FeedbackForNow> feedbackfornowlist = new List<FeedbackForNow>();
            feedbackfornowlist.Add(fb);
            feedbackfornowlist.Add(fb2);

            //put for loop later in the future
            var userProfileFeedbackAdapter = new UserProfileFeedbackAdapter(this, feedbackfornowlist);
            //var feedbackAdapter = new UserProfileFeedbackAdapter(this);
            var feedbackListView = FindViewById<ListView>(Resource.Id.userProfileListView);
            feedbackListView.Adapter = userProfileFeedbackAdapter;

        }
    }
}