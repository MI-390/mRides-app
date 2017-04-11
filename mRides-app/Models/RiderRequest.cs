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
using Java.IO;

namespace mRides_app.Models
{
    /// <summary>
    /// Class that corresponds to the requests made by a rider.
    /// </summary>
    public class RiderRequest
    {
        public int id { get; set; }
        public int riderID { get; set; }
        public User rider { get; set; }
        public int requestID { get; set; }
        public string location { get; set; }
        public string destination { get; set; }
    }
}