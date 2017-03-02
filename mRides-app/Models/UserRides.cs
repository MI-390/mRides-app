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
    public class UserRides
    {
        public int ID { get; set; }

        public int RiderId { get; set; }
        public User Rider { get; set; }

        public int RideId { get; set; }
        public Ride Ride { get; set; }

        public string location { get; set; }
        public string destinaion { get; set; }
        public string driverFeedback { get; set; }
        public string riderFeedback { get; set; }
        public int driverStars { get; set; }
        public int riderStars { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public UserRides()
        {
            this.RiderId = 0;
            this.Rider = new User();
            this.RideId = 0;
            this.Ride = new Ride();
            this.location = "";
            this.destinaion = "";
            this.driverFeedback = "";
            this.riderFeedback = "";
            this.driverStars = 0;
            this.riderStars = 0;
        }
    }
}