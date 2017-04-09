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
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using mRides_app.Models;

namespace mRides_app.Gateways
{


    /// <summary>
    /// This class allows to send API calls to the server.
    /// It is a singleton since only one instance of it is required across the application.
    /// 
    /// ---------------------------------------------------------------------------------------------
    /// Details:
    /// 
    /// To communicate with the server, data is sent through RestSharp requests objects.
    /// In brief, to call a method found on the server side, we access a URL, and attach data
    /// to the URL. For instance, if we wish to obtain the User object for userid = 1:
    /// http://mrides-server.azurewebsites.net/api/User/getUser/1 will be called, and the response
    /// of that call is expected to be the User object with ID = 1.
    /// 
    /// ---------------------------------------------------------------------------------------------
    /// Structure:
    /// 
    /// 1. The private class ApiEndPointUrl contains a list of constant strings representing the 
    ///    URL to access each method on the server side.
    /// 
    /// 2. Multiple public static methods define the exposed interface of the server. They will be
    ///    used by the app activities to make api calls. For example: if an activity needs to find the
    ///    user object with userid = 1, it will call "MRidesWebApi.getUser(1)". This method will be
    ///    responsible to commnicate with the server and obtain the object.
    /// 
    /// 3. Multiple private static methods help the other methods to send the GET/POST/DELETE requests
    ///    to the server.
    /// 
    /// 4. The response may not exactly to correspond to a model object, it may contain a composition of 
    ///    those objects. For this purpose, the namespace MRidesJSON is used to contain classes that will
    ///    correspond to the deserialized responses from the server side.
    /// 
    /// ---------------------------------------------------------------------------------------------
    /// Steps to add a method to the exposed interface:
    /// 
    /// 1. Go to http://mrides-server.azurewebsites.net/swagger/index.html
    /// 2. Find the method of interest to be added
    /// 3. Notice the HTTP method used (GET, POST, DELETE) and the URL used.
    /// 4. Define a public static method with the type signature corresponding to the expected
    ///    data type on the server. For this, you can use objects found under /Models/ to send
    ///    data.
    /// 5. The return value of this method can be known by looking at the server code, or 
    ///    asking the server development team about it.
    /// 6. Call the appropriate private static helper methods to send the request to the server,
    ///    specifying the type that the response should be converted to.
    /// 7. Manipulate the response if necessary, and return it.
    /// 
    /// In order to test those, add test cases to the UnitTests project.
    /// 
    /// ---------------------------------------------------------------------------------------------
    /// Example:
    /// 
    /// The server defines the following:
    /// POST | api/User/createUser
    /// and expects as parameter a User object as shown under Data Type / Example value on 
    /// http://mrides-server.azurewebsites.net/swagger/index.html
    /// 
    /// A method must then be defined for the app activities. Let's call it "CreateUser" 
    /// and take a User object as a paramater (from the Models namespace). From the server
    /// we see that such call will return the newly created user object (with its primary key ID
    /// assigned). Thus, the definition of the method will be:
    /// 
    /// public static User CreateUser(User newUser)
    /// 
    /// Since it is a POST, use the SendPost with type User, to make the call, giving it
    /// the correct URL, data (User), and in this case "false" since
    /// the user id is unknown and must not be included for this post
    /// request. Generally, this parameter is set to true.
    /// 
    /// </summary>
    public abstract class AbstractGateway
    {
        // URL to the web server
        protected const string BaseUrl = "http://mrides-server.azurewebsites.net/api/";
        protected const string HeaderNameUserId = "id";

        // Credentials
        protected static readonly string _accountSid = null;
        protected static readonly string _secretKey = null;


        // ---------------------------------------------------------------------------
        // METHODS TO SEND API
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Given an object of interest and an api end point, this method converts the
        /// object into a json, and sends it through a rest request to the api end point, 
        /// then returns the reponse of this request
        /// </summary>
        /// <typeparam name="T">Type to which the reponse content should be converted to</typeparam>
        /// <param name="apiEndPoint">API end point link</param>
        /// <param name="objectToUpdate">Object of interest to be updated</param>
        /// <param name="includeUserInHeader">true if the user id should be included in the header</param>
        /// <returns>Response</returns>
        public T SendPost<T>(string apiEndPoint, object objectToUpdate, bool includeUserInHeader) where T : new()
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
            if (includeUserInHeader)
            {
                request.AddHeader(HeaderNameUserId, User.currentUser.id.ToString());
            }
            request.AddParameter("text/json", json, ParameterType.RequestBody);

            // Execute the request and return the response
            var response = client.Execute<T>(request);

            return response.Data;
        }

        /// <summary>
        /// Given an api end point, a segment name and its value,
        /// this method will send a GET request to the given url,
        /// replacing the segment with its value and return the response
        /// of the request, casting it to the specified type.
        /// </summary>
        /// <typeparam name="T">Type to which the reponse content should be converted to</typeparam>
        /// <param name="apiEndPoint">API end point link</param>
        /// <param name="segmentName">Name of the segment to be replaced</param>
        /// <param name="segmentValue">Value of the segment that will replace the segment name</param>
        /// <returns>Response</returns>
        public T SendGetWithUrlSegment<T>(string apiEndPoint, string segmentName, string segmentValue) where T : new()
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
            var response = client.Execute<T>(request);
            return response.Data;
        }

        /// <summary>
        /// Given an api end point, a segment name and its value,
        /// this method will send a DELETE request to the given url,
        /// replacing the segment with its value and return the response
        /// of the request, casting it to the specified type.
        /// </summary>
        /// <typeparam name="T">Type to which the reponse content should be converted to</typeparam>
        /// <param name="apiEndPoint">API end point link</param>
        /// <param name="segmentName">Name of the segment to be replaced</param>
        /// <param name="segmentValue">Value of the segment that will replace the segment name</param>
        /// <returns>Response</returns>
        public T SendDeleteWithUrlSegment<T>(string apiEndPoint, string segmentName, string segmentValue) where T : new()
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


    // ---------------------------------------------------------------------------
    // AVAILABLE URLS
    // ---------------------------------------------------------------------------

    /// <summary>
    /// This class contains a set of API end point URL used as parameter in various method
    /// in the AbstractGateway class.
    /// </summary>
    public class ApiEndPointUrl
    {
        // Console related url
        public const string findDrivers = "Console/findDrivers";
        public const string findRiders = "Console/findRiders";
        public const string confirm = "Console/confirm";
        public const string acceptConfirm = "Console/acceptConfirm";
        public const string getRequestById = "Request/getRequest/{id}";

        // Request related url
        public const string createRequest = "Request/createRequest";
        public const string deleteRequest = "Request/deleteRequest/{id}";

        // User related url
        public const string createUser = "User/createUser";
        public const string getUser = "User/getUser/{id}";
        public const string getUserByFacebookId = "User/getUserByFacebookId";
        public const string getReviews = "User/getReviews/{id}";
        public const string leaveReview = "User/leaveReview";
        public const string updateFcmToken = "User/updateFcmToken";
        public const string getGSD = "User/getGSD/{id}";
        public const string setGSD = "User/setGSD";
        public const string getGender = "User/getGender/{id}";
        public const string setGender = "User/setGender";
        public const string getRequests = "User/getRequests/{id}";

        // Ride related url
        public const string createRide = "Ride/createRide";
        public const string addRiderToRide = "Ride/addRiderToRide";
        public const string getRide = "Ride/getRide/{id}";
    }
}