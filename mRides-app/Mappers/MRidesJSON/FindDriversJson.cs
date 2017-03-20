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

namespace MRidesJSON
{
    public class FindDriversJson
    {
        public List<Request> requests { get; set; }
        public int riderRequestID { get; set; }
    }
}