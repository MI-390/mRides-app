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
            return SendGetWithUrlSegment<User>(ApiEndPointUrl.getUserByFacebookId, "id", userId.ToString());
        }

        /**
         * Create a new user given the new user object
         */
        public User CreateUser(User newUser)
        {
            return SendPost<User>(ApiEndPointUrl.createUser, newUser, false);
        }

        /**
         * Obtain reviews given to a user
         */
        public List<Models.Feedback> GetReviews(int userId)
        {
            return SendGetWithUrlSegment<List<Models.Feedback>>(ApiEndPointUrl.getReviews, "id", userId.ToString());
        }
    }
}