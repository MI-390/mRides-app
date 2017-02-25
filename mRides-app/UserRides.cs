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

namespace mRides_app
{
    class UserRides
    {
        public int userRideId { get; set; }
        public int rideId { get; set; }
        public int riderId { get; set; } //in DB rider id has only one column but we might have multiple riders? 
        public int driverId { get; set; } //we don't have driver id right now in DB
        public String riderFeedback { get; set; }
        public String driverFeedback { get; set; }
        public double riderRating { get; set; } //we don't have ratings in DB yet
        public double driverRating { get; set; }
        public String location { get; set; }
        public DateTime dateOfRide { get; set; } //also not in DB yet
    }
}