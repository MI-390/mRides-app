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

namespace mRides_app.Models
{
    /// <summary>
    /// Class that corresponds to ride between a driver and possibly many riders.
    /// </summary>
    public class Ride
    {
        public int ID { get; set; }
        public string destination { get; set; }
        public string location { get; set; }
        public DateTime dateTime { get; set; }
        public Boolean isWeekly { get; set; }
        public string type;
        public double distanceTravelled { get; set; }

        //1 Driver per Ride
        public int? DriverID { get; set; }
        public User Driver { get; set; }

        //Many Riders through an association table
        public List<UserRides> UserRides { get; set; }

    }
}