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
using Java.Lang;

using mRides_app.Models;
using static mRides_app.Models.Request;
using mRides_app.Mappers;

namespace mRides_app.Tasks
{
    public class FindMatchAsyncTask : AsyncTask<Java.Lang.Void, Java.Lang.Void, Java.Lang.Void>
    {
        // Data necessary to build the request for the match
        string userType;
        List<DestinationCoordinate> coordinates;

        // Object to call once the task is complete
        private IOnFindMatchCompleteCallback callBackObject;

        // List of requests matched
        List<Request> listOfMatchedRequests;

        // Mapper used to make the request
        private ConsoleMapper consoleMapper;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userType"></param>
        /// <param name="coordinates"></param>
        /// <param name="callBackObject"></param>
        public FindMatchAsyncTask(string userType, List<DestinationCoordinate> coordinates, IOnFindMatchCompleteCallback callBackObject)
        {
            this.userType = userType;
            this.coordinates = coordinates;
            this.callBackObject = callBackObject;
            this.listOfMatchedRequests = null;
            this.consoleMapper = ConsoleMapper.getInstance();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        protected override Java.Lang.Void RunInBackground(params Java.Lang.Void[] @params)
        {
            // Send the request to the server
            Request userRequest = new Request
            {
                destinationCoordinates = coordinates,
                destination = coordinates.Last().coordinate,
                location = coordinates.First().coordinate,
                type = this.userType
            };

            if (this.userType == Request.TYPE_DRIVER)
            {
                this.listOfMatchedRequests = consoleMapper.FindRiders(userRequest);
            }
            else
            {
                this.listOfMatchedRequests = consoleMapper.FindDrivers(userRequest);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        protected override void OnPostExecute(Java.Lang.Void result)
        {
            this.callBackObject.OnFindMatchComplete(this.listOfMatchedRequests);
        }
    }
}