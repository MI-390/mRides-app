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
using mRides_app.Gateways;

namespace mRides_app.Mappers
{
    /// <summary>
    /// Class that corresponds to the mappers for the Console type.
    /// </summary>
    public class ConsoleMapper : AbstractMapper
    {
        private ConsoleGateway consoleGateway;

        /// <summary>
        /// Private constructor of the console mapper.
        /// </summary>
        private ConsoleMapper ()
        {
            this.consoleGateway = ConsoleGateway.GetInstance();
        }

        private static ConsoleMapper instance;
        public static ConsoleMapper getInstance ()
        {
            if (instance == null)
            {
                instance = new ConsoleMapper();
            }
            return instance;
        }


        // ---------------------------------------------------------------------------
        // CALLS TO CONSOLE WEB API
        // ---------------------------------------------------------------------------

        /// <summary>
        /// This method is used to find a list of rides that match the criteria
        /// of a request made by a rider.
        /// </summary>
        /// <param name="newRequest">New request to be created</param>
        /// <returns>The updated newly created list with the request</returns>
        public List<Request> FindDrivers(Request newRequest)
        {
            return consoleGateway.FindDrivers(newRequest);
        }

        public Request GetRequest(int requestId)
        {
            return consoleGateway.GetRequestById(requestId);
        }

        /**
          * This method is used to find a list of rides that match the criteria
          * of a request made by a driver.
          */

        /// <summary>
        /// This method is used to find a list of rides that match the criteria
        /// of a request made by a driver.
        /// </summary>
        /// <param name="newRequest">New request to be created</param>
        /// <returns>The updated newly created list with the request</returns>
        public List<Request> FindRiders(Request newRequest)
        {
            return consoleGateway.FindRiders(newRequest);
        }

        /// <summary>
        /// Sends the FIRST confirmation, in other words, ask the other user
        /// if he/she accepts to be matched. This does not imply that there is 
        /// match, unless the other users accepts the confirmation.
        /// </summary>
        /// <param name="riderRequestId">ID of the rider's request</param>
        /// <param name="driverRequestId">ID of the driver's request</param>
        /// <returns>The boolean indicating whether the message was sent successfully.</returns>
        public bool Confirm(int riderRequestId, int driverRequestId)
        {
            return consoleGateway.Confirm(riderRequestId, driverRequestId);
        }

        /// <summary>
        /// The other user already accepts the match, upon notifying this user,
        /// if the user also accepts to be match with the other, this method should
        /// be invoked.
        /// </summary>
        /// <param name="riderRequestId">ID of the rider's request</param>
        /// <param name="driverRequestId">ID of the driver's request</param>
        /// <returns>A boolean indicating whether the message was sent successfully.</returns>
        public bool AcceptConfirmation(int riderRequestId, int driverRequestId)
        {
            return consoleGateway.AcceptConfirmation(riderRequestId, driverRequestId);
        }

        /// <summary>
        /// Sets the distance travelled in a ride in order to keep track
        /// of ride metrics.
        /// </summary>
        /// <param name="rideId">ID of the ride in question</param>
        /// <param name="distanceMetric">Value of the distance travelled</param>
        public void setDistanceTravelled(int rideId, double distanceMetric)
        {
            consoleGateway.setDistanceTravelled(rideId, distanceMetric);
        }

        /// <summary>
        /// Sets the duration of travel in a ride in order to keep track
        /// of ride metrics.
        /// </summary>
        /// <param name="rideId">ID of the ride in question</param>
        /// <param name="durationMetric">Value of the duration of time travelled</param>
        public void setDurationTime(int rideId, long durationMetric)
        {
            consoleGateway.setDurationTime(rideId, durationMetric);
        }

    }
}