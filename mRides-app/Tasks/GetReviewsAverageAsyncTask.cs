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
using Java.Lang;
using mRides_app.Mappers;
using mRides_app.Tasks.Callbacks;

namespace mRides_app.Tasks
{
    public class GetReviewsAverageAsyncTask : AsyncTask<Java.Lang.Void, Java.Lang.Void, Java.Lang.Void>
    {

        // ID of the user for which we are retrieving reviews
        private int userId;

        // Callback object
        private IOnGetReviewsAverageCompleteCallback callBackObject;
        private double feedbacksAverage;

        // Mapper used to make the request
        private UserMapper userMapper;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userId">ID of the user for which reviews are to be retrieved</param>
        public GetReviewsAverageAsyncTask(int userId, IOnGetReviewsAverageCompleteCallback callBackObject)
        {
            this.userMapper = UserMapper.getInstance();
            this.userId = userId;
            this.callBackObject = callBackObject;
            this.feedbacksAverage = 0;
        }

        /// <summary>
        /// Obtains the feedbacks for the user and calculate the average.
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        protected override Java.Lang.Void RunInBackground(params Java.Lang.Void[] @params)
        {
            // Send the request to the server
            List<Models.Feedback> feedbacks = userMapper.GetReviews(this.userId);
            int feedbackSum = 0;
            int numFeedbacks = feedbacks.Count;

            if(numFeedbacks > 0)
            {
                foreach (Models.Feedback feedback in feedbacks)
                {
                    feedbackSum += feedback.stars;
                }
                this.feedbacksAverage = ((double) feedbackSum) / feedbacks.Count;
            }
            return null;
        }

        /// <summary>
        /// Calls back the callback method and giving it the computed average.
        /// </summary>
        /// <param name="result"></param>
        protected override void OnPostExecute(Java.Lang.Void result)
        {
            this.callBackObject.OnGetReviewsComplete(this.feedbacksAverage);
        }
    }
}