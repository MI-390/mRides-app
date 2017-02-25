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
    public class Request
    {
        public const string TYPE_RIDER = "rider";
        public const string TYPE_DRIVER = "driver";

        public int ID { get; set; }
        public string destination { get; set; }
        public string location { get; set; }
        public DateTime dateTime { get; set; }
        public Boolean isWeekly { get; set; }
        public string type { get; set; }

        //public string type;

        //1 Driver per Ride
        public int? DriverID { get; set; }
        public User Driver { get; set; }

        //Many Riders through an association table
        public ICollection<RiderRequest> RiderRequests { get; set; }

        // Default constructor
        public Request() { }

        // Constructor to send a request
        public Request(int ID, string destination, string location, DateTime dateTime, Boolean isWeekly, string type)
        {
            this.ID = ID;
            this.destination = destination;
            this.location = location;
            this.dateTime = dateTime;
            this.isWeekly = isWeekly;
            this.type = type;
        }
    }
}