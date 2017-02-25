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
        public int ID { get; set; }

        public int RiderID { get; set; }
        public User Rider { get; set; }

        public int RequestID { get; set; }
        public Request Request { get; set; }

        public string location { get; set; }


    }
}