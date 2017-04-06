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
using mRides_app.Tasks.Callbacks;

namespace mRides_app.Tasks
{
    public class FindMatchAsyncTask : AsyncTask<Java.Lang.Void, Java.Lang.Void, Java.Lang.Void>
    {
        // Data necessary to build the request for the match
        private Request userRequest;

        // Object to call once the task is complete
        private IOnFindMatchCompleteCallback callBackObject;

        // List of requests matched
        List<Request> listOfMatchedRequests;

        // Mapper used to make the request
        private ConsoleMapper consoleMapper;

        /// <summary>
        /// Constructor building an asynchronous task that is responsible of finding matched requests
        /// given a request and the callback object that implements the interface IOnFindMatchCompleteCallback,
        /// allowing the invokation of a method upon successfully obtaining the list of matched requests.
        /// </summary>
        /// <param name="userRequest">The request to which other requests should be matched with</param>
        /// <param name="callBackObject">The callback object that will be invoked upon obtaining the matched request list</param>
        public FindMatchAsyncTask(Request userRequest, IOnFindMatchCompleteCallback callBackObject)
        {
            this.userRequest = userRequest;
            this.callBackObject = callBackObject;
            this.listOfMatchedRequests = null;
            this.consoleMapper = ConsoleMapper.getInstance();
        }

        /// <summary>
        /// Obtains the list of requests that match the user request from the server.
        /// </summary>
        /// <param name="params">Void parameters</param>
        /// <returns></returns>
        protected override Java.Lang.Void RunInBackground(params Java.Lang.Void[] @params)
        {
            // Send the request to the server
            if (this.userRequest.type == Request.TYPE_DRIVER)
            {
                this.listOfMatchedRequests = consoleMapper.FindRiders(this.userRequest);
            }
            else
            {
                this.listOfMatchedRequests = consoleMapper.FindDrivers(this.userRequest);
            }

            return null;
        }

        /// <summary>
        /// Invoked upon successfully obtaining the list of matched requests. 
        /// This method will return that list to the callback object.
        /// </summary>
        /// <param name="result">Void parameter</param>
        protected override void OnPostExecute(Java.Lang.Void result)
        {
            this.callBackObject.OnFindMatchComplete(this.listOfMatchedRequests);
        }
    }
}