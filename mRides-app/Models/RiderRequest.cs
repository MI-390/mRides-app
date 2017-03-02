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
    public class RiderRequest
    {
        public int id { get; set; }
        public int riderID { get; set; }
        public User rider { get; set; }
        public int requestID { get; set; }
        public string location { get; set; }
        public string destination { get; set; }

        public RiderRequest()
        {
            this.id = 0;
            this.riderID = 0;
            this.rider = new Models.User();
            this.requestID = 0;
            this.location = "";
            this.destination = "";
        }
    }
}