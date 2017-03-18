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
using MRidesJSON;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;

namespace mRides_app.Mappers
{
    public class ConsoleMapper : AbstractMapper
    {
        private ConsoleMapper () { }

        private static ConsoleMapper _instance;
        public static ConsoleMapper getInstance ()
        {
            if (_instance == null)
            {
                _instance = new ConsoleMapper();
            }
            return _instance;
        }


        // ---------------------------------------------------------------------------
        // CALLS TO CONSOLE WEB API
        // ---------------------------------------------------------------------------

        /**
         * This method is used to find a list of rides that match the criteria
         * of a request made by a rider.
         */
        public List<Request> FindDrivers(Request newRequest)
        {
            dynamic response = SendPost<dynamic>(ApiEndPointUrl.findDrivers, newRequest, true);

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
        public List<Request> FindRiders(Request newRequest)
        {
            // Create a new rest client
            var client = new RestClient()
            {
                BaseUrl = new System.Uri(BaseUrl),
                Authenticator = new HttpBasicAuthenticator(_accountSid, _secretKey)
            };

            // Serialize the object of interest into a JSON
            var json = JsonConvert.SerializeObject(newRequest);

            // Make a new request object
            var request = new RestRequest(ApiEndPointUrl.findRiders, Method.POST);
            request.AddHeader(HeaderNameUserId, User.currentUser.id.ToString());
            request.AddParameter("text/json", json, ParameterType.RequestBody);

            // Execute the request and return the response
            var response = client.Execute<FindRidersJson>(request);
            var responseData = response == null ? null : response.Data;
            if(responseData == null)
            {
                return new List<Request>();
            }
            else
            {
                newRequest.ID = responseData.driverRequestID;
                return responseData.requests;
            }
        }

        /**
         * Sends the FIRST confirmation, in other words, ask the other user
         * if he/she accepts to be matched. This does not imply that there is 
         * match, unless the other users accepts the confirmation.
         * 
         * Returns a boolean indicating whether the message was sent successfully.
         */
        public bool Confirm(int riderRequestId, int driverRequestId)
        {
            object confirmation = new
            {
                riderRequestId = riderRequestId,
                driverRequestId = driverRequestId
            };
            return SendPost<bool>(ApiEndPointUrl.confirm, confirmation, true);
        }

        /**
         * The other user already accepts the match, upon notifying this user,
         * if the user also accepts to be match with the other, this method should
         * be invoked.
         * 
         * Returns a boolean indicating whether the messagw was sent successfully.
         */
        public bool AcceptConfirmation(int riderRequestId, int driverRequestId)
        {
            object confirmationAcceptance = new
            {
                riderRequestId = riderRequestId,
                driverRequestId = driverRequestId
            };
            return SendPost<bool>(ApiEndPointUrl.acceptConfirm, confirmationAcceptance, true);
        }
    }
}