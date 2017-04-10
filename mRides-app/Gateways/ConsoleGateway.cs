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

namespace mRides_app.Gateways
{
    public class ConsoleGateway : AbstractGateway
    {
        private ConsoleGateway() { }

        private static ConsoleGateway instance;
        public static ConsoleGateway GetInstance()
        {
            if (instance == null)
            {
                instance = new ConsoleGateway();
            }
            return instance;
        }


        // ---------------------------------------------------------------------------
        // CALLS TO CONSOLE WEB API
        // ---------------------------------------------------------------------------
        
        /// <summary>
        ///  This method is used to find a list of rides that match the criteria
        ///  of a request made by a rider.
        /// </summary>
        /// <param name="newRequest">Request made by a rider</param>
        /// <returns>List of Request that matches the request of the rider</returns>
        public List<Request> FindDrivers(Request newRequest)
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
            var request = new RestRequest(ApiEndPointUrl.findDrivers, Method.POST);
            request.AddHeader(HeaderNameUserId, User.currentUser.id.ToString());
            request.AddParameter("text/json", json, ParameterType.RequestBody);

            // Execute the request and return the response
            var response = client.Execute<FindDriversJson>(request);
            var responseData = response == null ? null : response.Data;
            if (responseData == null)
            {
                return new List<Request>();
            }
            else
            {
                newRequest.ID = responseData.riderRequestID;
                return responseData.requests;
            }
        }
        /// <summary>
        /// This method returns a user object given its user ID.
        /// </summary>
        public Request GetRequestById(int requestId)
        {
            return SendGetWithUrlSegment<Request>(ApiEndPointUrl.getRequestById, "id", requestId.ToString());
        }

        public void deleteRequest(int requestId)
        {
            SendGetWithUrlSegment<Request>(ApiEndPointUrl.deleteRequest, "id", requestId.ToString());
        }
        /// <summary>
        /// This method is used to find a list of rides that match the criteria
        /// of a request made by a driver.
        /// </summary>
        /// <param name="newRequest">Request made by a driver</param>
        /// <returns>List of Request that matches the request made by the driver</returns>
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
            if (responseData == null)
            {
                return new List<Request>();
            }
            else
            {
                newRequest.ID = responseData.driverRequestID;
                return responseData.requests;
            }
        }
        
        /// <summary>
        /// Send the first confirmation, in other words, asks the other user
        /// if he/she accepts to be matched. This does not imply that there is
        /// a match, unless the other user accepts the confirmation.
        /// </summary>
        /// <param name="riderRequestId">ID of the request of the rider</param>
        /// <param name="driverRequestId">ID of the request of the driver</param>
        /// <returns>bool indicating whether the message was sent successfully</returns>
        public bool Confirm(int riderRequestId, int driverRequestId)
        {
            object confirmation = new
            {
                riderRequestId = riderRequestId,
                driverRequestId = driverRequestId
            };
            return SendPost<bool>(ApiEndPointUrl.confirm, confirmation, true);
        }

        public void createRide(Ride ride)
        {
            SendPost<Ride>(ApiEndPointUrl.createRide, ride, false);
        }

        /// <summary>
        /// The other user already accepts the match, upon notifying this user,
        /// if the user also accepts to be match with the first user, this method
        /// should be invoked.
        /// </summary>
        /// <param name="riderRequestId">ID of the request of the rider</param>
        /// <param name="driverRequestId">ID of the request of the driver</param>
        /// <returns>bool indicating whether the message was sent successfully</returns>
        public bool AcceptConfirmation(int riderRequestId, int driverRequestId)
        {
            object confirmationAcceptance = new
            {
                riderRequestId = riderRequestId,
                driverRequestId = driverRequestId
            };
            return SendPost<bool>(ApiEndPointUrl.acceptConfirm, confirmationAcceptance, true);
        }

        /// <summary>
        /// Sets the distance travelled in a ride in order to keep track
        /// of ride metrics.
        /// </summary>
        /// <param name="rideId">ID of the ride in question</param>
        /// <param name="distanceMetric">Value of the distance travelled</param>
        public void setDistanceTravelled(int rideID, double distanceMetric)
        {
            object objectSent = new
            {
                rideId = rideID,
                distanceTravelled = distanceMetric
            };
            SendPost<object>(ApiEndPointUrl.setDistanceTravelled, objectSent, false);
        }

        /// <summary>
        /// Sets the duration of travel in a ride in order to keep track
        /// of ride metrics.
        /// </summary>
        /// <param name="rideId">ID of the ride in question</param>
        /// <param name="durationMetric">Value of the duration of travel</param>
        public void setDurationTime(int rideID, long durationMetric)
        {
            object objectSent = new
            {
                rideId = rideID,
                duration = durationMetric
            };
            SendPost<object>(ApiEndPointUrl.setDuration, objectSent, false);
        }

    }
}