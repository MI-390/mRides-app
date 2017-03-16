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
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;

namespace mRides_app.Mappers
{
    public class UserMapper : AbstractMapper
    {
        private UserMapper() { }

        private static UserMapper _instance;
        public static UserMapper getInstance()
        {
            if (_instance == null)
            {
                _instance = new UserMapper();
            }
            return _instance;
        }

        // ---------------------------------------------------------------------------
        // CALLS TO USER WEB API
        // ---------------------------------------------------------------------------

        /**
         * Obtain a user object given its ID
         */
        public User GetUser(int userId)
        {
            return SendGetWithUrlSegment<User>(ApiEndPointUrl.getUser, "id", userId.ToString());
        }

        /**
         * Obtain a user object given its Facebook ID
         */
        public User GetUserByFacebookId(long userId)
        {
            return SendPost<User>(ApiEndPointUrl.getUserByFacebookId, userId.ToString(), false);
        }

        /**
         * Create a new user given the new user object
         */
        public User CreateUser(User newUser)
        {
            return SendPost<User>(ApiEndPointUrl.createUser, newUser, false);
        }

        /**
         * Create a new review 
         */
        public void LeaveReview(int rideId, int revieweeId, int rating, string review)
        {
            object newReview = new
            {
                rideId = rideId,
                revieweeId = revieweeId,
                star = rating,
                review = review
            };
            SendPost<object>(ApiEndPointUrl.leaveReview, newReview, true);
        }

        /**
         * Obtain reviews given to a user
         */
        public List<Models.Feedback> GetReviews(int userId)
        {
            // Create a new rest client
            var client = new RestClient()
            {
                BaseUrl = new System.Uri(BaseUrl),
                Authenticator = new HttpBasicAuthenticator(_accountSid, _secretKey)
            };

            // Serialize the object of interest into a JSON
            var json = JsonConvert.SerializeObject(userId);

            // Make a new request object
            string url = ApiEndPointUrl.getReviews;
            url = url.Replace("{id}", userId.ToString());
            var request = new RestRequest(url, Method.GET);

            // Execute the request and return the response
            var response = client.Execute(request);
            dynamic oFeedbacks = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);

            // Build the list of feedbacks to be returns
            List<Models.Feedback> feedbacks = new List<Models.Feedback>();
            foreach(dynamic oFeedback in oFeedbacks)
            {
                Models.Feedback feedback = JsonConvert.DeserializeObject<Models.Feedback>(oFeedback.ToString());
                feedbacks.Add(feedback);
            }

            return feedbacks;
        }

        /**
        * Obtain a user's GSD amount
        */
        public long GetUserGSD(int userId)
        {
            return SendGetWithUrlSegment<long>(ApiEndPointUrl.getGSD, "id", userId.ToString());
        }

        /**
        * Change a user's GSD amount
        */
        public long setGSD(int userId, long amountGSD)
        {
            User u = SendGetWithUrlSegment<User>(ApiEndPointUrl.getUser, "id", userId.ToString());
            SendPost<long>(ApiEndPointUrl.setGSD, u, true);
            return u.gsd;
        }


    }
}