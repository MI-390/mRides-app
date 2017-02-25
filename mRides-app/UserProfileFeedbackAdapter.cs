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

    public class FeedbackForNow
    {
        public String review { get; set; }
        public int rating { get; set; }
        public int userIdForNow { get; set; }
        public String username { get; set; }
        public DateTime dateOfFeedback { get; set; }
    }


    class UserProfileFeedbackAdapter : BaseAdapter
    { 
        List<FeedbackForNow> fbfn;
        Activity context;

        public UserProfileFeedbackAdapter(Activity context, List<FeedbackForNow> fb) : base() { //change the FeedbackForNow parameter to Feedback later when it is implemented
            this.context = context;
            this.fbfn = fb;
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
            get { return fbfn.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            // could wrap a Contact in a Java.Lang.Object
            // to return it here if needed
            return null;
        }

        public override long GetItemId(int position)
        {
            return fbfn[position].userIdForNow;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {

            for (int i = 0; i < fbfn.Count; i++)
            {
                var view = convertView ?? context.LayoutInflater.Inflate(
                     Resource.Layout.UserProfileFeedbackFragment, parent, false);
                    
                var dateOfFeedback = view.FindViewById<TextView>(Resource.Id.feedbackDateProfileFragment);
                var userNameGivingFeedback = view.FindViewById<TextView>(Resource.Id.feedbackFragmentUserName);
                var ratingBarOfFeedback = view.FindViewById<RatingBar>(Resource.Id.feedbackFragmentRatingBar);
                var reviewOfFeedback = view.FindViewById<TextView>(Resource.Id.feedbackFragmentReview);

                userNameGivingFeedback.Text = fbfn.ElementAt(position).username;
                dateOfFeedback.Text = fbfn.ElementAt(position).dateOfFeedback.ToString();
                ratingBarOfFeedback.NumStars = fbfn.ElementAt(position).rating;
                reviewOfFeedback.Text = fbfn.ElementAt(position).review;

                return view;
            }
            return null;
        }

    }

}