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
using Android.Graphics;
using System.Net;
using System.Threading;
using mRides_app.Gateways;
using mRides_app.Cache;
using System.Threading.Tasks;

namespace mRides_app.Mappers
{
    public class UserMapper : AbstractMapper
    {
        private UserGateway userGateway;
        private UserCache userCache;

        private UserMapper()
        {
            this.userGateway = UserGateway.GetInstance();
            this.userCache = UserCache.GetInstance();
        }

        private static UserMapper instance;

        /// <summary>
        /// Method that returns the user mapper singleton instance.
        /// </summary>
        public static UserMapper getInstance()
        {
            if (instance == null)
            {
                instance = new UserMapper();
            }
            return instance;
        }

        /// <summary>
        /// This method returns a user object given its user ID.
        /// </summary>
        public User GetUser(int userId)
        {
            return userGateway.GetUser(userId);
        }

        /// <summary>
        /// Obtain a user object given its Facebook ID.
        /// </summary>
        public User GetUserByFacebookId(long facebookId)
        {
            return userGateway.GetUserByFacebookId(facebookId);
        }

        /// <summary>
        /// This method creates a new user, given a new user object. It 
        /// also saves the user's preference in the local persistent storage.
        /// </summary>
        /// <param name="newUser">New user to be created</param>
        /// <returns>The updated newly created user with the ID</returns>
        public User CreateUser(User newUser)
        {
            newUser = userGateway.CreateUser(newUser);
            this.userCache.SaveUserPreferences(newUser.id, newUser.isSmoker, newUser.isHandicap, newUser.hasLuggage, newUser.hasPet, newUser.genderPreference);
            return newUser;
        }

        /// <summary>
        /// Updates a fcm token of a user.
        /// </summary>
        public void updateFcmToken(string fcmToken)
        {
            userGateway.UpdateFcmToken(fcmToken);
        }

        /// <summary>
        /// Creates a new review.
        /// </summary>
        public void LeaveReview(int rideId, int revieweeId, int rating, string review)
        {
            userGateway.LeaveReview(rideId, revieweeId, rating, review);
        }

        /// <summary>
        /// Obtains reviews given to a user.
        /// </summary>
        public List<Models.Feedback> GetReviews(int userId)
        {
            return userGateway.GetReviews(userId);
        }

        /// <summary>
        /// Given the facebook ID of a user, this method will retrieve the profile picture of 
        /// the user, and return a bitmap of that image, or null if any exceptions occur
        /// while attempting to retrieve or convert the image, most likely because the provided
        /// ID is wrong.
        /// </summary>
        /// <param name="facebookId">Facebook ID of the user for which we want the profile picture</param>
        /// <returns>Bitmap object representing the picture</returns>
        public Bitmap GetUserFacebookProfilePicture(long facebookId)
        {
            return userGateway.GetUserFacebookProfilePicture(facebookId);
        }

        /// <summary>
        /// Given the facebook ID of a user, this method will retrieve the profile picture of 
        /// the user asynchronously, and return a bitmap of that image, or null if any exceptions occur
        /// while attempting to retrieve or convert the image, most likely because the provided
        /// ID is wrong.
        /// </summary>
        /// <param name="facebookId">Facebook ID of the user for which we want the profile picture</param>
        /// <returns>Bitmap object representing the picture</returns>
        public async Task<Bitmap> GetUserFacebookProfilePictureAsync(long facebookId)
        {
            return await userGateway.GetUserFacebookProfilePictureAsync(facebookId);
        }

        /// <summary>
        /// Obtains the GSD amount of a user.
        /// </summary>
        public long GetGSD(int userId)
        {
            return userGateway.GetGSD(userId);
        }

        /// <summary>
        /// This method sets the GSD amount of a user.
        /// </summary>
        public long setGSD(int id, long gsdAmount)
        {
            return userGateway.setGSD(id, gsdAmount);
        }

        /// <summary>
        /// This method returns the gender of a user.
        /// </summary>
        /// <param name="userId">Id of the user whose gender we are obtaining</param>
        /// <returns>Gender of the user</returns>
        public string getGender(int userId)
        {
            return userGateway.getGender(userId);
        }

        /// <summary>
        /// This method sets the gender of a user.
        /// </summary>
        public void setGender(int id, string newGender)
        {
            userGateway.setGender(id, newGender);
        }

        /// <summary>
        /// Set the theme of an activity depending on whether the user is a driver or a rider
        /// </summary>
        /// <param name="context">Current activity</param>
        public void setTheme(Activity context)
        {
            if (User.currentUser.currentType == "driver")
            {
                context.SetTheme(Resource.Style.mRidesTheme);
            }
            else if (User.currentUser.currentType == "rider")
            {
                context.SetTheme(Resource.Style.mRidesThemeRider);
            }
        }

    }
}