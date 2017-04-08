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
using System.Threading.Tasks;

namespace mRides_app.Gateways
{
    public class UserGateway : AbstractGateway
    {
        private UserGateway() { }

        private static UserGateway instance;

        /// <summary>
        /// Method that returns the user gateway singleton instance.
        /// </summary>
        public static UserGateway GetInstance()
        {
            if (instance == null)
            {
                instance = new UserGateway();
            }
            return instance;
        }

        // ---------------------------------------------------------------------------
        // CALLS TO USER WEB API
        // ---------------------------------------------------------------------------

        /// <summary>
        /// This method returns a user object given its user ID.
        /// </summary>
        public User GetUser(int userId)
        {
            return SendGetWithUrlSegment<User>(ApiEndPointUrl.getUser, "id", userId.ToString());
        }

        /// <summary>
        /// Obtain a user object given its Facebook ID.
        /// </summary>
        public User GetUserByFacebookId(long userId)
        {
            return SendPost<User>(ApiEndPointUrl.getUserByFacebookId, userId.ToString(), false);
        }

        /// <summary>
        /// This method creates a new user, given a new user object.
        /// </summary>
        public User CreateUser(User newUser)
        {
            return SendPost<User>(ApiEndPointUrl.createUser, newUser, false);
        }

        /// <summary>
        /// Updates a fcm token of a user.
        /// </summary>
        public void UpdateFcmToken(string fcmToken)
        {
            object fcmTokenObject = new
            {
                fcmToken = fcmToken
            };
            SendPost<object>(ApiEndPointUrl.updateFcmToken, fcmTokenObject, true);
        }

        public List<Request> GetRequests(int userId)
        {
            return SendGetWithUrlSegment<List<Request>>(ApiEndPointUrl.getUser, "id", userId.ToString());
        }

        /// <summary>
        /// Creates a new review.
        /// </summary>
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

        /// <summary>
        /// Obtains reviews given to a user.
        /// </summary>
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
            foreach (dynamic oFeedback in oFeedbacks)
            {
                Models.Feedback feedback = JsonConvert.DeserializeObject<Models.Feedback>(oFeedback.ToString());
                feedbacks.Add(feedback);
            }

            return feedbacks;
        }
        


        // ---------------------------------------------------------------------------
        // CALLS TO FACEBOOK
        // ---------------------------------------------------------------------------

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
            Bitmap facebookPicture;
            try
            {
                facebookPicture = GetImageBitmapFromUrl("http://graph.facebook.com/" + facebookId + "/picture?type=large");
            }
            catch (Exception e)
            {
                facebookPicture = null;
            }
            return facebookPicture;
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
            Bitmap facebookPicture;
            try
            {
                facebookPicture = await GetImageBitmapFromUrlAsync("http://graph.facebook.com/" + facebookId + "/picture?type=large");
            }
            catch (Exception e)
            {
                facebookPicture = null;
            }
            return facebookPicture;
        }


        // ---------------------------------------------------------------------------
        // HELPER METHODS
        // ---------------------------------------------------------------------------

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

        /// <summary>
        /// This method takes in the URL that goes to an image, and converts
        /// the image into a bitmap to be returned asynchronously.
        /// </summary>
        /// <param name="url">URL representing the image</param>
        /// <returns>Bitmap of the image</returns>
        private async Task<Bitmap> GetImageBitmapFromUrlAsync(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = await webClient.DownloadDataTaskAsync(new Uri(url));
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = await BitmapFactory.DecodeByteArrayAsync(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }

        /// <summary>
        /// Obtains the GSD amount of a user.
        /// </summary>
        public long GetGSD(int userId)
        {
            return SendGetWithUrlSegment<long>(ApiEndPointUrl.getGSD, "id", userId.ToString());
        }

        /// <summary>
        /// This method sets the GSD amount of a user.
        /// </summary>
        public long setGSD(int id, long gsdAmount)
        {
            object objectSent = new
            {
                userId = id,
                amountGSD = gsdAmount
            };
            return SendPost<long>(ApiEndPointUrl.setGSD, objectSent, false);
        }

        /// <summary>
        /// This method returns the gender of a user.
        /// </summary>
        /// <param name="userId">Id of the user whose gender we are obtaining</param>
        /// <returns>Gender of the user</returns>
        public string getGender(int userId)
        {
            // Create a new rest client
            var client = new RestClient()
            {
                BaseUrl = new System.Uri(BaseUrl),
                Authenticator = new HttpBasicAuthenticator(_accountSid, _secretKey)
            };

            // Serialize the object of interest into a JSON
            var json = JsonConvert.SerializeObject(userId);

            string url = ApiEndPointUrl.getGender;
            url = url.Replace("{id}", userId.ToString());

            // Make a new request object
            var request = new RestRequest(url, Method.GET);

            //Response variable
            var response = client.Execute(request);

            string genderObtained = response.Content;
            return genderObtained;
        }

        /// <summary>
        /// This method sets the gender of a user.
        /// </summary>
        public void setGender(int id, string newGender)
        {
            object objectSent = new
            {
                userId = id,
                userGender = newGender
            };
            SendPost<object>(ApiEndPointUrl.setGender, objectSent, false);
        }
    }
}