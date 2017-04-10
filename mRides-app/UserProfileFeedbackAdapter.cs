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

    /// <summary>
    /// Implementation of the adapter class for the 
    /// ratings and reviews shown on user profile.
    /// </summary>
    public class UserProfileFeedbackAdapter : BaseAdapter
    {
        List<Models.Feedback> feedbackList;
        Activity context;
        int counter = 0;

        /// <summary>
        /// Constructor of UserProfileFeedbackAdapter
        /// </summary>
        /// <param name="context">An instance of chatListActivity</param>
        /// <param name="fbList">A list of Feedbacks to be displayed</param>
        public UserProfileFeedbackAdapter(Activity context, List<Models.Feedback> fbList) : base() { 
            if (counter == 0)
            {
                this.context = context;
                this.feedbackList = fbList;
                counter++;
            }
            else
                this.context = context;
        }

        /// <summary>
        /// Constructor of UserProfileFeedbackAdapter
        /// </summary>
        /// <param name="activity">An instance of chatListActivity</param>
        public UserProfileFeedbackAdapter(Activity activity)
        {
            context = activity;
        }

        /// <summary>
        /// Method to get the number of feedbacks in the list
        /// </summary>
        public override int Count
        {
            get { return feedbackList.Count; }
        }

        /// <summary>
        /// Method to get an item given its position in the list.
        /// </summary>
        /// <param name="position">Position of item in list</param>
        /// <returns>Position of item</returns>
        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        /// <summary>
        /// Method to get ID of the item.
        /// </summary>
        /// <returns>ID of item</returns>
        public override long GetItemId(int position)
        {
            return feedbackList[position].givenBy.id;
        }

        /// <summary>
        /// Method that is returns the view of the adapter
        /// </summary>
        /// <param name="position">Position of the view</param>
        /// <param name="convertView">A View object</param>
        /// <param name="parent">The parent View object</param>
        /// <returns>View object</returns>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {

            var view = convertView ?? context.LayoutInflater.Inflate(Resource.Layout.UserProfileFeedbackFragment, parent, false);

            var dateOfFeedback = view.FindViewById<TextView>(Resource.Id.feedbackDateProfileFragment);
            var userNameGivingFeedback = view.FindViewById<TextView>(Resource.Id.feedbackFragmentUserName);
            var ratingBarOfFeedback = view.FindViewById<RatingBar>(Resource.Id.feedbackFragmentRatingBar);
            var reviewOfFeedback = view.FindViewById<TextView>(Resource.Id.feedbackFragmentReview);

            userNameGivingFeedback.Text = feedbackList.ElementAt(position).givenBy.firstName;

            string[] dateTextArray = feedbackList.ElementAt(position).time.Split('T');
            string[] dayArray = dateTextArray[0].Split('-');
            string[] timeArray = dateTextArray[1].Split(':');

            if (dayArray[1].Equals("01"))
                dayArray[1] = "January";
            else if (dayArray[1].Equals("02"))
                dayArray[1] = "February";
            else if (dayArray[1].Equals("03"))
                dayArray[1] = "March";
            else if (dayArray[1].Equals("04"))
                dayArray[1] = "April";
            else if (dayArray[1].Equals("05"))
                dayArray[1] = "May";
            else if (dayArray[1].Equals("06"))
                dayArray[1] = "June";
            else if (dayArray[1].Equals("07"))
                dayArray[1] = "July";
            else if (dayArray[1].Equals("08"))
                dayArray[1] = "August";
            else if (dayArray[1].Equals("09"))
                dayArray[1] = "September";
            else if (dayArray[1].Equals("10"))
                dayArray[1] = "October";
            else if (dayArray[1].Equals("11"))
                dayArray[1] = "November";
            else if (dayArray[1].Equals("12"))
                dayArray[1] = "December";

            dateOfFeedback.Text = dayArray[1] + " " + dayArray[2] + ", " + dayArray[0] +
                " at " + timeArray[0] + ":" + timeArray[1]; 
            ratingBarOfFeedback.NumStars = feedbackList.ElementAt(position).stars;
            reviewOfFeedback.Text = feedbackList.ElementAt(position).feedbackText;

            // Get the picture of the reviewer from the mapper
            UserMapper userMapper = UserMapper.getInstance();
            var facebookPictureBitMap = userMapper.GetUserFacebookProfilePicture(feedbackList.ElementAt(position).givenBy.facebookID);
            if(facebookPictureBitMap != null)
            {
                var reviewerProfilePicture = view.FindViewById<ImageView>(Resource.Id.feedbackFragmentUserProfilePicture);
                reviewerProfilePicture.SetImageBitmap(facebookPictureBitMap);
            }

            return view;
        }
    }
}