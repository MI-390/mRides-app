using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;

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
            public const string createRide = "Console/createRide";
            public const string addRiderToRequest = "Console/addRiderToRequest";

            // User related url
            public const string createUser = "User/createUser";
            public const string getUser = "User/getUser/{id}";

            // Request related url
            public const string createRequest = "Request/createRequest";
            public const string deleteRequest = "Request/deleteRequest/{id}";
        }


        // ---------------------------------------------------------------------------
        // CALLS TO CONSOLE WEB API
        // ---------------------------------------------------------------------------


        // ---------------------------------------------------------------------------
        // CALLS TO REQUEST WEB API
        // ---------------------------------------------------------------------------
        //public static Request CreateRequest(Request newRequest)
        //{
        //    return SendPost<Request>(MRidesWebApi.ApiEndPointUrl.createRequest, newRequest);
        //}

           


        // ---------------------------------------------------------------------------
        // CALLS TO USER WEB API
        // ---------------------------------------------------------------------------

       /**
        * Obtain a user object given its ID
        */ 
        public static User GetUser(long userId)
        {
            return SendGetWithUrlSegment<User>(MRidesWebApi.ApiEndPointUrl.getUser, "id", userId.ToString());
        }

        /**
         * Create a new user given the new user object
         */ 
        public static User CreateUser(User newUser)
        {
            return SendPost<User>(MRidesWebApi.ApiEndPointUrl.createUser, newUser);
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
        private static T SendPost<T>(string apiEndPoint, object objectToUpdate) where T : new()
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
            var request = new RestRequest(apiEndPoint, Method.GET);
            request.AddUrlSegment(segmentName, segmentValue);

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
            var request = new RestRequest(apiEndPoint, Method.DELETE);
            request.AddUrlSegment(segmentName, segmentValue);

            // Execute the request and return the response
            var response = client.Execute<T>(request).Data;
            return response;
        }
    }
}