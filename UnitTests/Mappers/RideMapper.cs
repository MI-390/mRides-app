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
using mRides_app.Mappers;

namespace UnitTests.Mappers
{
    /// <summary>
    /// The ride mapper class provides an interface for its users to perform operations on the
    /// server related to Rides. This SHOULD NOT be used by the application activities.
    /// </summary>
    public class RideMapper : AbstractMapper
    {
        
        /// <summary>
        /// Singleton class instance
        /// </summary>
        private static RideMapper _instance;

        /// <summary>
        /// Private default constructor since this class is a singleton
        /// </summary>
        private RideMapper() { }

        /// <summary>
        /// Used to obtain the instance of this class, since it is a singleton.
        /// </summary>
        /// <returns>Instance of the ride mapper</returns>
        public static RideMapper getInstance()
        {
            if (_instance == null)
            {
                _instance = new RideMapper();
            }
            return _instance;
        }

        /// <summary>
        /// Method is used to create ride, mainly to setup the preconditions of the unit tests.
        /// </summary>
        /// <param name="ride">Ride object to be created on the server side</param>
        /// <returns>The newly created Ride</returns>
        public Ride CreateRide(Ride ride)
        {
            return SendPost<Ride>(ApiEndPointUrl.createRide, ride, true);
        }

        /// <summary>
        /// Method is used to add a rider (currrent user) to a ride, mainly to setup the preconditions of the unit tests.
        /// </summary>
        /// <param name="rideId">ID of the ride to which a rider will be added</param>
        /// <returns>The ride to which the rider has been added</returns>
        public Ride AddRiderToRide(int rideid, int userid)
        {
            object request = new
            {
                rideid = rideid,
                userid = userid
            };
            return SendPost<Ride>(ApiEndPointUrl.addRiderToRide, request, true);
        }

        /// <summary>
        /// Method is used to obtain a ride given its ID.
        /// </summary>
        /// <param name="rideId">ID of the ride to be obtained</param>
        /// <returns>Ride associated with the given ID</returns>
        public Ride GetRide(int rideId)
        {
            return SendGetWithUrlSegment<Ride>(ApiEndPointUrl.getRide, "id", rideId.ToString());
        }
    }
}