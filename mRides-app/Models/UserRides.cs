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
        public string destination { get; set; }
        public string driverFeedback { get; set; }
        public string riderFeedback { get; set; }
        public int driverStars { get; set; }
        public int riderStars { get; set; }


    }
}