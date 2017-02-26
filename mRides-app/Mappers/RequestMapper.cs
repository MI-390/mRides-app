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

namespace mRides_app.Mappers
{
    /**
     * This mapper class is only used for testing purposes in order to populate requests.
     * It should never be used by the activities in the app.
     */ 
    public class RequestMapper : AbstractMapper
    {
        private RequestMapper() { }
        private static RequestMapper _instance;
        public static RequestMapper getInstance()
        {
            if (_instance == null)
            {
                _instance = new RequestMapper();
            }
            return _instance;
        }

        // ---------------------------------------------------------------------------
        // CALLS TO REQUEST WEB API
        // ---------------------------------------------------------------------------

        /**
         * Create a request
         */
        public void CreateRequest(Request newRequest)
        {
            newRequest.driverID = null;
            SendPost<Request>(ApiEndPointUrl.createRequest, newRequest, true);
        }
    }
}