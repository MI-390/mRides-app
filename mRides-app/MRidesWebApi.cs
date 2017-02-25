using mRides_app.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;

namespace mRides_app
{
    /**
     * This class allows to send API calls to the server.
     * It is a singleton since only one instance of it is required across the application.
     */ 
    public class MRidesWebApi
    {
        // URL to the web server
        const string BaseUrl = "http://mrides-server.azurewebsites.net/api/";
        const string HeaderNameUserId = "id";

        // Credentials
        private static readonly string _accountSid = null;
        private static readonly string _secretKey = null;

        // Class instance <Singleton>
        private static MRidesWebApi _instance;

        /**
         * Private constructor
         */ 
        private MRidesWebApi()
        {
        }

        /**
         * Instance getter
         */
        public static MRidesWebApi getInstance ()
        {
            if(_instance == null)
            {
                _instance = new MRidesWebApi();
            }
            return _instance;
        }


        // ---------------------------------------------------------------------------
        // AVAILABLE URLS
        // ---------------------------------------------------------------------------

        private class ApiEndPointUrl
        {
            // Console related url
            public const string findDrivers = "Console/findDrivers";
            public const string findRiders = "Console/findRiders";
            public const string confirm = "Console/confirm";
            public const string acceptConfirm = "Console/acceptConfirm";
            public const string createRide = "Console/createRide";
            public const string addRiderToRequest = "Console/addRiderToRequest";

            // Request related url
            public const string createRequest = "Request/createRequest";
            public const string deleteRequest = "Request/deleteRequest/{id}";

            // User related url
            public const string createUser = "User/createUser";
            public const string getUser = "User/getUser/{id}";
            public const string getUserByFacebookId = "User/getUserByFacebookId/{id}";
            public const string getReviews = "User/getReviews/{id}";
            
        }


        // ---------------------------------------------------------------------------
        // CALLS TO CONSOLE WEB API
        // ---------------------------------------------------------------------------
        
       /**
        * This method is used to find a list of rides that match the criteria
        * of a request made by a rider.
        */ 
        public static List<Request> FindDrivers(Request newRequest)
        {
            dynamic response = SendPost<dynamic>(MRidesWebApi.ApiEndPointUrl.findDrivers, newRequest, true);

            // The response consists of a list:
            // - The first element is a list of Ride
            // - The second element is the id of the newly create request
            List<Request> requests = response.rides.ToObject<List<Request>>();
            int requestId = Convert.ToInt32(response.requestId);
            newRequest.ID = requestId;

            return requests;
        }

        /**
          * This method is used to find a list of rides that match the criteria
          * of a request made by a driver.
          */
        public static List<Request> FindRiders(Request newRequest)
        {
            dynamic response = SendPost<dynamic>(MRidesWebApi.ApiEndPointUrl.findRiders, newRequest, true);

            // The response consists of a list:
            // - The first element is a list of Ride
            // - The second element is the id of the newly create request
            List<Request> requests = response.rides.ToObject<List<Request>>();
            int requestId = Convert.ToInt32(response.requestId);
            newRequest.ID = requestId;

            return requests;
        }

        /**
         * Sends the FIRST confirmation, in other words, ask the other user
         * if he/she accepts to be matched. This does not imply that there is 
         * match, unless the other users accepts the confirmation.
         * 
         * Returns a boolean indicating whether the message was sent successfully.
         */ 
        public static bool Confirm(int riderRequestId, int driverRequestId)
        {
            object confirmation = new
            {
                riderRequestId = riderRequestId,
                driverRequestId = driverRequestId
            };
            return SendPost<bool>(MRidesWebApi.ApiEndPointUrl.confirm, confirmation, true);
        }

        /**
         * The other user already accepts the match, upon notifying this user,
         * if the user also accepts to be match with the other, this method should
         * be invoked.
         * 
         * Returns a boolean indicating whether the messagw was sent successfully.
         */ 
        public static bool AcceptConfirmation(int riderRequestId, int driverRequestId)
        {
            object confirmationAcceptance = new
            {
                riderRequestId = riderRequestId,
                driverRequestId = driverRequestId
            };
            return SendPost<bool>(MRidesWebApi.ApiEndPointUrl.acceptConfirm, confirmationAcceptance, true);
        }





        // ---------------------------------------------------------------------------
        // CALLS TO REQUEST WEB API
        // ---------------------------------------------------------------------------
        
        /**
         * Create a request
         */ 
        public static void CreateRequest(Request newRequest)
        {
            newRequest.DriverID = null;
            SendPost<Request>(MRidesWebApi.ApiEndPointUrl.createRequest, newRequest, true);
        }
        



        // ---------------------------------------------------------------------------
        // CALLS TO USER WEB API
        // ---------------------------------------------------------------------------

        /**
         * Obtain a user object given its ID
         */
        public static User GetUser(int userId)
        {
            return SendGetWithUrlSegment<User>(MRidesWebApi.ApiEndPointUrl.getUser, "id", userId.ToString());
        }

        /**
         * Obtain a user object given its Facebook ID
         */
        public static User GetUserByFacebookId(long userId)
        {
            return SendGetWithUrlSegment<User>(MRidesWebApi.ApiEndPointUrl.getUserByFacebookId, "id", userId.ToString());
        }  

        /**
         * Create a new user given the new user object
         */ 
        public static User CreateUser(User newUser)
        {
            return SendPost<User>(MRidesWebApi.ApiEndPointUrl.createUser, newUser, false);
        }

        /**
         * Obtain reviews given to a user
         */
        public static List<Models.Feedback> GetReviews(int userId)
        {
            return SendGetWithUrlSegment<List<Models.Feedback>>(ApiEndPointUrl.getReviews, "id", userId.ToString());
        }






        // ---------------------------------------------------------------------------
        // HELPER METHODS
        // ---------------------------------------------------------------------------

        /**
         * Given an object of interest and an api end point, this method
         * converts the object into a json, and sends it through a rest
         * request to the api end point, then returns the response of 
         * this request.
         */
        private static T SendPost<T>(string apiEndPoint, object objectToUpdate, bool includeUserInHeader) where T : new()
        {
            // Create a new rest client
            var client = new RestClient()
            {
                BaseUrl = new System.Uri(BaseUrl),
                Authenticator = new HttpBasicAuthenticator(_accountSid, _secretKey)
            };

            // Serialize the object of interest into a JSON
            var json = JsonConvert.SerializeObject(objectToUpdate);

            // Make a new request object
            var request = new RestRequest(apiEndPoint, Method.POST);
            if(includeUserInHeader)
            {
                request.AddHeader(HeaderNameUserId, User.currentUser.id.ToString());
            }
            request.AddParameter("text/json", json, ParameterType.RequestBody);

            // Execute the request and return the response
            var response = client.Execute<T>(request).Data;
            return response;
        }

        /**
         * Given an api end point, a segment name and its value,
         * this method will send a GET request to the given url,
         * replacing the segment with its value and return the response
         * of the request, casting it to the specified type.
         */
        private static T SendGetWithUrlSegment<T>(string apiEndPoint, string segmentName, string segmentValue) where T : new()
        {
            // Create a new rest client
            var client = new RestClient()
            {
                BaseUrl = new System.Uri(BaseUrl),
                Authenticator = new HttpBasicAuthenticator(_accountSid, _secretKey)
            };

            // Make a new request object
            apiEndPoint = apiEndPoint.Replace("{" + segmentName + "}", segmentValue);
            var request = new RestRequest(apiEndPoint, Method.GET);
            //request.AddUrlSegment(segmentName, segmentValue);

            // Execute the request and return the response
            var response = client.Execute<T>(request).Data;
            return response;
        }

        /**
         * Given an api end point, a segment name and its value,
         * this method will send a DELETE request to the given url,
         * replacing the segment with its value and return the response
         * of the request, casting it to the specified type.
         */
        private static T SendDeleteWithUrlSegment<T>(string apiEndPoint, string segmentName, string segmentValue) where T : new()
        {
            // Create a new rest client
            var client = new RestClient()
            {
                BaseUrl = new System.Uri(BaseUrl),
                Authenticator = new HttpBasicAuthenticator(_accountSid, _secretKey)
            };

            // Make a new request object
            apiEndPoint = apiEndPoint.Replace("{" + segmentName + "}", segmentValue);
            var request = new RestRequest(apiEndPoint, Method.DELETE);
            //request.AddUrlSegment(segmentName, segmentValue);

            // Execute the request and return the response
            var response = client.Execute<T>(request).Data;
            return response;
        }
    }
}