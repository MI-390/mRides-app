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
using Android.Graphics.Drawables;

namespace mRides_app
{
    [Activity(Label = "UserProfileActivity")]
    public class UserProfileActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            UserMapper.getInstance().setTheme(this);
            base.OnCreate(savedInstanceState);
            string idToGet = Intent.GetStringExtra("id") ?? GetString(Resource.String.ExtraData_DataNotAvailable);
            int id = Int32.Parse(idToGet);
            User user = UserMapper.getInstance().GetUser(id);

            //Displaying username based on name sent after authentication
            //In the future, can be changed to get username from DB
            TextView usernameText;
            SetContentView(Resource.Layout.UserProfile);
            usernameText = FindViewById<TextView>(Resource.Id.userName);
            usernameText.Text = user.firstName.ToString() + " " + user.lastName.ToString();
            List<Models.Feedback> userFeedback = UserMapper.getInstance().GetReviews(id);

            // Obtain the user's profile picture from facebook
            UserMapper userMapper = UserMapper.getInstance();
            var facebookPictureBitMap = userMapper.GetUserFacebookProfilePicture(user.facebookID);
            if (facebookPictureBitMap != null)
            {
                ImageView facebookPicture = FindViewById<ImageView>(Resource.Id.userPhoto);
                facebookPicture.SetImageBitmap(facebookPictureBitMap);
            }

            TextView gsdText;
            gsdText = FindViewById<TextView>(Resource.Id.userProfileGSD);
            long userGSD = UserMapper.getInstance().GetGSD(id);
            gsdText.Text = "$ " + userGSD.ToString() + " GSD";

            ImageView genderIcon = FindViewById<ImageView>(Resource.Id.genderImage);
            string facebookGender = userMapper.getGender(id);

            if (facebookGender.Equals("female")) {
                Drawable femaleIcon = Resources.GetDrawable(Resource.Drawable.girl);
                genderIcon.SetImageDrawable(femaleIcon);
            }
            else if (facebookGender.Equals("male"))
            {
                Drawable maleIcon = Resources.GetDrawable(Resource.Drawable.boy);
                genderIcon.SetImageDrawable(maleIcon);

            }

            LinearLayout layout = FindViewById<LinearLayout>(Resource.Id.profileBackground);

            // Set background color to the right color
            if (User.currentUser != null)
            {
                if (User.currentUser.currentType == "rider")
                {
                    layout.SetBackgroundColor(new Android.Graphics.Color(Color.ParseColor("#26A65B")));
                }
                else
                {
                    layout.SetBackgroundColor(new Android.Graphics.Color(Color.ParseColor("#EF5350")));
                }
            }

            var userProfileFeedbackAdapter = new UserProfileFeedbackAdapter(this, userFeedback);
            var feedbackListView = FindViewById<ListView>(Resource.Id.userProfileListView);
            feedbackListView.Adapter = userProfileFeedbackAdapter;

        }
        
    }
}