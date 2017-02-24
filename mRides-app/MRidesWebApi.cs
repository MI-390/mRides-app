using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;

namespace mRides_app
{
    public class MRidesWebApi
    {
        // URL to the web server
        const string BaseUrl = "http://mrides-server.azurewebsites.net/api/";

        // Credentials
        readonly string _accountSid;
        readonly string _secretKey;

        public MRidesWebApi()
        {
            // TODO: Those are not required for now, they can be both null, but
            // once these are setup, maybe centralize them here, don't make it
            // part of the parameter required to instantiate the object.
            _accountSid = null;
            _secretKey = null;
        }

        /**
         * Given an object of interest and an api end point, this method
         * converts the object into a json, and sends it through a rest
         * request to the api end point, then returns the response of 
         * this request.
         */ 
        public IRestResponse SendPost<T>(string apiEndPoint, object objectToUpdate) where T : new()
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
            var response = client.Execute<T>(request);
            return response;
        }

        public IRestResponse SendGetWithUrlSegment<T>(string apiEndPoint, string segmentName, string segmentValue) where T : new()
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
            var response = client.Execute<T>(request);
            return response;
        }

        public IRestResponse SendDeleteWithUrlSegment<T>(string apiEndPoint, string segmentName, string segmentValue) where T : new()
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
            var response = client.Execute<T>(request);
            return response;
        }

        public class ApiEndPointUrl
        {
            // User related url
            public const string createUser = "User/createUser";
            public const string getUser = "User/getUser/{id}";

            // Request related url
            public const string createRequest = "Request/createRequest";
            public const string deleteRequest = "Request/deleteRequest/{id}";
        }
    }
}