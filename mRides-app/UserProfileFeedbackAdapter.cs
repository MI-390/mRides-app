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


    public class UserProfileFeedbackAdapter : BaseAdapter
    {
        List<Models.Feedback> feedbackList;
        Activity context;
        int counter = 0;

        public UserProfileFeedbackAdapter(Activity context, List<Models.Feedback> fbList) : base() { //change the FeedbackForNow parameter to Feedback later when it is implemented
            if (counter == 0)
            {
                this.context = context;
                this.feedbackList = fbList;
                counter++;
            }
            else
                this.context = context;
        }

        public UserProfileFeedbackAdapter(Activity activity)
        {
            context = activity;
            FillFeedbacks();
        }

        void FillFeedbacks()
        {

        }

        public override int Count
        {
            get { return feedbackList.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            // could wrap a Contact in a Java.Lang.Object
            // to return it here if needed
            return null;
        }

        public override long GetItemId(int position)
        {
            return feedbackList[position].givenBy.id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {

                var view = convertView ?? context.LayoutInflater.Inflate(Resource.Layout.UserProfileFeedbackFragment, parent, false);

            var dateOfFeedback = view.FindViewById<TextView>(Resource.Id.feedbackDateProfileFragment);
            var userNameGivingFeedback = view.FindViewById<TextView>(Resource.Id.feedbackFragmentUserName);
            var ratingBarOfFeedback = view.FindViewById<RatingBar>(Resource.Id.feedbackFragmentRatingBar);
            var reviewOfFeedback = view.FindViewById<TextView>(Resource.Id.feedbackFragmentReview);

            userNameGivingFeedback.Text = feedbackList.ElementAt(position).givenBy.firstName;
            dateOfFeedback.Text = feedbackList.ElementAt(position).time;
            ratingBarOfFeedback.NumStars = feedbackList.ElementAt(position).stars;
            reviewOfFeedback.Text = feedbackList.ElementAt(position).feedback;

            return view;
        }
    }
}