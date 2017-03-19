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
using mRides_app.Models;
using mRides_app.Mappers;
using Android.Graphics;
using System.Net;
using Xamarin.Auth;

namespace mRides_app
{
    [Activity(Label = "UserProfileActivity")]
    public class UserProfileActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            string idToGet = Intent.GetStringExtra("id") ?? GetString(Resource.String.ExtraData_DataNotAvailable);
            int id = Int32.Parse(idToGet);
            User user = UserMapper.getInstance().GetUser(id);

            //Displaying username based on name sent after authentication
            //In the future, can be changed to get username from DB
            TextView usernameText;
            //String username = Intent.GetStringExtra("Profile Info") ?? "Data not available";
            SetContentView(Resource.Layout.UserProfile);
            usernameText = FindViewById<TextView>(Resource.Id.userName);
            usernameText.Text = user.firstName.ToString() + " " + user.lastName.ToString();
            List<Models.Feedback> userFeedback = UserMapper.getInstance().GetReviews(id);
            
            // Obtain the user's profile picture from facebook
            UserMapper userMapper = UserMapper.getInstance();
            var facebookPictureBitMap = userMapper.GetUserFacebookProfilePicture(user.facebookID);
            if(facebookPictureBitMap != null)
            {
                ImageView facebookPicture = FindViewById<ImageView>(Resource.Id.userPhoto);
                facebookPicture.SetImageBitmap(facebookPictureBitMap);
            }
            

            //For testing
            //FeedbackForNow fb = new FeedbackForNow();
            //fb.dateOfFeedback = DateTime.Now;
            //fb.rating = 3;
            //fb.review = "Awesome";
            //fb.userIdForNow = 2;
            //fb.username = "Nassim";

            TextView gsdText;
            gsdText = FindViewById<TextView>(Resource.Id.userProfileGSD);
            long userGSD = UserMapper.getInstance().GetGSD(id);
            gsdText.Text = "$ " + userGSD.ToString() + " GSD";


            List<Models.Feedback> userFeedback = UserMapper.getInstance().GetReviews(id);
            var userProfileFeedbackAdapter = new UserProfileFeedbackAdapter(this, userFeedback);
            var feedbackListView = FindViewById<ListView>(Resource.Id.userProfileListView);
            feedbackListView.Adapter = userProfileFeedbackAdapter;

        }
        
    }
}