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

            ImageView facebookPicture = FindViewById<ImageView>(Resource.Id.userPhoto);
            //var facebookPictureBitMap = GetImageBitmapFromUrl(User.currentUser.facebookPictureUrl);
            // TODO: use the real link once the server is updated to contain the user profile picture
            var facebookPictureBitMap = GetImageBitmapFromUrl("https://scontent.xx.fbcdn.net/v/t1.0-1/p50x50/13532894_10209745550106339_7260287654358119057_n.jpg?oh=70145db69abb22ca27097cb4f3d96e16&oe=5931D940");
            facebookPicture.SetImageBitmap(facebookPictureBitMap);

            //For testing
            //FeedbackForNow fb = new FeedbackForNow();
            //fb.dateOfFeedback = DateTime.Now;
            //fb.rating = 3;
            //fb.review = "Awesome";
            //fb.userIdForNow = 2;
            //fb.username = "Nassim";

            //FeedbackForNow fb2 = new FeedbackForNow();
            //fb2.dateOfFeedback = DateTime.Now;
            //fb2.rating = 5;
            //fb2.review = "Cool";
            //fb2.userIdForNow = 3;
            //fb2.username = "Hannah";

            //List<FeedbackForNow> feedbackfornowlist = new List<FeedbackForNow>();
            //feedbackfornowlist.Add(fb);
            //feedbackfornowlist.Add(fb2);

            //put for loop later in the future
            var userProfileFeedbackAdapter = new UserProfileFeedbackAdapter(this, userFeedback);
            //var feedbackAdapter = new UserProfileFeedbackAdapter(this);
            var feedbackListView = FindViewById<ListView>(Resource.Id.userProfileListView);
            feedbackListView.Adapter = userProfileFeedbackAdapter;

        }

        /// <summary>
        /// This method takes in the URL that goes to an image, and converts
        /// the image into a bitmap to be returned.
        /// </summary>
        /// <param name="url">URL representing the image</param>
        /// <returns>Bitmap of the image</returns>
        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
    }
}