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

        /**
         * This method is used to find a list of rides that match the criteria
         * of a request made by a rider.
         */
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
        public List<Request> FindRiders(Request newRequest)
        {
            return consoleGateway.FindRiders(newRequest);
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
            return consoleGateway.Confirm(riderRequestId, driverRequestId);
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
            return consoleGateway.AcceptConfirmation(riderRequestId, driverRequestId);
        }
    }
}