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
using mRides_app.Gateways;

namespace UnitTests.Gateways
{
    /// <summary>
    /// This mapper class is only used for testing purposes in order to populate requests.
    /// It should never be used by the activities in the app.
    /// </summary>
    public class RequestGateway : AbstractGateway
    {
        private RequestGateway() { }
        private static RequestGateway _instance;
        public static RequestGateway getInstance()
        {
            if (_instance == null)
            {
                _instance = new RequestGateway();
            }
            return _instance;
        }

        // ---------------------------------------------------------------------------
        // CALLS TO REQUEST WEB API
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Method to create a request
        /// </summary>
        /// <param name="newRequest"></param>
        public void CreateRequest(Request newRequest)
        {
            newRequest.driverID = null;
            SendPost<Request>(ApiEndPointUrl.createRequest, newRequest, true);
        }
    }
}